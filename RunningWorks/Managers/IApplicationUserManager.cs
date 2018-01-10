using System;
using System.Threading.Tasks;
using RunningWorks.Data;

namespace RunningWorks.Managers
{
    public interface IApplicationUserManager : IDisposable
    {
        Task<bool> SetFirstNameAsync(ApplicationUser user, string firstName);
        Task<bool> SetLastNameAsync(ApplicationUser user, string lastName);
    }
}