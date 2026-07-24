using Microsoft.AspNetCore.Mvc;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Repositories;

namespace Hotel_Management_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            if (checkIn.Date < DateTime.Today)
            {
                return BadRequest("Rezervasyon tarihi geçmiş bir tarih olamaz.");
            }

            if (checkOut <= checkIn)
            {
                return BadRequest("Çıkış tarihi, giriş tarihinden sonra olmalıdır.");
            }

            var availableRooms = await _roomRepository.GetAvailableRoomsAsync(checkIn, checkOut);

            return Ok(availableRooms);
        }

    }
}
