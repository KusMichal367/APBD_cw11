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
    public class PrescriptionsTest
    {
        private readonly DatabaseContext _context;
        private readonly IDbService _dbService;
        private readonly PrescriptionsController _prescriptionsController;

        public PrescriptionsTest()
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
        public async Task CreatePrescription()
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
        }
    }
}


