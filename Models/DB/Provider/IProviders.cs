using WanderSnap.Models.DB.Primitives;
namespace WanderSnap.Models.DB.Providers
{
    public interface IDbProvider
    {
        bool insert(string table, List<WanderSnap.Models.DB.Primitives.Column> data);
        bool update(string table, List<WanderSnap.Models.DB.Primitives.Column> data, Where[]? where = null, int limit = 1);
        bool delete(string table, Where[]? where = null, int limit = 1);
        Row[] select(string table, string[]? columns = null, Where[]? where = null, int? limit = 1, int? offset = null);
        int count(string table, Where[]? where = null);
        Row[] rawQuery(string query, (string column, dynamic value)[]? parameters = null);
    }
}