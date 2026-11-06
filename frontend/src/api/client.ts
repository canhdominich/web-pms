import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios';
import type { AuthResponse, TokenPair } from './types';

const STORAGE_KEY = 'eim.tokens';

export const tokenStore = {
  get(): TokenPair | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as TokenPair) : null;
  },
  set(tokens: TokenPair) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens));
  },
  clear() {
    localStorage.removeItem(STORAGE_KEY);
  },
};

export class ApiError extends Error {
  constructor(
    message: string,
    public status: number,
    public code: string,
    public details?: unknown,
  ) {
    super(message);
  }
}

type ErrorBody = { code?: string; message?: string; details?: unknown };
type RetryConfig = InternalAxiosRequestConfig & { _retry?: boolean };

const authFailureListeners = new Set<() => void>();
export const onAuthFailure = (listener: () => void) => {
  authFailureListeners.add(listener);
  return () => {
    authFailureListeners.delete(listener);
  };
};

export const api = axios.create({ baseURL: '/api' });

api.interceptors.request.use((config) => {
  const tokens = tokenStore.get();
  if (tokens) config.headers.Authorization = `Bearer ${tokens.accessToken}`;
  return config;
});

let refreshing: Promise<string | null> | null = null;

async function refreshAccessToken(): Promise<string | null> {
  const tokens = tokenStore.get();
  if (!tokens) return null;
  try {
    const { data } = await axios.post<AuthResponse>('/api/auth/refresh', { refreshToken: tokens.refreshToken });
    tokenStore.set(data.tokens);
    return data.tokens.accessToken;
  } catch {
    tokenStore.clear();
    return null;
  }
}

async function toApiError(error: AxiosError): Promise<ApiError> {
  const status = error.response?.status ?? 0;
  let body = error.response?.data as ErrorBody | Blob | undefined;
  if (body instanceof Blob) {
    try {
      body = JSON.parse(await body.text()) as ErrorBody;
    } catch {
      body = undefined;
    }
  }
  const parsed = body as ErrorBody | undefined;
  const fallback = status === 403 ? 'Bạn không có quyền thực hiện thao tác này' : status === 0 ? 'Không kết nối được máy chủ' : 'Đã có lỗi xảy ra';
  return new ApiError(parsed?.message ?? fallback, status, parsed?.code ?? 'ERROR', parsed?.details);
}

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as RetryConfig | undefined;
    if (error.response?.status === 401 && original && !original._retry && !original.url?.startsWith('/auth/')) {
      original._retry = true;
      refreshing ??= refreshAccessToken().finally(() => {
        refreshing = null;
      });
      const token = await refreshing;
      if (token) {
        original.headers.Authorization = `Bearer ${token}`;
        return api(original);
      }
      authFailureListeners.forEach((listener) => listener());
    }
    return Promise.reject(await toApiError(error));
  },
);

export function errorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    if (error.details && typeof error.details === 'object' && !Array.isArray(error.details)) {
      const lines = Object.entries(error.details as Record<string, unknown>)
        .flatMap(([, value]) => (Array.isArray(value) ? value : []))
        .filter(Boolean);
      if (lines.length > 0) return `${error.message}: ${lines.join('; ')}`;
    }
    return error.message;
  }
  return error instanceof Error ? error.message : 'Đã có lỗi xảy ra';
}

function fileNameOf(disposition: string | undefined, fallback: string) {
  const match = disposition?.match(/filename\*=UTF-8''([^;]+)|filename="?([^";]+)"?/i);
  return match ? decodeURIComponent(match[1] ?? match[2]) : fallback;
}

export async function download(url: string, params?: Record<string, unknown>, fallbackName = 'export.xlsx') {
  const response = await api.get<Blob>(url, { params: compact(params), responseType: 'blob' });
  const href = URL.createObjectURL(response.data);
  const anchor = document.createElement('a');
  anchor.href = href;
  anchor.download = fileNameOf(response.headers['content-disposition'] as string | undefined, fallbackName);
  anchor.click();
  URL.revokeObjectURL(href);
}

export async function upload<T>(url: string, file: File) {
  const form = new FormData();
  form.append('file', file);
  const response = await api.post<T>(url, form, { validateStatus: (s) => (s >= 200 && s < 300) || s === 422 });
  return response.data;
}

export function compact(params?: Record<string, unknown>) {
  if (!params) return undefined;
  return Object.fromEntries(Object.entries(params).filter(([, v]) => v !== undefined && v !== null && v !== ''));
}
