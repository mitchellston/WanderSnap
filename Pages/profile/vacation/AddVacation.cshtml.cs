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
        this._DB._Provider.insert("Vacation", data);
        return RedirectToPage("/profile/Profile", null, new { Username = User.Identity.Name });
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