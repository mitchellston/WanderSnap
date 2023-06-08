﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P4_Vacation_photos.Classes;
using System.ComponentModel.DataAnnotations;
namespace P4_Vacation_photos.Pages;
public class VacationsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty(SupportsGet = true)]
    public string Username { get; set; }
    [BindProperty(SupportsGet = true)]
    public int VacationId { get; set; }

    [BindProperty(SupportsGet = false), Required(ErrorMessage = "Please give a photo of the vacation")]
    public IFormFile form_photo { get; set; }
    [BindProperty(SupportsGet = false)]
    public DateTime form_date { get; set; }
    [BindProperty(SupportsGet = false), StringLength(255, ErrorMessage = "The description is too long")]
    public string? form_description { get; set; }
    private IWebHostEnvironment _environment;
    public bool _HasError = false;
    private DbHandler _DB = new DbHandler();
    public User _User;
    public Vacation _Vacation;
    public int _HowManyPhotos;
    public VacationsModel(IWebHostEnvironment environment, ILogger<IndexModel> logger)
    {
        _logger = logger;
        _environment = environment;
    }

    public IActionResult OnGet()
    {
        bool usesId = Int64.TryParse(Username, out long id);
        var user = this._DB._Provider.select("User",
        new string[] { "id", "username", "email", "description", "picture", "created_at" },
        new Models.DB.Primitives.Where[] {
            usesId ?
            new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, Username)
            : new Models.DB.Primitives.Where("username", Models.DB.Primitives.Compare.Equal, Username)
        });
        //if (usesId == false) return RedirectToPage("/profile/vacation/Vacation", null, new { Username = user[0]._columns.Find(col => col._column == "id")._value, VacationId = VacationId });
        if (user.Count() != 1) return RedirectToPage("/Index");

        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(user[0]._columns.Find(col => col._column == "created_at")._value);
        this._User = new User(
            user[0]._columns.Find(col => col._column == "id")?._value,
            user[0]._columns.Find(col => col._column == "username")?._value,
            user[0]._columns.Find(col => col._column == "email")?._value,
            user[0]._columns.Find(col => col._column == "description")?._value,
            user[0]._columns.Find(col => col._column == "picture")?._value,
            start
        );
        var vacation = this._DB._Provider.select("Vacation",
        new string[] { "id", "name", "description", "start", "end" },
        new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, VacationId.ToString())
        });
        if (vacation.Count() != 1) return RedirectToPage("/profile/Profile", null, new { Username = this._User.id });
        start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(vacation[0]._columns.Find(col => col._column == "start")?._value);
        this._Vacation = new Vacation(
            vacation[0]._columns.Find(col => col._column == "id")?._value,
            this._User.id,
            vacation[0]._columns.Find(col => col._column == "name")?._value,
            vacation[0]._columns.Find(col => col._column == "description")?._value,
            start,
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(vacation[0]._columns.Find(col => col._column == "end")?._value),
            new string[] { }
        );
        this._HowManyPhotos = this._DB._Provider.count("Vacation_Photo", new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("Vacation", Models.DB.Primitives.Compare.Equal, this._Vacation.id.ToString())
        });
        return Page();
    }
    [IgnoreAntiforgeryToken(Order = 1001)]
    [HttpPost]
    public IActionResult OnPostAddPhoto()
    {
        // validate input
        this._HasError = false;
        Console.WriteLine("fweoiwefoifehwowefh qhdoqwhd");
        if (ModelState.IsValid == false)
        {
            this.OnGet();
            this._HasError = true;
            return Page();
        }
        var fileValidation = this.FileValidation(this.form_photo);
        if (fileValidation.status == false)
        {
            ModelState.AddModelError("form_photo", fileValidation.error);
            this.OnGet();
            this._HasError = true;
            return Page();
        }
        if (this.form_date > DateTime.UtcNow.AddDays(1))
        {
            ModelState.AddModelError("form_date", "The date cannot be in the future");
            this.OnGet();
            this._HasError = true;
            return Page();
        }
        // check if the vacation exists
        if (this._DB._Provider.count("Vacation", new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("id", Models.DB.Primitives.Compare.Equal, this.VacationId.ToString()),
            new Models.DB.Primitives.Where("User", Models.DB.Primitives.Compare.Equal, User.Identity.Name)
        }) < 1)
        {
            ModelState.AddModelError("form_photo", "The vacation does not exist");
            this.OnGet();
            this._HasError = true;
            return Page();
        }
        // upload the file
        var fileName = "vacation_" + this.VacationId + "_photo_" + DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + "." + this.form_photo.ContentType.Split("/")[1];
        var file = Path.Combine(_environment.ContentRootPath, "wwwroot/profile/vacations", fileName);
        using (var filestream = new FileStream(file, FileMode.Create))
        {
            this.form_photo.CopyTo(filestream);
        }
        // insert the photo
        var photo = this._DB._Provider.insert("Vacation_Photo", new Models.DB.Primitives.Column[] {
            new Models.DB.Primitives.Column("Vacation", this.VacationId.ToString()),
            new Models.DB.Primitives.Column("path", fileName),
            new Models.DB.Primitives.Column("description", this.form_description ?? ""),
            new Models.DB.Primitives.Column("date", this.form_date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds.ToString())
        }.ToList());

        return RedirectToPage("/profile/vacation/Vacation", null, new { Username = this.Username, VacationId = this.VacationId });
    }
    private (string error, bool status) FileValidation(IFormFile file)
    {
        if (file == null) return (error: "Please give a photo of the vacation", status: false);
        if (file.Length > 1000000) return (error: "The photo is too big", status: false);
        if (file.ContentType != "image/jpeg" && file.ContentType != "image/png") return (error: "The photo is not a jpeg or png", status: false);
        return (error: "", status: true);
    }
}

