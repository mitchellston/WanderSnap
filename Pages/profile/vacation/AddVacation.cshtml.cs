using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P4_Vacation_photos.Classes;
using P4_Vacation_photos.Models.DB.Primitives;

namespace P4_Vacation_photos.Pages;
[Authorize]
public class AddVacationModel : PageModel
{
    private DbHandler _DB = new DbHandler();
    private readonly ILogger<IndexModel> _logger;
    public bool _IsEditing = false;
    [BindProperty(SupportsGet = true)]
    public int? VacationId { get; set; }
    [BindProperty, Required(ErrorMessage = "Er is geen naam ingevuld")]
    public string form_VacationName { get; set; }
    [BindProperty]
    public string? form_VacationDescription { get; set; }
    [BindProperty, Required(ErrorMessage = "Er is geen startdatum ingevuld")]
    public DateTime form_StartDate { get; set; }
    [BindProperty, Required(ErrorMessage = "Er is geen einddatum ingevuld")]
    public DateTime form_EndDate { get; set; }
    public AddVacationModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }
    public IActionResult OnGet()
    {
        if (VacationId == null) return Page();
        var vacation = this._DB._Provider.select("Vacation",
            new string[] { "id", "name", "description", "start", "end" },
            new Where[] {
                new Where("id", Compare.Equal, VacationId.ToString()),
                new Where("User", Compare.Equal, User.Identity.Name)
        });
        if (vacation.Count() != 1) return Page();
        this._IsEditing = true;
        this.form_VacationName = vacation[0]._columns.Find(col => col._column == "name")._value;
        this.form_VacationDescription = vacation[0]._columns.Find(col => col._column == "description")._value;
        this.form_StartDate = DateTimeOffset.FromUnixTimeSeconds((long)vacation[0]._columns.Find(col => col._column == "start")._value).DateTime;
        this.form_EndDate = DateTimeOffset.FromUnixTimeSeconds((long)vacation[0]._columns.Find(col => col._column == "end")._value).DateTime;
        return Page();
    }
    public IActionResult OnPost()
    {
        if (!this.OnPostValidation())
            return Page();
        var data = new List<Column>();
        data.Add(new Column("name", this.form_VacationName));
        data.Add(new Column("User", User.Identity.Name));
        data.Add(new Column("start", new DateTimeOffset(this.form_StartDate.ToUniversalTime()).ToUnixTimeSeconds().ToString()));
        data.Add(new Column("end", new DateTimeOffset(this.form_EndDate.ToUniversalTime()).ToUnixTimeSeconds().ToString()));
        if (this.form_VacationDescription != null)
            data.Add(new Column("description", this.form_VacationDescription));
        bool status;
        if (this.VacationId != null)
            status = this.OnPostUpdate(data);
        else
            status = this.OnPostInsert(data);

        if (!status)
            return Page();

        return RedirectToPage("/profile/Profile", null, new { Username = User.Identity.Name });
    }
    private bool OnPostInsert(List<Column> data)
    {
        return this._DB._Provider.insert("Vacation", data);
    }
    private bool OnPostUpdate(List<Column> data)
    {
        var vacation = this._DB._Provider.select("Vacation",
            new string[] { "id", "User" },
            new Where[] {
                new Where("id", Compare.Equal, VacationId.ToString()),
                new Where("User", Compare.Equal, User.Identity.Name)
        });
        if (vacation.Count() != 1)
        {
            ModelState.AddModelError("form_VacationName", "Deze vakantie bestaat niet");
            return false;
        }
        return this._DB._Provider.update("Vacation", data,
            new Where[] { new Where("id", Compare.Equal, this.VacationId.ToString()) },
        -1);
    }
    private bool OnPostValidation()
    {
        if (!ModelState.IsValid)
            return false;
        if (this.form_StartDate > this.form_EndDate)
        {
            ModelState.AddModelError("form_StartDate", "De startdatum mag niet later zijn dan de einddatum");
            return false;
        }

        return true;
    }
}