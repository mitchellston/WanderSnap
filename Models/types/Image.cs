namespace WanderSnap.Models
{
    public class Image
    {
        public long id { get; set; }
        public string description { get; set; }
        public string path { get; set; }
        public DateTime date { get; set; }

        public Image(long id, string description, string path, DateTime date)
        {
            this.id = id;
            this.description = description;
            this.path = path;
            this.date = date;
        }
    }
}