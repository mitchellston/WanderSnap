namespace WanderSnap.Models;
public class ProfilePostEditProfile
{
    public string? username { get; set; }
    public string? description { get; set; }
    public IFormFile? profilePicture { get; set; }
}