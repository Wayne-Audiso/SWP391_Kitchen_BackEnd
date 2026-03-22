namespace BackendSWP391.Application.Models.User;

public class CreateUserModel
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string Role { get; set; }

    /// <summary>Số điện thoại Việt Nam (không bắt buộc)</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Địa chỉ, tối thiểu 10 ký tự (không bắt buộc)</summary>
    public string? Address { get; set; }
}

public class CreateUserResponseModel : BaseResponseModel { }

