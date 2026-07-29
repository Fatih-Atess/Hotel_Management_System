using Hotel_Management_Api.DTOs;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Models;
using Hotel_Management_Api.Repositories;
using Microsoft.AspNetCore.Mvc;

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
            if(room.Oda_Numarasi == "")
            {
                return BadRequest("Lütfen bir oda numarası giriniz");
            }
            if(room.Tip == "")
            {
                return BadRequest("Lütfen bir oda tipi giriniz");
            }

            bool isSuccess = await _roomRepository.AddRoomAsync(room);
            if (isSuccess == false)
            {
                return Conflict("Bu oda numarasına sahip başka bir oda mevcut. Lütfen başka bir oda numarası giriniz");
            }
            return Ok(new { message = "Oda başarıyla eklendi", isSuccess = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom([FromRoute] int id)
        {
            if (await _roomRepository.GetRoomStatusAsync(id) == 1)
            {
                return BadRequest("Rezerve edilmiş bir odayı silemezsiniz");
            }
            if (await _roomRepository.RemoveRoomAsync(id) == false)
            {
                return NotFound("Bu id'ye sahip oda bulunamadı.");
            }
            

            return Ok(new { message = "Oda başarıyla silindi.", isSuccess = true });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom([FromRoute] int id, [FromBody] RoomDto updatedRoom)
        {
            if (updatedRoom.Gecelik_Fiyat < 1)
            {
                return BadRequest("Lütfen geçerli bir fiyat giriniz");
            }
            if (updatedRoom.Oda_Numarasi == "")
            {
                return BadRequest("Lütfen bir oda numarası giriniz");
            }
            if (updatedRoom.Tip == "")
            {
                return BadRequest("Lütfen bir oda tipi giriniz");
            }
            if(await _roomRepository.GetRoomStatusAsync(id) == 1)
            {
                return BadRequest("Rezerve edilmiş bir oda güncellenemez");
            }
            if (await _roomRepository.UpdateRoomAsync(id, updatedRoom) == false)
            {
                return NotFound("Bu id'ye sahip oda bulunamadı");
            }
            return Ok(new { message = "Oda başarıyla güncellendi", isSuccess = true });
        }

    }
}
