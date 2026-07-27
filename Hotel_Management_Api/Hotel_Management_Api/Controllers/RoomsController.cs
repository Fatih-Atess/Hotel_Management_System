using Microsoft.AspNetCore.Mvc;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Repositories;
using Hotel_Management_Api.DTOs;

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

        [HttpGet("available")]
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

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomRepository.GetAllRoomsAsync();
            return Ok(rooms);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom([FromBody] RoomDto room)
        {
            if(room.Gecelik_Fiyat < 1)
            {
                return BadRequest("Lütfen geçerli bir fiyat giriniz");
            }

            bool isSuccess = await _roomRepository.AddRoomAsync(room);
            if (isSuccess)
            {
                return Conflict("Bu oda numarasına sahip başka bir oda mevcut. Lütfen başka bir oda numarası giriniz");
            }
            return Ok("Oda başarıyla eklendi");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            if(await _roomRepository.RemoveRoomAsync(id) == false)
            {
                return BadRequest("Bu id'ye sahip oda bulunamadı.");
            }

            return Ok("Oda başarıyla silindi.");
        }

    }
}
