namespace SafeScribe.DTOs
{
    public record NoteDto(System.Guid Id, string Title, string? Content, System.DateTime CreatedAt, System.Guid UserId);
}
