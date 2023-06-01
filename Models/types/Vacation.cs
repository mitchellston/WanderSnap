namespace P4_Vacation_photos.Classes
{
    public class Vacation
    {
        public long id { get; set; }
        public long user { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string[] images { get; set; }
        public Vacation(long id, long user, string name, string? description, DateTime start, DateTime end, string[] images)
        {
            this.id = id;
            this.user = user;
            this.name = name;
            this.description = description;
            this.start = start;
            this.end = end;
            this.images = images;
        }

    }
}