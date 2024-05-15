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
    [Required(ErrorMessage = "User Id is required")] 
    [Range(0, int.MaxValue, ErrorMessage = "Id is not a valid number")]
    public required int Id { get; set; }
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    public required string Email { get; set; }
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100, ErrorMessage = "Name is too long. Max 100 characters")]
    public required string FirstName { get; set; }
    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100, ErrorMessage = "Name is too long. Max 100 characters")]
    public required string LastName { get; set; }
    [Required(ErrorMessage = "Country code is required.")]
    public required string CountryCode { get; set; }
    [Required(ErrorMessage = "Phone number is required.")]
    [MaxLength(20, ErrorMessage = "Phone number is too long. Max 20 characters")]
    [RegularExpression(@"^[0-9() \-]*$", ErrorMessage = "Invalid Phone Number.")] // Accepts digits, parenthesis, dashes and spaces
    public required string Number { get; set; }
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
