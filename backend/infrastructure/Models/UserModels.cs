using System.ComponentModel.DataAnnotations;

namespace infrastructure.Models;


/**
 * contains all data relevant for stateService SHOULD NOT BE SEND TO CLIENT!
 * Status for ban or not 
 * admin rights if needed
 */
public class EndUser
{
    public int Id { get; set; }
    public string? Email { get; set; }

    public PasswordHash? PasswordInfo;
    //todo should we have admin and ban rights??
    public bool Isbanned { get; set; }
    //public bool Isadmin { get; set; }
}
/**
 * contains all information from a single user EXCLUDING password
 */
public class FullUserDto
{
    public required int Id { get; set; }
    [Required] public required string FullName { get; set; }
    [Required, EmailAddress] public required string Email { get; set; }
    [Required, Timestamp] public required DateTime Created { get; set; }
}

/**
 * shot user dto for public use in client app
 */
public class ShortUserDto
{
    public required int Id { get; set; }
    public required string Email { get; set; }
}

public class UserRegisterDto
{
    [Required] public required string FullName { get; set; }
    [Required] public required string Phone { get; set; }
    [Required] public required string Password { get; set; }
    [Required, EmailAddress] public required string Email { get; set; }
}