using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using TLighthouse.Application.Abstractions;
using TLighthouse.Infrastructure;
using TLighthouse.Infrastructure.Persistence;
using TLighthouse.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace TLighthouse.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public const string AuthRateLimit = "auth";
    public const string CorsPolicy = "default";

    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = context =>
                new BadRequestObjectResult(new ErrorResponse(
                    "VALIDATION_ERROR",
                    "Dữ liệu không hợp lệ",
                    context.ModelState.Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()),
                    context.HttpContext.TraceIdentifier)));

        var jwt = configuration.GetSection(JwtOptions.Section).Get<JwtOptions>() ?? new JwtOptions();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = TokenService.CreateKey(jwt.SigningKey),
                    NameClaimType = TokenService.NameClaim,
                    RoleClaimType = TokenService.RoleClaim,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });
        services.AddAuthorization();

        var authPermits = configuration.GetValue("RateLimit:AuthPermitPerMinute", 10);
        services.AddRateLimiter(o =>
        {
            o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            o.AddPolicy(AuthRateLimit, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = authPermits, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
        });

        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(o => o.AddPolicy(CorsPolicy, p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()
            .WithExposedHeaders("Content-Disposition")));

        services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo { Title = "T-Lighthouse Preschool Management API", Version = "v1" });
            o.CustomSchemaIds(t => t.FullName?.Replace('+', '.'));
            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };
            o.AddSecurityDefinition("Bearer", scheme);
            o.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = [] });
        });
        return services;
    }
}
