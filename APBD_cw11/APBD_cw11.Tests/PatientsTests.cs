using Xunit;
using APBD_cw11.Controllers;
using APBD_cw11.Models;
using APBD_cw11.Data;
using APBD_cw11.DTOs;
using APBD_cw11.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD_cw11.Tests
{
    public class PatientsTests
    {
        private readonly DatabaseContext _context;
        private readonly IDbService _dbService;
        private readonly PatientsController _patientsController;

        public PatientsTests()
        {
            var databaseForTests =
                new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("TestDatabase").Options;

            _context = new DatabaseContext(databaseForTests);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Doctors.Add(new Doctor
            {
                IdDoctor = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jkowalski@gmail.com"
            });
            _context.Medicaments.Add(new Medicament
            {
                IdMedicament = 1, Name = "Apap", Description = "opis", Type = "przeciwbólowy"
            });
            _context.Patients.Add(new Patient
            {
                IdPatient = 1, FirstName = "Filip", LastName = "Testowy", Birthdate = DateTime.Now.AddYears(-22)
            });
            _context.Prescriptions.Add(new Prescription
            {
                IdPrescription = 1, Date = DateTime.Today, DueDate = DateTime.Today.AddMonths(1), IdPatient = 1, IdDoctor = 1
            });
            _context.PrescriptionMedicaments.Add(new PrescriptionMedicament
            {
                IdPrescription = 1, IdMedicament = 1, Dose = 1, Details = "jedną codziennie wieczorem"
            });

            _context.SaveChanges();


            _dbService = new DbService(_context);
            _patientsController = new PatientsController(_dbService);
        }


        [Fact]
        public async Task GetPatientsInfo()
        {
            var actionResult = await _patientsController.GetPatient(1);

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var patientInfoDto = Assert.IsType<PatientInfoDto>(okObjectResult.Value);

            Assert.Equal(1, patientInfoDto.IdPatient);
            Assert.Equal("Filip", patientInfoDto.FirstName);
            Assert.Equal(1, patientInfoDto.Prescriptions[0].IdPrescription);
            Assert.Equal("Jan", patientInfoDto.Prescriptions[0].Doctor.FirstName);
        }
    }
}


