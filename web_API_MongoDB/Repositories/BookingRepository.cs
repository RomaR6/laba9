using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration; 
using web_API_MongoDB.Models;
using web_API_MongoDB.Settings;

namespace web_API_MongoDB.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IMongoCollection<Booking> _bookingsCollection;

        public BookingRepository(IOptions<MongoDbSettings> settings)
        {
            
            var connectionString = settings.Value.ConnectionString;
            var mongoConnectionUrl = new MongoUrl(connectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);

            
            mongoClientSettings.SslSettings = new SslSettings() { CheckCertificateRevocation = false };

            var mongoClient = new MongoClient(mongoClientSettings);
            

            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _bookingsCollection = mongoDatabase.GetCollection<Booking>("Bookings");
        }

        

        public async Task<List<Booking>> GetAllAsync() =>
            await _bookingsCollection.Find(_ => true).ToListAsync();

        public async Task<Booking?> GetByIdAsync(string id) =>
            await _bookingsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<Booking>> GetByStudentIdAsync(string studentId) =>
            await _bookingsCollection.Find(x => x.StudentId == studentId).ToListAsync();

        public async Task<List<Booking>> GetByRoomIdAsync(string roomId) =>
            await _bookingsCollection.Find(x => x.RoomId == roomId).ToListAsync();

        public async Task CreateAsync(Booking booking) =>
            await _bookingsCollection.InsertOneAsync(booking);

        public async Task DeleteAsync(string id) =>
            await _bookingsCollection.DeleteOneAsync(x => x.Id == id);
    }
}