using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration; 
using web_API_MongoDB.Models;
using web_API_MongoDB.Settings;

namespace web_API_MongoDB.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IMongoCollection<Student> _studentsCollection;

        
        public StudentRepository(IOptions<MongoDbSettings> settings)
        {
            var connectionString = settings.Value.ConnectionString;
            var mongoConnectionUrl = new MongoUrl(connectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);

            mongoClientSettings.SslSettings = new SslSettings() { CheckCertificateRevocation = false };

            var mongoClient = new MongoClient(mongoClientSettings);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _studentsCollection = mongoDatabase.GetCollection<Student>("Students");
        }

        public async Task<List<Student>> GetAllAsync() =>
            await _studentsCollection.Find(_ => true).ToListAsync();

        public async Task<Student?> GetByIdAsync(string id) =>
            await _studentsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        
        public async Task<Student?> GetByEmailAsync(string email) =>
            await _studentsCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
       

        public async Task CreateAsync(Student student) =>
            await _studentsCollection.InsertOneAsync(student);

        public async Task UpdateAsync(string id, Student studentIn) =>
            await _studentsCollection.ReplaceOneAsync(x => x.Id == id, studentIn);

        public async Task DeleteAsync(string id) =>
            await _studentsCollection.DeleteOneAsync(x => x.Id == id);
    }
}