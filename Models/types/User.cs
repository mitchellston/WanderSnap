namespace P4_Vacation_photos.Classes
{
    public class User
    {
        public long id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string? description { get; set; }
        public string? profilePicture { get; set; }
        public DateTime createdAt { get; set; }
        public User(long id, string username, string email, string description, string profilePicture, DateTime createdAt)
        {
            this.id = id;
            this.username = username;
            this.email = email;
            this.description = description;
            this.profilePicture = profilePicture;
            this.createdAt = createdAt;
        }
    }
}