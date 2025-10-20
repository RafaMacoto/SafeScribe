using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SafeScribe.Data;
using SafeScribe.DTOs;
using SafeScribe.Models;

namespace SafeScribe.Controllers
{
    [ApiController]
    [Route("api/v1/notas")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public NotesController(ApplicationDbContext db) => _db = db;

        private Guid GetUserIdFromClaims()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<IActionResult> CreateNote([FromBody] NoteCreateDto dto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty) return Unauthorized();

            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = userId
            };

            _db.Notes.Add(note);
            await _db.SaveChangesAsync();

            var noteDto = new NoteDto(note.Id, note.Title, note.Content, note.CreatedAt, note.UserId);
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, noteDto);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetNote(Guid id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();

            var userId = GetUserIdFromClaims();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == Role.Reader || userRole == Role.Editor)
            {
                if (note.UserId != userId) return Forbid();
            }

            var noteDto = new NoteDto(note.Id, note.Title, note.Content, note.CreatedAt, note.UserId);
            return Ok(noteDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateNote(Guid id, [FromBody] NoteCreateDto dto)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();

            var userId = GetUserIdFromClaims();
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == Role.Editor)
            {
                if (note.UserId != userId) return Forbid();
            }

            note.Title = dto.Title;
            note.Content = dto.Content;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var note = await _db.Notes.FindAsync(id);
            if (note == null) return NotFound();

            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
