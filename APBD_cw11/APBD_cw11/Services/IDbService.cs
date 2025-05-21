using APBD_cw11.DTOs;
using APBD_cw11.Models;

namespace APBD_cw11.Services;

public interface IDbService
{
    Task<List<PrescriptionOutputDto>> GetPrescriptionsAsync();
    Task<PatientInfoDto> GetPatientsAsync(int id);
}