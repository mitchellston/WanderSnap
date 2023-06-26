using WanderSnap.Models.DB.Providers;

namespace WanderSnap.Models
{
    public class DbHandler
    {
        public SQLiteProvider _Provider = new SQLiteProvider("./database.sqlite");
    }
}