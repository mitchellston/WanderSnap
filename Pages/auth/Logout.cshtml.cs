using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;
using P4_Vacation_photos.Classes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace P4_Vacation_photos.Pages;
[Authorize]
public class LogoutModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private DbHandler _DB = new DbHandler();

    private Hashing _Hashing = new Hashing();

    [BindProperty(SupportsGet = false), EmailAddress(ErrorMessage = "The email input should be a valid email adress"), Required(ErrorMessage = "Please enter your email address.")]
    public string email { get; set; }

    [BindProperty(SupportsGet = false), Required(ErrorMessage = "Please enter your password.")]
    public string password { get; set; }
    public LogoutModel(ILogger<IndexModel> logger)
    {
        _logger = logger;

    }
    public IActionResult OnGet()
    {
        if (User.Identity.IsAuthenticated)
        {
            HttpContext.SignOutAsync();
            Console.WriteLine("Logged out");
            return Page();
        }
        return RedirectToPage("/Index");
    }
}

