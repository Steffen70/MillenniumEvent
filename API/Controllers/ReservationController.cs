using System;
using System.Collections;
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
    public class ReservationController : BaseApiController
    {
        public ReservationController(DataContext context, IMapper mapper) : base(context, mapper)
        {
        }

        [Authorize(Policy = "RequireEmployeeRole")]
        [HttpPost("Create")]
        public ActionResult<string> Create(ReservationCreateDto reservationCreateDto)
        {
            //Add Validation

            if (reservationCreateDto == null || reservationCreateDto.EndDate < reservationCreateDto.StartDate)
                return BadRequest();

            var reservation = Mapper.Map<Reservation>(reservationCreateDto);

            //Replace with alternative that runs on sql server
            if (Context.Reservations.ToList().Any(r =>
                    r.BikeId == reservation.BikeId &&
                    ((r.StartDate <= reservation.StartDate && r.EndDate >= reservation.StartDate) ||
                     (r.StartDate <= reservation.EndDate && r.EndDate >= reservation.EndDate))))
                return StatusCode(208);

            Context.Reservations.Add(reservation);

            Context.SaveChanges();

            var appUser = Context.Users.Single(u => u.Id == reservation.AppUserId);

            return Ok(appUser.Username);
        }

        [Authorize(Policy = "RequireEmployeeRole")]
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ReservationListDto>>> List()
        {
            if (!Context.Reservations.Any())
                return NoContent();

            var list = await Context.Reservations.OrderBy(r => r.StartDate).Include(r => r.AppUser).Include(r => r.Bike).ToListAsync();
            var reservationListDtos = list.Select(r =>
            {
                var reservationListDto = Mapper.Map<ReservationListDto>(r);
                reservationListDto.Bike = r.Bike.ToString();
                reservationListDto.ReservationName = r.AppUser.Username;
                return reservationListDto;
            });

            return Ok(reservationListDtos);
        }

        [Authorize(Policy = "RequireEmployeeRole")]
        [HttpGet("Options")]
        public async Task<ActionResult<IEnumerable>> Options()
        {
            if (!Context.Users.Any() || !Context.Bikes.Any())
                return NoContent();

            var bikes = await Context.Bikes.OrderBy(b => b.Brand + b.Model + b.Year).ToListAsync();
            var users = await Context.Users.OrderBy(u => u.UserRole).ToListAsync();
            var result = new
            {
                users = users.Select(u => new { u.Id, u.Username }),
                bikes = bikes.Select(b => new { b.Id, ToString = b.ToString() })
            };

            return Ok(result);
        }
    }
}