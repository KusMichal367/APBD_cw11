using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APBD_cw11.Models;
using APBD_cw11.Services;
using APBD_cw11.Data;
using APBD_cw11.DTOs;


namespace APBD_cw11.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public PrescriptionsController(DatabaseContext context, IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PrescriptionOutputDto>>> GetPrescriptions()
        {
            var prescriptions = await _dbService.GetPrescriptionsAsync();
            return Ok(prescriptions);
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromBody] PrescriptionInputDto dto)
        {
            return await _dbService.CreatePrescriptionAsync(dto);
        }

    }
}