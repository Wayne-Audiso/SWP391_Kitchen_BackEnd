namespace BackendSWP391.Application.Models.User;

public class RegisterUserModel
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    /// <summary>Tên role (Admin, Manager, Franchise Store Staff, Central Kitchen Staff, Supply Coordinator)</summary>
    public string Role { get; set; }
}

