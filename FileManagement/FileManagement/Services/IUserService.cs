using System;
using sharedfile.Models;

namespace sharedfile.Services
{
    public interface IUserService
    {
        public User login(string userId, string password);

        public string generateToken(string userId);

        public bool ValidateCurrentToken(string token);

        public string GetClaim(string token, string claimType);
    }
}
