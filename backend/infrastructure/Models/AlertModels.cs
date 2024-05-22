using System.ComponentModel.DataAnnotations;

namespace infrastructure.Models;

public class CreateAlertDto
{
    [Required(ErrorMessage = "Device Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Device Id is not a valid number")]
    public required int DeviceId { get; set; }
    
    [Required(ErrorMessage = "Timestamp is required.")]
    public required DateTime Timestamp { get; set; }
    
    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(200, ErrorMessage = "Description is too long (max 200 characters)")]
    public required string Description { get; set; }
    
    [Required(ErrorMessage = "IsRead boolean is required.")]
    public required bool IsRead { get; set; }
}

public class AlertDto
{
    public required int Id { get; set; }
    public required DateTime Timestamp { get; set; }
    public required bool IsRead { get; set; }
    public required string Description { get; set; }
    public required int DeviceId { get; set; }
    public required string DeviceName { get; set; }
    public required string RoomName { get; set; }
}