namespace infrastructure.Models;

public class PasswordHash
{
    public required int Id { get; set; }
    public required string Hash { get; set; }
    public required string Salt { get; set; }
    public required string Algorithm { get; set; }
}