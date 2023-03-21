using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.DTOs;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using API.Extensions;

namespace API.Controllers
{
    [AllowAnonymous]
    public class BikeController : BaseApiController
    {
        public BikeController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [Authorize(Policy = "RequireEmployeeRole")]
        [HttpPost("Create")]
        public ActionResult<string> Create(BikeCreateDto bikeCreateDto)
        {
            //Add Validation

            var bike = Mapper.Map<Bike>(bikeCreateDto);

            if (Context.Bikes.Any(b => b.Brand == bike.Brand && b.Model == bike.Model && b.Year == bike.Year))
                return StatusCode(208);

            Context.Bikes.Add(bike);

            Context.SaveChanges();

            return Ok(bike.ToString());
        }

        [Authorize(Policy = "RequireEmployeeRole")]
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<BikeDto>>> List()
        {
            if (!Context.Bikes.Any())
                return NoContent();

            var list = await Context.Bikes.OrderBy(b => b.Brand + b.Model + b.Year).ToListAsync();
            var bikeDtoList = list.Select(b =>
            {
                var bikeDto = Mapper.Map<BikeDto>(b);
                bikeDto.Category = b.Category.GetDescription();
                return bikeDto;
            });

            return Ok(bikeDtoList);
        }
    }
}