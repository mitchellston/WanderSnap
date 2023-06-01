using P4_Vacation_photos.Models.DB.Providers;

namespace P4_Vacation_photos.Classes
{
    public class DbHandler
    {
        public SQLiteProvider _Provider = new SQLiteProvider("./database.sqlite");
    }
}