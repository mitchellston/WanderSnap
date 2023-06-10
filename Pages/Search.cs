using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P4_Vacation_photos.Classes;
namespace P4_Vacation_photos.Pages;

public class SearchModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private DbHandler _DB = new DbHandler();
    public User[] _Users = new User[5];

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }
    public SearchModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        Models.DB.Primitives.Row[] users;
        if (this.Search == null)
        {
            users = this.OnGetNoSearch();
        }
        else
        {
            users = this.OnGetSearch(this.Search);
        }
        this._Users = new User[users.Length];
        for (int i = 0; i < users.Length; i++)
        {
            this._Users[i] = new User(
                (long)users[i]._columns.Find(x => x._column == "id")?._value,
                users[i]._columns.Find(x => x._column == "username")?._value,
                users[i]._columns.Find(x => x._column == "email")?._value,
                users[i]._columns.Find(x => x._column == "description")?._value,
                users[i]._columns.Find(x => x._column == "profile_picture")?._value,
                DateTimeOffset.FromUnixTimeSeconds((long)users[i]._columns.Find(col => col._column == "created_at")?._value).DateTime
            );
        }
    }
    private Models.DB.Primitives.Row[] OnGetSearch(string search)
    {
        // Get users from the database
        return this._DB._Provider.rawQuery("SELECT * FROM `User` WHERE `username` LIKE '%@Search%' OR `email` LIKE '%@Search%'", new (string column, dynamic value)[] { (column: "Search", search) });
    }
    private Models.DB.Primitives.Row[] OnGetNoSearch()
    {
        // Get random users from the database
        return this._DB._Provider.rawQuery("SELECT * FROM `User` ORDER BY RANDOM() LIMIT 5");
    }
}

