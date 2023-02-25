using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.Data.Repositories;
using API.DTOs;
using API.Extensions;
using API.Helpers.Filtration;
using API.Helpers.Filtration.Custom;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MembersController : BaseApiController
    {
        private readonly MemberRepository _memberRepository;
        public MembersController(UnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _memberRepository = unitOfWork.GetRepo<MemberRepository>();
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers([FromQuery] FiltrationParams filtrationParams)
        {
            var membersList = await _memberRepository.GetMembersAsync(filtrationParams);

            Response.AddFiltrationHeader(membersList);

            return membersList.Result;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("test-filter")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetTestFilter([FromQuery] TestParams filtrationParams)
        {
            var membersList = await _memberRepository.GetMembersTestFilterAsync(filtrationParams);

            Response.AddFiltrationHeader(membersList);

            return membersList.Result;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDto>> GetMemberById([FromRoute] int id)
        => await _memberRepository.GetMemberByIdAsync(id);

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{email}")]
        public async Task<ActionResult<MemberDto>> GetMemberByEmail([FromRoute] string email)
        => await _memberRepository.GetMemberByEmailAsync(email);
    }
}