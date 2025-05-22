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
    public class PatientsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public PatientsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientInfoDto>> GetPatient(int id)
        {
            var patient = await _dbService.GetPatientsAsync(id);
            if (patient == null)
            {
                return NotFound($"Patient {id} not found");
            }
            return Ok(new
            {
                patient.IdPatient,
                patient.FirstName,
                patient.LastName,
                patient.BirthDate,
                patient.Prescriptions,
            });
        }
    }
}

