namespace BackendSWP391.Application.Models.Validators.User;

public static class UserValidatorConfiguration
{
    public const int MinimumUsernameLength = 5;

    public const int MaximumUsernameLength = 20;

    public const int MinimumPasswordLength = 6;

    public const int MaximumPasswordLength = 128;

    public const int MinimumAddressLength = 10;

    /// <summary>
    /// Regex khớp số điện thoại Việt Nam:
    /// 03x, 05x, 07x, 08x, 09x (10 chữ số) và +84 tương ứng
    /// </summary>
    public const string VietnamPhonePattern =
        @"^(0|\+84)(3[2-9]|5[25689]|7[06789]|8[0-9]|9[0-9])[0-9]{7}$";
}

