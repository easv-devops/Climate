using System.ComponentModel.DataAnnotations;

namespace infrastructure.Models;

public class RoomModels
{
    
}

public class RoomWithId
{
    [Required(ErrorMessage = "Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Id is not a valid number")]
    public required int Id { get; set; }
    [Required(ErrorMessage = "Room Name is required")]
    [MaxLength(50, ErrorMessage = "Room Name is too long")]
    public required string RoomName { get; set; }
}

public class RoomWithNoId
{
    [Required(ErrorMessage = "Room Name is required")]
    [MaxLength(50, ErrorMessage = "Room Name is too long")]
    public required string RoomName { get; set; }
}

public class CreateRoomDto
{
    [Required(ErrorMessage = "User Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "User Id is not a valid number")]
    public required int UserId { get; set; }
    
    [Required(ErrorMessage = "Room Name is required")]
    [MaxLength(50, ErrorMessage = "Room Name is too long")]
    public required string RoomName { get; set; }
}