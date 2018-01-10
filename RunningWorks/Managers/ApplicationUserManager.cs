using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RunningWorks.Data;
using RunningWorks.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RunningWorks.Managers
{
    public class ApplicationUserManager : IDisposable, IApplicationUserManager
    {
        private bool _disposed;
        protected internal IApplicationUserStore Store { get; set; }
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ApplicationUserManager(IApplicationUserStore store)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<bool> SetFirstNameAsync(ApplicationUser user, string firstName)
        {
            ThrowIfDisposed();
            var store = GetApplicationUserStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await store.SetFirstNameAsync(user, firstName, CancellationToken);
            return await Store.UpdateAsync(user, CancellationToken);
        }

        public async Task<bool> SetLastNameAsync(ApplicationUser user, string lastName)
        {
            ThrowIfDisposed();
            var store = GetApplicationUserStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await store.SetLastNameAsync(user, lastName, CancellationToken);
            return await Store.UpdateAsync(user, CancellationToken);
        }

        private IApplicationUserStore GetApplicationUserStore()
        {
            var cast = Store as IApplicationUserStore;
            if (cast == null)
            {
                throw new NotSupportedException("Store does not implement IApplicationUserStore.");
            }
            return cast;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Store.Dispose();
                _disposed = true;
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
