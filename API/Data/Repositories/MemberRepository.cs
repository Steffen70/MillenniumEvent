using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers.Filtration;
using API.Helpers.Filtration.Custom;
using AutoMapper.QueryableExtensions;

namespace API.Data.Repositories
{
    public class MemberRepository : BaseRepository
    {
        private UserRepository _userRepository;
        private UserRepository UserRepository => _userRepository ?? (_userRepository = _unitOfWork.GetRepo<UserRepository>());

        public async Task<MemberDto> GetMemberByIdAsync(int id)
        {
            var user = await UserRepository.GetUserByIdAsync(id);

            return _mapper.Map<MemberDto>(user);
        }

        public async Task<MemberDto> GetMemberByEmailAsync(string email)
        {
            var user = await UserRepository.GetUserByEmailAsync(email);

            return _mapper.Map<MemberDto>(user);
        }

        public async Task<FilteredList<MemberDto>> GetMembersAsync(FiltrationParams filtrationParams)
        {
            var userAdminDtos = _context.Users
                .OrderBy(u => u.LastActive)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);

            return await FilteredList<MemberDto>.CreateAsync(userAdminDtos, filtrationParams, _mapper);
        }

        public async Task<FilteredList<MemberDto, TestHeader>> GetMembersTestFilterAsync(TestParams testParams)
        {
            var userAdminDtos = _context.Users
                .OrderBy(u => u.LastActive)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);

            return await FilteredList<MemberDto, TestHeader>.CreateAsync(userAdminDtos, testParams, _mapper);
        }
    }
}