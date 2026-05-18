using Authentication.Module.Domain;
using MongoDB.Driver;

namespace Authentication.Module.Infrastructure
{
    public class AuthenticationDbContext
    {
        private readonly IMongoDatabase _db;
        public AuthenticationDbContext(IMongoDatabase db) => _db = db;

        public IMongoCollection<Users> Users => _db.GetCollection<Users>("auth.users");
    }
}
