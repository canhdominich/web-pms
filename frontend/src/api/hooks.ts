import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api, compact, errorMessage } from './client';
import { useToast } from '../components/Toast';

export function useGet<T>(url: string | null | undefined, params?: Record<string, unknown>, refetchInterval?: number) {
  const cleaned = compact(params);
  return useQuery({
    queryKey: [url, cleaned],
    queryFn: () => api.get<T>(url!, { params: cleaned }).then((r) => r.data),
    enabled: !!url,
    refetchInterval,
  });
}

export function useAction<V, R = unknown>(
  fn: (vars: V) => Promise<R>,
  options?: { success?: string; onSuccess?: (result: R, vars: V) => void },
) {
  const queryClient = useQueryClient();
  const toast = useToast();
  return useMutation({
    mutationFn: fn,
    onSuccess: (result, vars) => {
      void queryClient.invalidateQueries();
      if (options?.success) toast.success(options.success);
      options?.onSuccess?.(result, vars);
    },
    onError: (error) => toast.error(errorMessage(error)),
  });
}

export const http = {
  post: <R = unknown>(url: string, body?: unknown) => api.post<R>(url, body).then((r) => r.data),
  put: <R = unknown>(url: string, body?: unknown) => api.put<R>(url, body).then((r) => r.data),
  patch: <R = unknown>(url: string, body?: unknown) => api.patch<R>(url, body).then((r) => r.data),
  delete: <R = unknown>(url: string) => api.delete<R>(url).then((r) => r.data),
};
