namespace BackendSWP391.Application.Models.User;

public class UpdateUserModel
{
    public string UserName { get; set; }

    public string Email { get; set; }
}

public class UpdateUserResponseModel : BaseResponseModel { }

