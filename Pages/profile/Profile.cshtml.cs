using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P4_Vacation_photos.Classes;
using P4_Vacation_photos.Classes.api;
namespace P4_Vacation_photos.Pages;
public class ProfileModel : PageModel
{
    private DbHandler _DB = new DbHandler();
    private readonly ILogger<IndexModel> _logger;
    public User _User;
    public long _HowManyVacations = 0;
    [BindProperty(SupportsGet = true)]
    public string Username { get; set; }

    public ProfileModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        var user = this._DB._Provider.select("User",
        new string[] { "id", "username", "email", "description", "picture", "created_at" },
        new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("username", Models.DB.Primitives.Compare.Equal, Username)
        });
        if (user.Count() != 1)
        {
            return RedirectToPage("/Index");
        }
        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(user[0]._columns.Find(col => col._column == "created_at")._value);
        this._User = new User(
            user[0]._columns.Find(col => col._column == "id")._value,
            user[0]._columns.Find(col => col._column == "username")._value,
            user[0]._columns.Find(col => col._column == "email")._value,
            user[0]._columns.Find(col => col._column == "description")._value,
            user[0]._columns.Find(col => col._column == "picture")._value,
            start
        );
        this._HowManyVacations = this._DB._Provider.count("Vacation", new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal, this._User.id.ToString())
        });
        return Page();
    }

    public JsonResult OnGetVacations([FromQuery] ProfileGetVacations data)
    {
        var response = new ApiResponse<Vacation?>(false, "Nothing found", null);
        var userFetch = this._DB._Provider.select("User",
        new string[] { "id" },
        new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("username", Models.DB.Primitives.Compare.Equal, Username)
        });
        if (userFetch.Count() != 1)
        {
            return new JsonResult(response);
        }
        long userId = userFetch[0]._columns.Find(col => col._column == "id")._value;
        var vacationFetch = this._DB._Provider.select("Vacation", new string[] { "id", "User", "name", "description", "start", "end" }, new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal, userId.ToString()),
        }, 1, data.which);
        if (vacationFetch.Count() != 1)
        {
            return new JsonResult(response);
        }
        var photoFetch = this._DB._Provider.select("Vacation_Photo", new string[] { "path" }, new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("Vacation", Models.DB.Primitives.Compare.Equal, ((long) vacationFetch[0]._columns.Find(col => col._column == "id")._value).ToString()),
        }, 1);
        var vacation = new Vacation(
            vacationFetch[0]._columns.Find(col => col._column == "id")._value,
            vacationFetch[0]._columns.Find(col => col._column == "User")._value,
            vacationFetch[0]._columns.Find(col => col._column == "name")._value,
            vacationFetch[0]._columns.Find(col => col._column == "description")._value,
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(vacationFetch[0]._columns.Find(col => col._column == "start")._value),
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(vacationFetch[0]._columns.Find(col => col._column == "end")._value),
            photoFetch.Count() == 1 ? new string[] { photoFetch[0]._columns.Find(col => col._column == "id")._value.ToString() } : new string[] { }
        );

        return response.CreateJsonResult(true, "Found", vacation);
    }
}

