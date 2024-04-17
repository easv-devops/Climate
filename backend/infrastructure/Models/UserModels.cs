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
    public required string Name { get; set; }
}

public class UserRegisterDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Country code is required.")]
    public string CountryCode { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Phone number is not valid.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; }
}
