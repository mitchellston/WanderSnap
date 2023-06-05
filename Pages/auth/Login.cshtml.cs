using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;
using P4_Vacation_photos.Classes;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace P4_Vacation_photos.Pages;

public class LoginModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private DbHandler _DB = new DbHandler();

    private Hashing _Hashing = new Hashing();
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [BindProperty(SupportsGet = false), EmailAddress(ErrorMessage = "The email input should be a valid email adress"), Required(ErrorMessage = "Please enter your email address.")]
    public string email { get; set; }

    [BindProperty(SupportsGet = false), Required(ErrorMessage = "Please enter your password.")]
    public string password { get; set; }
    public LoginModel(ILogger<IndexModel> logger)
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
        // Get the user from the database and check if it exists
        var user = this._DB._Provider.select("User",
        new String[] { "email", "password", "id", "username" },
        new Models.DB.Primitives.Where[] {
            new Models.DB.Primitives.Where("email", Models.DB.Primitives.Compare.Equal, email)
        });
        if (user.Count() != 1)
        {
            ModelState.AddModelError("email", "The email address or password is incorrect.");
            return Page();
        }
        // Check if the password is correct
        string passwordHash = user[0]._columns.Find(col => col._column == "password")._value;
        if (passwordHash == null || passwordHash.GetType() != typeof(string) || this._Hashing.Verify(password, passwordHash) == false)
        {
            ModelState.AddModelError("email", "The email address or password is incorrect.");
            return Page();
        }
        // Create the claims
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, ((long)user[0]._columns.Find(col => col._column == "id")._value).ToString()));
        claims.Add(new Claim(ClaimTypes.GivenName, user[0]._columns.Find(col => col._column == "username")._value));
        claims.Add(new Claim(ClaimTypes.Email, email));
        claims.Add(new Claim(ClaimTypes.Role, "User"));
        Console.WriteLine("User logged in: " + user[0]._columns.Find(col => col._column == "username")._value);
        // Create the identity
        ClaimsIdentity identity = new ClaimsIdentity(claims, "login");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        // Sign in the user
        HttpContext.SignInAsync(principal);

        return Redirect(this.ReturnUrl ?? "/Index");
    }
}

