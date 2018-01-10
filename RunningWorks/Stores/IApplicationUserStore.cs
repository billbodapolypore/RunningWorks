using Microsoft.AspNetCore.Identity;
using RunningWorks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RunningWorks.Stores
{
    public interface IApplicationUserStore : IDisposable
    {
        Task SetFirstNameAsync(ApplicationUser user, string firstName, CancellationToken cancellationToken);
        Task SetLastNameAsync(ApplicationUser user, string lastName, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default(CancellationToken));
    }
}
