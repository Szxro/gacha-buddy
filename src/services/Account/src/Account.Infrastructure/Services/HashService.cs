using System.Security.Cryptography;
using Account.Application.Contracts;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Account.Infrastructure.Services;

[Inject(serviceKind:ServiceKind.Service, serviceLifetime:ServiceLifetime.Transient)]
public class HashService : IHashService
{
    private readonly HashOptions _hashOptions;
    
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    
    public HashService(IOptions<HashOptions> hashOptions)
    {
        _hashOptions = hashOptions.Value;
    }
    
    public (string hash, byte[] salt) GetHashAndSalt(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(_hashOptions.SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            _hashOptions.Iterations,
            Algorithm,
            _hashOptions.HashSize);

        return (Convert.ToHexString(hash), salt);
    }

    public bool VerifyHash(string password, string currentHash, string currentSalt)
    {
        // Converting from string to byte[] (return its representation in byte[])
        byte[] salt = Convert.FromHexString(currentSalt);

        byte[] hash = Convert.FromHexString(currentHash);

        // Re-creating the hash
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            _hashOptions.Iterations,
            Algorithm,
            _hashOptions.HashSize);

        // Comparing both base on the length
        return CryptographicOperations.FixedTimeEquals(hash, inputHash); // recommend way of doing to prevent timing attacks
    }
}