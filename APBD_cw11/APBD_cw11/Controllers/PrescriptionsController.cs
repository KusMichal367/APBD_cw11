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
        private readonly DatabaseContext _context;
        private readonly IDbService _dbService;

        public PrescriptionsController(DatabaseContext context, IDbService dbService)
        {
            _context = context;
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
            if (dto.Medicaments.Count > 10)
            {
                return BadRequest("Medicaments must be less than 10");
            }

            if (dto.DueDate < dto.Date)
            {
                return BadRequest("Due date must be after the Date");
            }

            Doctor doctor = await _context.Doctors.FindAsync(dto.IdDoctor);
            if (doctor == null)
                return BadRequest("Doctor does not exist");

            var medId = dto.Medicaments.Select(m => m.IdMedicament).ToList();
            var existingMedicament = await _context.Medicaments.CountAsync(m=>medId.Contains(m.IdMedicament));

            if (existingMedicament != medId.Count)
            {
                return BadRequest("Some medicaments do not exist");
            }


            Patient patient = await _context.Patients.FindAsync(dto.Patient.IdPatient);
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = dto.Patient.FirstName,
                    LastName = dto.Patient.LastName,
                    Birthdate = dto.Patient.BirthDate,
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            var prescription = new Prescription
            {
                Date = dto.Date,
                DueDate = dto.DueDate,
                IdDoctor = dto.IdDoctor,
                IdPatient = patient.IdPatient,
            };

            var prescriptionMedicaments = dto.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdPrescription = prescription.IdPrescription,
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Description
            }).ToList();

            _context.PrescriptionMedicaments.AddRange(prescriptionMedicaments);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrescriptions), new {id = prescription.IdPrescription}, null);
        }
        
    }
}