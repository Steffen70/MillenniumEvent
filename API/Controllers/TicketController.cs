using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace API.Controllers
{
    [AllowAnonymous]
    public class TicketController : BaseApiController
    {
        private readonly IOptions<ApiSettings> _apiSettings;
        private readonly EmailService _service;

        public TicketController(DataContext context, IMapper mapper, IOptions<ApiSettings> apiSettings,
            EmailService service) : base(context, mapper)
        {
            _apiSettings = apiSettings;
            _service = service;
        }

        [Authorize(Policy = "RequirePromoterRole")]
        [HttpPost("Send")]
        public async Task<ActionResult> Send([FromQuery] string email)
        {
            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!regex.Match(email).Success)
                return BadRequest();

            // if (Context.Tickets.Any(t => t.Email == email))
            //     return StatusCode(208);

            var ticket = new Ticket(User.GetUserId(), email);

            Context.Tickets.Add(ticket);
            Context.SaveChanges();

            try
            {
                var logicTicket = Mapper.Map<Logic.Ticket>(ticket);

                await using var fs = System.IO.File.OpenRead(Path.Combine("Data", _apiSettings.Value.FlyerImageName));
                using var flyer = SKBitmap.Decode(fs);

                var image = logicTicket.GenerateTicketBitmap(flyer);
                logicTicket.SendViaMail(_service, image);
            }
            catch
            {
                Context.Tickets.Remove(ticket);
                await Context.SaveChangesAsync();

                throw;
            }

            return StatusCode(201);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("List")]
        public async Task<ActionResult<IQueryable<TicketDto>>> List()
        {
            var list = await Context.Tickets
                .GroupBy(t => new
                {
                    Ticket = t.Email,
                    Promoter = t.AppUser.Email
                })
                .Select(t => new TicketDto
                {
                    TicketEmail = t.Key.Ticket,
                    PromoterEmail = t.Key.Promoter,
                    TicketCount = t.Count()
                }).ToListAsync();
            return Ok(list);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("Redeem")]
        public ActionResult<string> Redeem([FromQuery] Guid ticketKey)
        {
            var ticket = Context.Tickets.FirstOrDefault(t => t.TicketKey == ticketKey);

            if (ticket == null) return NotFound();

            if (ticket.Redeemed != null) return StatusCode(208); //ticket is redeemed

            ticket.Redeemed = DateTime.UtcNow;

            Context.SaveChanges();

            return Ok(ticket.Email);
        }
    }
}