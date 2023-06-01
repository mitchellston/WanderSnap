using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;
using P4_Vacation_photos.Classes;


namespace P4_Vacation_photos.Pages;

public class RegisterModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private DbHandler _DB = new DbHandler();

    private Hashing _Hashing = new Hashing();

    [BindProperty(SupportsGet = false), EmailAddress(ErrorMessage = "The email input should be a valid email adress"), Required(ErrorMessage = "Please enter your email address.")]
    public string email { get; set; }

    [BindProperty(SupportsGet = false), Required(ErrorMessage = "Please enter a username.")]
    public string username { get; set; }

    [BindProperty(SupportsGet = false), Required(ErrorMessage = "Please enter your password.")]
    public string password { get; set; }

    [BindProperty(SupportsGet = false), Required(ErrorMessage = "Please verify your password.")]
    public string verifyPassword { get; set; }
    public RegisterModel(ILogger<IndexModel> logger)
    {
        _logger = logger;

    }
    public IActionResult OnPost()
    {
        // Check if the model is valid
        if (ModelState.IsValid == false)
        {
            return Page();
        }
        var passwordValidation = new Validation().ValidatePassword(password);
        if (passwordValidation.valid == false)
        {
            ModelState.AddModelError("password", passwordValidation.errorMessage);
            return Page();
        }
        if (password != verifyPassword)
        {
            ModelState.AddModelError("verifyPassword", "The passwords do not match.");
            return Page();
        }

        // Check if the email address is already in use
        var user = this._DB._Provider.count("User", new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("email", Models.DB.Primitives.Compare.Equal, email)
        });
        if (user != 0)
        {
            ModelState.AddModelError("email", "The email address is already in use.");
            return Page();
        }

        // Check if the username is already in use
        user = this._DB._Provider.count("User", new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("username", Models.DB.Primitives.Compare.Equal, username)
        });
        if (user != 0)
        {
            ModelState.AddModelError("username", "The username is already in use.");
            return Page();
        }

        // Create the user
        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
        var hashedPassword = this._Hashing.Hash(password);
        this._DB._Provider.insert("User", new Models.DB.Primitives.Column[] {
            new Models.DB.Primitives.Column("email", email),
            new Models.DB.Primitives.Column("username", username),
            new Models.DB.Primitives.Column("password", hashedPassword),
            new Models.DB.Primitives.Column("created_at", now.ToUnixTimeSeconds())
        }.ToList());
        return RedirectToPage("/Login");
    }
}

