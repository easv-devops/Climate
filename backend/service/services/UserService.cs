using infrastructure;
using infrastructure.Models;

namespace service.services;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public FullUserDto GetFullUserById(int userId)
    {
        return _userRepository.GetFullUserById(userId);
    }

    public FullUserDto EditUser(FullUserDto userDto)
    {
        return _userRepository.EditUser(userDto);
    }
}