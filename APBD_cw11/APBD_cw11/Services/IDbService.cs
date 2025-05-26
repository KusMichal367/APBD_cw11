using APBD_cw11.DTOs;
using APBD_cw11.Models;
using Microsoft.AspNetCore.Mvc;

namespace APBD_cw11.Services;

public interface IDbService
{
    Task<List<PrescriptionOutputDto>> GetPrescriptionsAsync();
    Task<PatientInfoDto> GetPatientsAsync(int id);
    Task<IActionResult> AddPrescriptionAsync(PrescriptionInputDto dto);
}