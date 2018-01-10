using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RunningWorks.Data;

namespace RunningWorks.Stores
{
    public class ApplicationUserStore : IApplicationUserStore
    {
        private bool _disposed;
        private readonly ApplicationDbContext _context;
        public bool AutoSaveChanges { get; set; } = true;

        public ApplicationUserStore(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task SetFirstNameAsync(ApplicationUser user, string firstName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.FirstName = firstName;
            return Task.CompletedTask;
        }

        public Task SetLastNameAsync(ApplicationUser user, string lastName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.LastName = lastName;
            return Task.CompletedTask;
        }

        public async Task<bool> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _context.Attach(user);
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            _context.Update(user);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        protected Task SaveChanges(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? _context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
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
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}
