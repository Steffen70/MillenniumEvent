using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Entities;
using API.Helpers.Filtration;
using AutoMapper.QueryableExtensions;

namespace API.Data.Repositories
{
    public class UserRepository : BaseRepository
    {
        public async Task<bool> UserExistsAsync(string email)
        {
            return await Context.Users.AnyAsync(u => u.Email == email.ToLower());
        }

        public void AddUser(AppUser user)
        {
            Context.Users.Add(user);
        }

        public void UpdateUser(AppUser user)
        {
            Context.Entry(user).State = EntityState.Modified;
        }

        public void DeleteUser(AppUser user)
        {
            Context.Users.Remove(user);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await Context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            return await Context.Users
                .SingleOrDefaultAsync(x => x.Email == email.ToLower());
        }

        public async Task<FilteredList<UserAdminDto>> GetUsersAsync(FiltrationParams filtrationParams)
        {
            var userAdminDtos = Context.Users
                .Where(u => u.Created <= filtrationParams.TimeStamp)
                .OrderBy(u => u.LastActive)
                .ProjectTo<UserAdminDto>(Mapper.ConfigurationProvider);

            return await FilteredList<UserAdminDto>.CreateAsync(userAdminDtos, filtrationParams, Mapper);
        }

        public async Task<bool> AnyUsersAsync()
        => await Context.Users.AnyAsync();
    }
}