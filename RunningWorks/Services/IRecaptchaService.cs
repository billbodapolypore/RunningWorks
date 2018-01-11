using System.Threading.Tasks;

namespace RunningWorks.Services
{
    public interface IRecaptchaService
    {
        Task<bool> VerifyResponse(string response);
    }
}
