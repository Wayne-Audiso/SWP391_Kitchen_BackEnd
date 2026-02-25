using BackendSWP391.Application.Models;
using BackendSWP391.Application.Models.User;

namespace BackendSWP391.Application.Services;

public interface IUserService
{
    Task<BaseResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel);

    Task<ConfirmEmailResponseModel> ConfirmEmailAsync(ConfirmEmailModel confirmEmailModel);

    Task<CreateUserResponseModel> CreateAsync(CreateUserModel createUserModel);

    Task<LoginResponseModel> LoginAsync(LoginUserModel loginUserModel);

    Task<IEnumerable<UserResponseModel>> GetAllAsync();

    Task<UserResponseModel> GetByIdAsync(Guid id);

    Task<UpdateUserResponseModel> UpdateAsync(Guid id, UpdateUserModel updateUserModel); 

    Task<bool> DeleteAsync(Guid id);
}

