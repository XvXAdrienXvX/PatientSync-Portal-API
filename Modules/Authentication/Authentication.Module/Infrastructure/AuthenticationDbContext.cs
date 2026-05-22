using Authentication.Module.Infrastructure.Persistence;
using MongoDB.Driver;

namespace Authentication.Module.Infrastructure
{
    public class AuthenticationDbContext
    {
        private readonly IMongoDatabase _db;
        public AuthenticationDbContext(IMongoDatabase db) => _db = db;

        public IMongoCollection<UsersDO> Users => _db.GetCollection<UsersDO>("auth.users");
    }
}
