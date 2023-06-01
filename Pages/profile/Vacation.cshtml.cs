using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P4_Vacation_photos.Pages;

public class VacationsModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    [BindProperty(SupportsGet = true)]
    public string User { get; set; }
    [BindProperty(SupportsGet = true)]
    public int VacationId { get; set; }
    public VacationsModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {  
        

    }
}

