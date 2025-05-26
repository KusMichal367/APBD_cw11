using Microsoft.EntityFrameworkCore;
using APBD_cw11.Data;
using APBD_cw11.DTOs;
using APBD_cw11.Models;
using APBD_cw11.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace APBD_cw11.Services;

public class DbService: IDbService
{
    private readonly DatabaseContext _context;

    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<PrescriptionOutputDto>> GetPrescriptionsAsync()
    {
        var prescriptions = await _context.Prescriptions.OrderBy(e => e.DueDate).Select(e => new PrescriptionOutputDto
        {
            IdPrescription = e.IdPrescription,
            Date = e.Date,
            DueDate = e.DueDate,
            Medicaments = e.PrescriptionMedicaments.Select(a => new MedicamentDto
                {
                    IdMedicament = a.IdMedicament,
                    Name = a.Medicament.Name,
                    Dose = a.Dose ?? 0,
                    Description = a.Details

                }
            ).ToList(),
            Doctor = new DoctorDto
            {
                IdDoctor = e.Doctor.IdDoctor,
                FirstName = e.Doctor.FirstName,
            }
        }).ToListAsync();
        return prescriptions;
    }

    public async Task<PatientInfoDto> GetPatientAsync(int id)
    {
        var patient = await _context.Patients.Where(e => e.IdPatient == id).Select(a=> new PatientInfoDto
        {
            IdPatient = a.IdPatient,
            FirstName = a.FirstName,
            LastName = a.LastName,
            BirthDate = a.Birthdate,
            Prescriptions = a.Prescriptions.Select(p => new PrescriptionOutputDto
            {
                IdPrescription = p.IdPrescription,
                Date = p.Date,
                DueDate = p.DueDate,
                Medicaments = p.PrescriptionMedicaments.Select(m => new MedicamentDto
                {
                    IdMedicament = m.IdMedicament,
                    Name = m.Medicament.Name,
                    Dose = m.Dose ?? 0,
                    Description = m.Details
                }).ToList(),
                Doctor = new DoctorDto
                {
                    IdDoctor = p.Doctor.IdDoctor,
                    FirstName = p.Doctor.FirstName
                }
            }).ToList(),
        }).FirstOrDefaultAsync();

        return patient;
    }

    public async Task<IActionResult> CreatePrescriptionAsync(PrescriptionInputDto dto)
    {
        if (dto.Medicaments.Count > 10)
        {
            return new BadRequestObjectResult("Medicament must be less than 10");
        }

        if (dto.DueDate < dto.Date)
        {
            return new BadRequestObjectResult("Due date must be after the Date");
        }

        var doctor = await _context.Doctors.FindAsync(dto.IdDoctor);
        if (doctor == null)
        {
            return new BadRequestObjectResult("Doctor does not exist");
        }

        var medIds = dto.Medicaments.Select(m => m.IdMedicament).ToList();
        var existingMedicament = await _context.Medicaments
            .CountAsync(m => medIds.Contains(m.IdMedicament));
        if (existingMedicament != medIds.Count)
            return new BadRequestObjectResult("Some medicaments do not exist");

        Patient patient = await _context.Patients.FindAsync(dto.Patient.IdPatient);
        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = dto.Patient.FirstName,
                LastName  = dto.Patient.LastName,
                Birthdate = dto.Patient.BirthDate
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

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        var prescriptionMedicaments = dto.Medicaments.Select(m => new PrescriptionMedicament
        {
            IdPrescription = prescription.IdPrescription,
            IdMedicament = m.IdMedicament,
            Dose = m.Dose,
            Details = m.Description
        }).ToList();

        _context.PrescriptionMedicaments.AddRange(prescriptionMedicaments);
        await _context.SaveChangesAsync();

        return new CreatedAtActionResult(
            nameof(PrescriptionsController.GetPrescriptions),
            "Prescriptions",
            new { id = prescription.IdPrescription },
            null);

    }
}