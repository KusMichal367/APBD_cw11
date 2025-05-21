using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace APBD_cw11.Models;

[PrimaryKey(nameof(IdMedicament),nameof(IdPrescription))]
[Table("Prescription_Medicament")]

public class PrescriptionMedicament
{
    [ForeignKey(nameof(Medicament))]
    public int IdMedicament { get; set; }

    [ForeignKey(nameof(Prescription))]
    public int IdPrescription { get; set; }

    public int? Dose { get; set; }

    [MaxLength(100)]
    public String Details { get; set; }

    public Medicament Medicament { get; set; }
    public Prescription Prescription { get; set; }
}