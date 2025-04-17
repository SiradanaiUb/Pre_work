using backend.DTOs;
using backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public interface ICarrierRepository
    {
        Task<IEnumerable<CarrierDto>> GetAllCarriers();
        Task<CarrierDto?> GetCarrierById(string sjpNumber);
        Task<IEnumerable<CarrierDto>> SearchCarriers(string? sjpNumber, string? carrierCode, DateTime? createDate, string? status);
        Task<(string SjpNumber, string CarrierCode)> GenerateNewIdentifiers();
        Task<bool> CreateCarrier(Carrier carrier);
        Task<bool> UpdateCarrier(Carrier carrier);
        Task<bool> DeleteCarrier(string sjpNumber);
        Task<string?> GetCarrierStatus(string sjpNumber);
        Task<bool> UpdateCarrierStatus(string sjpNumber, string status);
        Task<bool> UpdateCarrierStatusWithApproveDate(string sjpNumber, string status);
        Task<bool> RejectCarrier(string sjpNumber, string rejectReason);
    }
}