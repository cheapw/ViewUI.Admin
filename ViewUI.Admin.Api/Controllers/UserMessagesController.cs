using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewUI.Admin.Api.Entities;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace ViewUI.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserMessagesController : ControllerBase
    {
        private readonly AdminApiContext _context;

        public UserMessagesController(AdminApiContext context)
        {
            _context = context;
        }

        // GET: api/UserMessages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserMessage>>> GetUserMessages()
        {
            return await _context.UserMessages.ToListAsync();
        }

        // GET: api/UserMessages/count
        [HttpGet("unreadcount")]
        public async Task<ActionResult<object>> GetUserMessagesCount()
        {
            var messages = await _context.UserMessages.Where(m => !m.IsRead && !m.IsDelete).ToArrayAsync();
            return new { unread_count = messages.Count() };
        }

        // GET: api/UserMessages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserMessage>> GetUserMessage(Guid id)
        {
            var userMessage = await _context.UserMessages.FindAsync(id);

            if (userMessage == null)
            {
                return NotFound();
            }

            return userMessage;
        }

        // GET: api/UserMessages/content/5
        [HttpGet("content")]
        public async Task<ActionResult<string>> GetMessageContent([FromQuery] Guid id)
        {
            var userMessage = await _context.UserMessages.FindAsync(id);

            if (userMessage == null)
            {
                return NotFound();
            }

            return userMessage.Content;
        }

        // POST: api/UserMessages/has_read
        [HttpPost("has_read")]
        public async Task<ActionResult> HasReadMessage([FromForm] Guid id)
        {
            var userMessage = await _context.UserMessages.FindAsync(id);
            userMessage.IsRead = true;
            _context.Entry(userMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserMessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserMessages/remove_readed
        [HttpPost("remove_readed")]
        public async Task<ActionResult> RemoveReadMessage([FromForm] Guid id)
        {
            var userMessage = await _context.UserMessages.FindAsync(id);
            userMessage.IsDelete = true;
            _context.Entry(userMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserMessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserMessages/restore
        [HttpPost("restore")]
        public async Task<ActionResult> RestoreMessage([FromForm] Guid id)
        {
            var userMessage = await _context.UserMessages.FindAsync(id);
            userMessage.IsDelete = false;
            _context.Entry(userMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserMessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/UserMessages/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserMessage(Guid id, UserMessage userMessage)
        {
            if (id != userMessage.Id)
            {
                return BadRequest();
            }

            _context.Entry(userMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserMessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserMessages
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserMessage>> PostUserMessage(UserMessage userMessage)
        {
            _context.UserMessages.Add(userMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserMessage", new { id = userMessage.Id }, userMessage);
        }

        // DELETE: api/UserMessages/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserMessage>> DeleteUserMessage(Guid id)
        {
            var userMessage = await _context.UserMessages.FindAsync(id);
            if (userMessage == null)
            {
                return NotFound();
            }

            _context.UserMessages.Remove(userMessage);
            await _context.SaveChangesAsync();

            return userMessage;
        }

        private bool UserMessageExists(Guid id)
        {
            return _context.UserMessages.Any(e => e.Id == id);
        }
    }
}
