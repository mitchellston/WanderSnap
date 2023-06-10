using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P4_Vacation_photos.Classes;
namespace P4_Vacation_photos.Pages;
[Authorize]
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
            var vacations = this._DB._Provider.select("Vacation", new string[] { "id", "name" },
                new Models.DB.Primitives.Where[] {
                    new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal , ((long)users[i]._columns.Find(x => x._column == "id")?._value).ToString())
            }, 3);
            var vacationCount = this._DB._Provider.count("Vacation", new Models.DB.Primitives.Where[] {
                new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal , ((long)users[i]._columns.Find(x => x._column == "id")?._value).ToString())
            });
            User.Vacation[] vacationsArray = new User.Vacation[vacations.Length];
            for (int j = 0; j < vacations.Length; j++)
            {
                vacationsArray[j] = new User.Vacation(
                    (long)vacations[j]._columns.Find(x => x._column == "id")?._value,
                    (string)vacations[j]._columns.Find(x => x._column == "name")?._value
                );
            }
            this._Users[i] = new User(
                (long)users[i]._columns.Find(x => x._column == "id")?._value,
                users[i]._columns.Find(x => x._column == "username")?._value,
                users[i]._columns.Find(x => x._column == "email")?._value,
                users[i]._columns.Find(x => x._column == "description")?._value,
                users[i]._columns.Find(x => x._column == "profile_picture")?._value,
                DateTimeOffset.FromUnixTimeSeconds((long)users[i]._columns.Find(col => col._column == "created_at")?._value).DateTime,
                vacationsArray,
                vacationCount
            );
        }
    }
    public new class User
    {
        public P4_Vacation_photos.Classes.User user { get; set; }
        public Vacation[] vacations { get; set; }
        public int vacationCount { get; set; }
        public class Vacation
        {
            public long id { get; set; }
            public string name { get; set; }
            public Vacation(long id, string name)
            {
                this.id = id;
                this.name = name;
            }
        }
        public User(long id, string username, string email, string? description, string? profilePicture, DateTime createdAt, Vacation[] vacations, int vacationCount)
        {
            this.user = new P4_Vacation_photos.Classes.User(
                id,
                username,
                email,
                description ?? "",
                profilePicture ?? "",
                createdAt
            );
            this.vacations = vacations;
            this.vacationCount = vacationCount;
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

