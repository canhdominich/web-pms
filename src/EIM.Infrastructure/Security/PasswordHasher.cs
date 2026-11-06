using EIM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using IAppPasswordHasher = EIM.Application.Abstractions.IPasswordHasher;

namespace EIM.Infrastructure.Security;

public sealed class PasswordHasher : IAppPasswordHasher
{
    private static readonly AuthUser Subject = new();
    private readonly PasswordHasher<AuthUser> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(Subject, password);

    public bool Verify(string hash, string password) =>
        _hasher.VerifyHashedPassword(Subject, hash, password) != PasswordVerificationResult.Failed;
}
