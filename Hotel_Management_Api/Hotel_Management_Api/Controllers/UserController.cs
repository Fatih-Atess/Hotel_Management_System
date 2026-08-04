using Hotel_Management_Api.DTOs;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Models;
using Hotel_Management_Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController :ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync([FromBody] UserDto user)
        {
            if(user.Kullanici_Adi == null || user.Kullanici_Adi == "" || user.Sifre == null || user.Sifre == "" || user.Kullanici_Adi.Trim() == "")
            {
                return BadRequest("Lütfen geçerli bir kullanıcı adı ve şifre giriniz");
            }
            bool isSuccess = await _userRepository.AddUserAsync(user);
            if (isSuccess == false)
            {
                return Conflict("Bu kullanıcı adına sahip başka bir kullanıcı mevcut. Lütfen başka bir kullanıcı adı giriniz");
            }
            return Ok(new { message = "Kullanıcı başarıyla eklendi", isSuccess = true });
        }
    }
}
