namespace WanderSnap.Models
{
    public class Validation
    {
        public (bool valid, string errorMessage) ValidatePassword(string password)
        {
            if (password.Length < 8)
            {
                return (false, "The password should be at least 8 characters long.");
            }
            if (password.Any(char.IsUpper) == false)
            {
                return (false, "The password should contain at least one uppercase letter.");
            }
            if (password.Any(char.IsLower) == false)
            {
                return (false, "The password should contain at least one lowercase letter.");
            }
            if (password.Any(char.IsDigit) == false)
            {
                return (false, "The password should contain at least one number.");
            }

            return (true, "");

        }
    }

}