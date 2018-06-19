using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using IMOMaritimeSingleWindow.Helpers;
using IMOMaritimeSingleWindow.Identity.Models;
using IMOMaritimeSingleWindow.Identity.Stores;
using System.Security.Claims;

namespace IMOMaritimeSingleWindow.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store,

            IOptions<IdentityOptions> optionsAccessor,

            IPasswordHasher<ApplicationUser> passwordHasher,

            IEnumerable<IUserValidator<ApplicationUser>> userValidators,

            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,

            ILookupNormalizer keyNormalizer,

            IdentityErrorDescriber errors,

            IServiceProvider services,

            ILogger<UserManager<ApplicationUser>> logger)

            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)

        {
            //this.UserValidators.Clear();
            //this.UserValidators.Add(new CustomUserValidator<ApplicationUser>());
            //this.Options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.";
        }

        #region UserManager<TUser>
        
        public override async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userRoleStore = GetUserRoleStore();
            
            var normalizedRoleName = NormalizeKey(role);
            
            if (await userRoleStore.IsInRoleAsync(user, normalizedRoleName, CancellationToken))
            {
                return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(normalizedRoleName));
            }
            await userRoleStore.AddToRoleAsync(user, normalizedRoleName, CancellationToken);

            var userStore = GetUserStore();
            
            await userStore.SetNormalizedRoleNameAsync(user, role);

            //await base.AddToRoleAsync(user, role);
            return await userStore.UpdateRoleAsync(user);
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            return base.CreateAsync(user);
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            user.EmailConfirmed = true;
            return base.CreateAsync(user, password);
        }

        #endregion // UserManager<TUser>

        #region Custom methods

        private IUserRoleStore<ApplicationUser> GetUserRoleStore()
        {
            if (!(Store is IUserRoleStore<ApplicationUser> cast))
            {
                throw new NotSupportedException();
            }
            return cast;
        }

        private UserStore GetUserStore()
        {
            if (!(Store is UserStore cast))
            {
                throw new NotSupportedException();
            }
            return cast;
        }

        public async Task<string> GetRoleNameAsync(ApplicationUser user)
        {
            var userRoleStore = GetUserStore();
            return await userRoleStore.GetRoleNameAsync(user);
        }
        
        #endregion

    }
}
