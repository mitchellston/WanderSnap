using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P4_Vacation_photos.Classes;
using P4_Vacation_photos.Classes.api;
namespace P4_Vacation_photos.Pages;
[Authorize]
public class ProfileModel : PageModel
{
    private DbHandler _DB = new DbHandler();
    private readonly ILogger<IndexModel> _logger;
    private IWebHostEnvironment _environment;
    public User _User;
    public long _HowManyVacations = 0;
    [BindProperty(SupportsGet = true)]
    public string Username { get; set; }

    public ProfileModel(IWebHostEnvironment environment, ILogger<IndexModel> logger)
    {
        _logger = logger;
        _environment = environment;
    }

    public IActionResult OnGet()
    {
        // if userName is not id, but username, then redirect to the id
        bool usesId = Int64.TryParse(Username, out long id);
        var user = this._DB._Provider.select("User",
        new string[] { "id", "username", "email", "description", "created_at", "profile_picture" },
        new Models.DB.Primitives.Where[] {
            usesId ?
            new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, Username)
            : new Models.DB.Primitives.Where("username", Models.DB.Primitives.Compare.Equal, Username)
        });
        if (usesId == false) return RedirectToPage("/profile/Profile", null, new { Username = user[0]._columns.Find(col => col._column == "id")?._value });
        if (user.Count() != 1) return RedirectToPage("/Index");
        // set the user
        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(user[0]._columns.Find(col => col._column == "created_at")?._value);
        this._User = new User(
            user[0]._columns.Find(col => col._column == "id")?._value,
            user[0]._columns.Find(col => col._column == "username")?._value,
            user[0]._columns.Find(col => col._column == "email")?._value,
            user[0]._columns.Find(col => col._column == "description")?._value,
            user[0]._columns.Find(col => col._column == "profile_picture")?._value == "" ? null : user[0]._columns.Find(col => col._column == "profile_picture")?._value,
            start
        );
        // set the number of vacations
        this._HowManyVacations = this._DB._Provider.count("Vacation", new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal, this._User.id.ToString())
        });
        return Page();
    }
    [HttpPost]
    public JsonResult OnPostEditProfile([FromForm] ProfilePostEditProfile? data)
    {
        var response = new ApiResponse<List<Models.DB.Primitives.Column>?>(false, "Nothing to change", null);
        // Check if the user is logged in
        if (data == null) return response.CreateJsonResult(false, "Nothing to change", null);
        if (User.Identity?.IsAuthenticated == false) return response.CreateJsonResult(false, "You are not allowed to do this", null);
        var currentUserInfo = this._DB._Provider.select("User", new string[] { "id", "username", "description", "profile_picture" }, new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, User.Identity?.Name ?? "")
        });
        // Get the data to update
        var updatingData = new List<Models.DB.Primitives.Column>() { };
        data.GetType().GetProperties().ToList().ForEach(property =>
        {
            // Check if the property is not null, not empty, not the same as the current value and not the profile picture
            if (
                property.GetValue(data) != null &&
                property.GetValue(data)?.ToString() != "" &&
                property.GetValue(data)?.ToString() != currentUserInfo[0]._columns.Find(col => col._column == property.Name)?._value &&
                property.Name != "profilePicture"
            )
            {
                updatingData.Add(new Models.DB.Primitives.Column(property.Name, property.GetValue(data)?.ToString()));
            }
        });
        // Update the profile picture if there is a new one
        if (data.profilePicture != null)
        {
            // Check if the file is an image
            if (data.profilePicture.ContentType.Split("/")[0] != "image") return response.CreateJsonResult(false, "The file is not an image", null);
            // Check if the file is not too big
            if (data.profilePicture.Length > 1000000) return response.CreateJsonResult(false, "The file is too big", null);
            // Check if the folder /uploads/profile/profilePics exists and create it if it doesn't
            if (!Directory.Exists(Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/profile/profilePics"))) Directory.CreateDirectory(Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/profile/profilePics"));
            // Upload the new profile picture + add to the updatingData
            var fileName = "user_" + currentUserInfo[0]._columns.Find(col => col._column == "id")?._value + "_photo_" + DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + "." + data.profilePicture.ContentType.Split("/")[1];
            var file = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/profile/profilePics", fileName);
            using (var filestream = new FileStream(file, FileMode.Create))
            {
                data.profilePicture.CopyTo(filestream);
            }
            updatingData.Add(new Models.DB.Primitives.Column("profile_picture", fileName));
            // Delete the old profile picture
            if (currentUserInfo[0]._columns.Find(col => col._column == "profile_picture")?._value != null)
            {
                var oldFile = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/profile/profilePics", currentUserInfo[0]._columns.Find(col => col._column == "profile_picture")?._value);
                if (System.IO.File.Exists(oldFile)) System.IO.File.Delete(oldFile);
            }

        }
        // Check if there is something to update
        if (updatingData.Count() == 0) return response.CreateJsonResult(false, "Nothing to change", null);
        // Update the user
        this._DB._Provider.update("User", updatingData, new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, User.Identity?.Name ?? "")
        }, 0);
        return response.CreateJsonResult(true, "Profile updated", updatingData);
    }
    public JsonResult OnPostDeleteVacation([FromBody] ProfileGetVacations data)
    {
        var response = new ApiResponse<bool?>(false, "Can't find this vacation", null);
        if (data == null || data.which == null) return response.CreateJsonResult(false, "Can't find this vacation", null);
        if (User.Identity?.IsAuthenticated == false) return response.CreateJsonResult(false, "You are not allowed to do this", null);
        // Look if the user owns this vacation
        var vacationFetch = this._DB._Provider.select("Vacation", new string[] { "id" }, new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, data.which.ToString() ?? ""),
            new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal, User.Identity?.Name ?? "")
        }, 1);
        if (vacationFetch.Count() != 1) return response.CreateJsonResult(false, "Can't find this vacation", null);
        // Delete all the photos and records in db (multithreaded)
        Thread thread = new Thread(() =>
        {
            Console.WriteLine("Thread started for deleting photos - " + vacationFetch[0]._columns.Find(col => col._column == "id")._value.ToString() ?? "");
            var photosFetch = this._DB._Provider.select("Vacation_Photo", new string[] { "id", "path" }, new Models.DB.Primitives.Where[] {
                new Models.DB.Primitives.Where("Vacation", Models.DB.Primitives.Compare.Equal, vacationFetch[0]._columns.Find(col => col._column == "id")._value.ToString() ?? ""),
            }, -1);
            Console.WriteLine("Found " + photosFetch.Count() + " photos");
            for (var i = 0; i < photosFetch.Count(); i++)
            {
                var photo = photosFetch[i];
                this._DB._Provider.delete("Vacation_Photo", new Models.DB.Primitives.Where[] {
                    new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, photosFetch[i]._columns.Find(col => col._column == "id")._value.ToString() ?? ""),
                    new Models.DB.Primitives.Where("Vacation", Models.DB.Primitives.Compare.Equal, data.which.ToString() ?? ""),
                }, 0);
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/profile/vacations", photo._columns.Find(col => col._column == "path")._value);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
        });
        thread.Start();
        // Delete the vacation
        this._DB._Provider.delete("Vacation", new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, data.which.ToString() ?? ""),
            new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal, User.Identity?.Name ?? "")
        }, 0);
        return response.CreateJsonResult(true, "Vacation deleted", null);
    }
    [HttpGet]
    public JsonResult OnGetVacations([FromQuery] ProfileGetVacations data)
    {
        var response = new ApiResponse<Vacation?>(false, "Nothing found", null);
        if (data == null) return new JsonResult(response);
        // Check if the user exists
        var userFetch = this._DB._Provider.select("User",
            new string[] { "id" },
            new Models.DB.Primitives.Where[] {
                new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, Username)
        });
        if (userFetch.Count() != 1)
        {
            return new JsonResult(response);
        }
        // get the vacation
        long userId = userFetch[0]._columns.Find(col => col._column == "id")._value;
        var vacationFetch = this._DB._Provider.select("Vacation", new string[] { "id", "User", "name", "description", "start", "end" }, new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal, userId.ToString()),
        }, 1, offset: data.which);
        if (vacationFetch.Count() != 1)
        {
            return new JsonResult(response);
        }
        // get the photo
        var photoFetch = this._DB._Provider.select("Vacation_Photo", new string[] { "path" }, new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("Vacation", Models.DB.Primitives.Compare.Equal, ((long) vacationFetch[0]._columns.Find(col => col._column == "id")._value).ToString()),
        }, 1);
        // return the vacation + photo
        var vacation = new Vacation(
            vacationFetch[0]._columns.Find(col => col._column == "id")?._value,
            vacationFetch[0]._columns.Find(col => col._column == "User")?._value,
            vacationFetch[0]._columns.Find(col => col._column == "name")?._value,
            vacationFetch[0]._columns.Find(col => col._column == "description")?._value,
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(vacationFetch[0]._columns.Find(col => col._column == "start")?._value),
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(vacationFetch[0]._columns.Find(col => col._column == "end")?._value),
            photoFetch.Count() == 1 ? new string[] { (photoFetch[0]._columns.Find(col => col._column == "path")?._value.ToString()) ?? "" } : new string[] { }
        );
        return response.CreateJsonResult(true, "Found", vacation);
    }
}

