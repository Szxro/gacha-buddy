namespace Account.Application.Contracts;

public interface IHashService
{
    (string hash, byte[] salt) GetHashAndSalt(string password);

    bool VerifyHash(string password, string currentHash, string currentSalt);
}