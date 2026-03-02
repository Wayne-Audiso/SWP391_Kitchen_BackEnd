using System.Threading.Tasks;
using BackendSWP391.Application.Common.Email;
using BackendSWP391.Application.Services;

namespace BackendSWP391.Api.IntegrationTests.Helpers.Services;

public class EmailTestService : IEmailService
{
    public async Task SendEmailAsync(EmailMessage emailMessage)
    {
        await Task.Delay(100);
    }
}

