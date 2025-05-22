using System.Text.Json.Serialization;
using APBD_cw11.Models;

namespace APBD_cw11.DTOs;

public class PatientDto
{
    public int IdPatient { get; set; }
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public DateTime BirthDate { get; set; }
}

public class MedicamentToPrescriptionDto
{
    public int IdMedicament { get; set; }
    public int Dose { get; set; }
    public String Description { get; set; }
}

public class DoctorDto
{
    public int IdDoctor { get; set; }
    public String FirstName { get; set; }
}

public class PrescriptionInputDto
{
    public PatientDto Patient { get; set; }
    public int IdDoctor { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentToPrescriptionDto> Medicaments { get; set; }
}

public class MedicamentDto
{
    public int IdMedicament { get; set; }
    public String Name { get; set; }
    public int Dose { get; set; }
    public String Description { get; set; }
}

public class PrescriptionOutputDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentDto> Medicaments { get; set; }
    public DoctorDto Doctor { get; set; }
}

public class PatientInfoDto : PatientDto
{
    [JsonPropertyOrder(5)]
    public List<PrescriptionOutputDto> Prescriptions { get; set; }
}