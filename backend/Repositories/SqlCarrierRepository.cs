using backend.DTOs;
using backend.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class SqlCarrierRepository : ICarrierRepository
    {
        private readonly string _connectionString;

        public SqlCarrierRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("StoreDbConnection");
        }

        public async Task<IEnumerable<CarrierDto>> GetAllCarriers()
        {
            var carriers = new List<CarrierDto>();
            
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand("SELECT * FROM CarrierInfo", connection);
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    carriers.Add(MapToCarrierDto(reader));
                }
            }
            
            return carriers;
        }

        public async Task<CarrierDto?> GetCarrierById(string sjpNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand("SELECT * FROM CarrierInfo WHERE SJP_Number = @SjpNumber", connection);
                command.Parameters.AddWithValue("@SjpNumber", sjpNumber);
                
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return MapToCarrierDto(reader);
                }
            }
            
            return null;
        }

        public async Task<IEnumerable<CarrierDto>> SearchCarriers(string? sjpNumber, string? carrierCode, DateTime? createDate, string? status)
        {
            var carriers = new List<CarrierDto>();
            
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT * FROM CarrierInfo WHERE 1=1";

                if (!string.IsNullOrEmpty(sjpNumber)) query += " AND SJP_Number = @SjpNumber";
                if (!string.IsNullOrEmpty(carrierCode)) query += " AND Carrier_Code = @CarrierCode";
                if (createDate.HasValue) query += " AND CAST(Create_Date AS DATE) = @CreateDate";
                if (!string.IsNullOrEmpty(status)) query += " AND Status = @Status";

                using var command = new SqlCommand(query, connection);
                
                if (!string.IsNullOrEmpty(sjpNumber)) command.Parameters.AddWithValue("@SjpNumber", sjpNumber);
                if (!string.IsNullOrEmpty(carrierCode)) command.Parameters.AddWithValue("@CarrierCode", carrierCode);
                if (createDate.HasValue) command.Parameters.AddWithValue("@CreateDate", createDate.Value.Date);
                if (!string.IsNullOrEmpty(status)) command.Parameters.AddWithValue("@Status", status);
                
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    carriers.Add(MapToCarrierDto(reader));
                }
            }
            
            return carriers;
        }

        public async Task<(string SjpNumber, string CarrierCode)> GenerateNewIdentifiers()
        {
            string newSjpNumber = "PRQ-000001";
            string newCarrierCode = "00001";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                // Generate SJP Number
                using (var command = new SqlCommand("SELECT TOP 1 SJP_Number FROM CarrierInfo WHERE SJP_Number LIKE 'PRQ-%' ORDER BY SJP_Number DESC", connection))
                {
                    var lastSjp = (await command.ExecuteScalarAsync())?.ToString();
                    
                    if (!string.IsNullOrEmpty(lastSjp) && lastSjp.StartsWith("PRQ-"))
                    {
                        int lastNumber = int.Parse(lastSjp.Substring(4));
                        newSjpNumber = "PRQ-" + (lastNumber + 1).ToString("D6");
                    }
                }
                
                // Generate Carrier Code
                using (var command = new SqlCommand("SELECT TOP 1 Carrier_Code FROM CarrierInfo ORDER BY Carrier_Code DESC", connection))
                {
                    var lastCode = (await command.ExecuteScalarAsync())?.ToString();
                    
                    if (!string.IsNullOrEmpty(lastCode) && int.TryParse(lastCode, out int lastNum))
                    {
                        newCarrierCode = (lastNum + 1).ToString("D5");
                    }
                }
            }
            
            return (newSjpNumber, newCarrierCode);
        }

        public async Task<bool> CreateCarrier(Carrier carrier)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand(
                    "INSERT INTO CarrierInfo (SJP_Number, Carrier_Code, Carrier_Name, Create_Date, Update_Date, Status) " +
                    "VALUES (@SjpNumber, @Carrier_Code, @Carrier_Name, @Create_Date, @Update_Date, @Status)", 
                    connection);
                
                command.Parameters.AddWithValue("@SjpNumber", carrier.SjpNumber);
                command.Parameters.AddWithValue("@Carrier_Code", carrier.Carrier_Code);
                command.Parameters.AddWithValue("@Carrier_Name", carrier.Carrier_Name);
                command.Parameters.AddWithValue("@Create_Date", carrier.Create_Date);
                command.Parameters.AddWithValue("@Update_Date", carrier.Update_Date);
                command.Parameters.AddWithValue("@Status", carrier.Status);
                
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateCarrier(Carrier carrier)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand(
                    "UPDATE CarrierInfo SET Carrier_Code = @Carrier_Code, Carrier_Name = @Carrier_Name, " +
                    "Update_Date = @Update_Date, Status = @Status " +
                    "WHERE SJP_Number = @SjpNumber", 
                    connection);
                
                command.Parameters.AddWithValue("@SjpNumber", carrier.SjpNumber);
                command.Parameters.AddWithValue("@Carrier_Code", carrier.Carrier_Code);
                command.Parameters.AddWithValue("@Carrier_Name", carrier.Carrier_Name);
                command.Parameters.AddWithValue("@Update_Date", DateTime.Now);
                command.Parameters.AddWithValue("@Status", carrier.Status);
                
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteCarrier(string sjpNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand(
                    "DELETE FROM CarrierInfo WHERE SJP_Number = @SjpNumber", 
                    connection);
                
                command.Parameters.AddWithValue("@SjpNumber", sjpNumber);
                
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<string?> GetCarrierStatus(string sjpNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand("SELECT Status FROM CarrierInfo WHERE SJP_Number = @SjpNumber", connection);
                command.Parameters.AddWithValue("@SjpNumber", sjpNumber);
                
                return (await command.ExecuteScalarAsync())?.ToString();
            }
        }

        public async Task<bool> UpdateCarrierStatus(string sjpNumber, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand(
                    "UPDATE CarrierInfo SET Status = @Status, Update_Date = GETDATE() WHERE SJP_Number = @SjpNumber", 
                    connection);
                
                command.Parameters.AddWithValue("@SjpNumber", sjpNumber);
                command.Parameters.AddWithValue("@Status", status);
                
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateCarrierStatusWithApproveDate(string sjpNumber, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand(
                    "UPDATE CarrierInfo SET Status = @Status, Update_Date = GETDATE(), Approve_Date = GETDATE() WHERE SJP_Number = @SjpNumber", 
                    connection);
                
                command.Parameters.AddWithValue("@SjpNumber", sjpNumber);
                command.Parameters.AddWithValue("@Status", status);
                
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> RejectCarrier(string sjpNumber, string rejectReason)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using var command = new SqlCommand(
                    "UPDATE CarrierInfo SET Status = 'reject', Reject_Resaon = @RejectReason, Update_Date = GETDATE() WHERE SJP_Number = @SjpNumber", 
                    connection);
                
                command.Parameters.AddWithValue("@SjpNumber", sjpNumber);
                command.Parameters.AddWithValue("@RejectReason", rejectReason);
                
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        private CarrierDto MapToCarrierDto(SqlDataReader reader)
        {
            return new CarrierDto
            {
                SjpNumber = reader["SJP_Number"].ToString(),
                Carrier_Code = reader["Carrier_Code"].ToString(),
                Carrier_Name = reader["Carrier_Name"].ToString(),
                Create_Date = reader["Create_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Create_Date"]) : null,
                Update_Date = reader["Update_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Update_Date"]) : null,
                Status = reader["Status"].ToString(),
                Reject_Reason = reader["Reject_Resaon"].ToString(),
                Approve_Date = reader["Approve_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Approve_Date"]) : null
            };
        }
    }
}