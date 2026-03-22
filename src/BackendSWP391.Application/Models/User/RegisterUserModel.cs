namespace BackendSWP391.Application.Models.User;

public class RegisterUserModel
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    /// <summary>Tên role (Admin, Manager, Franchise Store Staff, Central Kitchen Staff, Supply Coordinator)</summary>
    public string Role { get; set; }

    /// <summary>Số điện thoại Việt Nam (không bắt buộc)</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Địa chỉ, tối thiểu 10 ký tự (không bắt buộc)</summary>
    public string? Address { get; set; }
}

