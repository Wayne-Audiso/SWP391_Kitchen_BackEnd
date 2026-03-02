using BackendSWP391.Application.Common.Email;

namespace BackendSWP391.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage emailMessage);
}

