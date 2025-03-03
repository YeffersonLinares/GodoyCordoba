using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GodoyCordoba.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }
        public string Name { get; set; } 
        public string LastName { get; set; } 
        public string Email { get; set; } 
        public string Password { get; set; } 
        public DateOnly? LastLogin { get; set; }
        public int? Score { get; set; }
    }
}