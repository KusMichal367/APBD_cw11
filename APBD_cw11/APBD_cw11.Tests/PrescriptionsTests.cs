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
    public class PrescriptionsTests
    {
        private readonly DatabaseContext _context;
        private readonly IDbService _dbService;
        private readonly PrescriptionsController _prescriptionsController;

        public PrescriptionsTests()
        {
            var databaseForTests =
                new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("TestDatabase").Options;

            _context = new DatabaseContext(databaseForTests);

            _context.Doctors.Add(new Doctor{ IdDoctor = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jkowalski@gmail.com" });
            _context.Medicaments.Add(new Medicament
                { IdMedicament = 1, Name = "Apap", Description = "opis", Type = "przeciwbólowy" });
            _context.SaveChanges();

            _dbService = new DbService(_context);
            _prescriptionsController = new PrescriptionsController(_context, _dbService);
        }


        [Fact]
        public async Task CreatePrescriptionAndPatient()
        {
            var dto = new PrescriptionInputDto
            {
                Patient = new PatientDto
                    { FirstName = "Jakub", LastName = "Malinowski", BirthDate = DateTime.Today.AddYears(-20) },
                IdDoctor = 1,
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddMonths(1),
                Medicaments = new List<MedicamentToPrescriptionDto>
                {
                    new MedicamentToPrescriptionDto { IdMedicament = 1, Dose = 1, Description = "codziennie rano" }
                }
            };

            var result = await _prescriptionsController.AddPrescription(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, await _context.Prescriptions.CountAsync());
            Assert.Equal(1, await _context.Patients.CountAsync());
        }

        [Fact]
        public async Task MedicamentNotExists()
        {
            var dto = new PrescriptionInputDto
            {
                Patient = new PatientDto
                    { FirstName = "Andrzej", LastName = "Śliwa", BirthDate = DateTime.Today.AddYears(-40) },
                IdDoctor = 1,
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddMonths(1),
                Medicaments = new List<MedicamentToPrescriptionDto>
                {
                    new MedicamentToPrescriptionDto { IdMedicament = 100, Dose = 1, Description = "codziennie rano" }
                }
            };

            var result = await _prescriptionsController.AddPrescription(dto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task TooManyMedicaments()
        {
            var medicaments = new List<MedicamentToPrescriptionDto>();
            for (int i = 0; i < 11; i++)
            {
                medicaments.Add(new MedicamentToPrescriptionDto{IdMedicament = 1, Dose = 1, Description = $"test {i}"});
            }

            var dto = new PrescriptionInputDto
            {
                Patient = new PatientDto
                    { FirstName = "Krzyś", LastName = "Miś", BirthDate = DateTime.Today.AddYears(-9) },
                IdDoctor = 1,
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddMonths(1),
                Medicaments = medicaments
            };

            var result = await _prescriptionsController.AddPrescription(dto);
            var message = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("must be less than 10", message.Value.ToString());
        }

        [Fact]
        public async Task WrongDate()
        {
            var dto = new PrescriptionInputDto
            {
                Patient = new PatientDto
                    { FirstName = "Marek", LastName = "Miły", BirthDate = DateTime.Today.AddYears(-25) },
                IdDoctor = 1,
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddMonths(-1),
                Medicaments = new List<MedicamentToPrescriptionDto>
                {
                    new MedicamentToPrescriptionDto { IdMedicament = 1, Dose = 1, Description = "codziennie rano" }
                }
            };

            var result = await _prescriptionsController.AddPrescription(dto);
            var message = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("must be after the Date", message.Value.ToString());
        }
    }
}


