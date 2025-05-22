using Microsoft.EntityFrameworkCore;
using APBD_cw11.Data;
using APBD_cw11.DTOs;

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

    public async Task<PatientInfoDto> GetPatientsAsync(int id)
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
}