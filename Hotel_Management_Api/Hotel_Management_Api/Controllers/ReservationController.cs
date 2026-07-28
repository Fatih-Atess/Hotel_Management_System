using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Models;
using Hotel_Management_Api.DTOs;


namespace Hotel_Management_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
        {
            if (request.Cikis_Tarihi <= request.Giris_Tarihi)
            {
                return BadRequest("Hata: Çıkış tarihi, giriş tarihinden sonra olmalıdır.");

            }
            if (request.Giris_Tarihi.Date < DateTime.UtcNow.Date)
            {
                return BadRequest("Hata: Geçmiş bir tarihe rezervasyon yapılamaz.");
            }

            bool isSuccess = await _reservationRepository.CreateReservationAsync(request);

            if (!isSuccess)
            {
                return Conflict("Seçili tarihler arasında oda zaten rezerve edilmiş veya oda bulunamadı. Lütfen farklı tarihler seçin.");
            }

            return Ok(new { message = "Rezervasyon başarıyla oluşturuldu", isSuccess = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation([FromRoute]int id)
        {
            if(await _reservationRepository.DeleteReservationAsync(id) == false)
            {
                return NotFound("Böyle bir rezervasyon bulunamadı.");
            }
           
            return Ok(new { message = "Rezervasyon silindi", isSuccess = true });
        }

        [HttpGet]
        public async Task<IActionResult> GelAllReservations()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            return Ok(reservations);
        }
        
    }
}
