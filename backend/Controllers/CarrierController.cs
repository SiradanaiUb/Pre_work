using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrierController : ControllerBase
    {
        public readonly IConfiguration _configuration;

        public CarrierController(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
        [HttpGet]
        [Route("GetAllCarriers")]
        public string GetCarriers()
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("StoreDbConnection").ToString());
            SqlDataAdapter da = new SqlDataAdapter("select * from CarrierInfo", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Carrier> carrierList = new List<Carrier>();
            Responce responce = new Responce();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Carrier carrier = new Carrier();
                    carrier.SjpNumber = Convert.ToString(dt.Rows[i]["SjpNumber"]);
                    carrier.Carrier_Code = Convert.ToString(dt.Rows[i]["Carrier_Code"]);
                    carrier.Carrier_Name = Convert.ToString(dt.Rows[i]["Carrier_Name"]);
                    carrier.Create_Date = Convert.ToDateTime(dt.Rows[i]["Create_Date"]);
                    carrier.Update_Date = Convert.ToDateTime(dt.Rows[i]["Update_Date"]);
                    carrier.Status = Convert.ToString(dt.Rows[i]["Status"]);
                    carrier.Reject_Reason = Convert.ToString(dt.Rows[i]["Reject_Resaon"]);
                    //carrier.Approve_Date = Convert.ToDateTime(dt.Rows[i]["Approve_Date"]);
                    carrierList.Add(carrier);
                }
            }
            if (carrierList.Count > 0)
                return JsonConvert.SerializeObject(carrierList);
            else
            {
                responce.SatusCode = 100;
                responce.ErrorMessage = "No Data Found";
                return JsonConvert.SerializeObject(responce);
            }
        }
        [HttpPost]
        [Route("CreateCarrier")]
        public IActionResult CreateCarrier([FromBody] Carrier carrier)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("StoreDbConnection"));
            con.Open();

            // Step 1: Auto-generate SJP Number (e.g., PRQ-000001)
            string newSjpNumber = "PRQ-000001";
            SqlCommand getLastSjpCmd = new SqlCommand("SELECT TOP 1 SJP_Number FROM CarrierInfo WHERE SJP_Number LIKE 'PRQ-%' ORDER BY SJP_Number DESC", con);
            var lastSjp = getLastSjpCmd.ExecuteScalar()?.ToString();

            if (!string.IsNullOrEmpty(lastSjp) && lastSjp.StartsWith("PRQ-"))
            {
                int lastNumber = int.Parse(lastSjp.Substring(4));
                newSjpNumber = "PRQ-" + (lastNumber + 1).ToString("D6");
            }

            // Step 2: Auto-generate Carrier_Code (e.g., 00001)
            string newCarrierCode = "00001";
            SqlCommand getLastCodeCmd = new SqlCommand("SELECT TOP 1 Carrier_Code FROM CarrierInfo ORDER BY SJP_Number DESC", con);
            var lastCode = getLastCodeCmd.ExecuteScalar()?.ToString();

            if (!string.IsNullOrEmpty(lastCode) && int.TryParse(lastCode, out int lastNum))
            {
                newCarrierCode = (lastNum + 1).ToString("D5");
            }

            // Step 3: Set timestamps
            DateTime now = DateTime.Now;

            // Step 4: Insert record
            SqlCommand cmd = new SqlCommand("INSERT INTO CarrierInfo (SJP_Number, Carrier_Code, Carrier_Name, Create_Date, Update_Date, Status) VALUES (@SjpNumber, @Carrier_Code, @Carrier_Name, @Create_Date, @Update_Date, @Status)", con);

            cmd.Parameters.AddWithValue("@SjpNumber", newSjpNumber);
            cmd.Parameters.AddWithValue("@Carrier_Code", newCarrierCode);
            cmd.Parameters.AddWithValue("@Carrier_Name", carrier.Carrier_Name);
            cmd.Parameters.AddWithValue("@Create_Date", now);
            cmd.Parameters.AddWithValue("@Update_Date", now);
            cmd.Parameters.AddWithValue("@Status", "wait for approve");

            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();

            return Ok(new
            {
                message = rowsAffected > 0 ? "Carrier Created" : "Failed to Create",
                SjpNumber = newSjpNumber,
                Carrier_Code = newCarrierCode,
                Created_At = now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        [HttpDelete]
        [Route("DeleteCarrier/{sjpNumber}")]
        public IActionResult DeleteCarrier(string sjpNumber)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("StoreDbConnection"));
            using SqlCommand cmd = new SqlCommand("DELETE FROM CarrierInfo WHERE SJP_Number = @SjpNumber", con);
            cmd.Parameters.AddWithValue("@SjpNumber", sjpNumber);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();

            return Ok(new { message = rowsAffected > 0 ? "Carrier Deleted" : "Carrier Not Found" });
        }
        [HttpPut]
        [Route("EditCarrier")]
        public IActionResult EditCarrier([FromBody] Carrier carrier)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("StoreDbConnection"));
            using SqlCommand cmd = new SqlCommand("UPDATE CarrierInfo SET Carrier_Code = @Carrier_Code, Carrier_Name = @Carrier_Name, Update_Date = @Update_Date, Status = @Status WHERE SJP_Number = @SjpNumber", con);
            
            cmd.Parameters.AddWithValue("@SjpNumber", carrier.SjpNumber);
            cmd.Parameters.AddWithValue("@Carrier_Code", carrier.Carrier_Code);
            cmd.Parameters.AddWithValue("@Carrier_Name", carrier.Carrier_Name);
            cmd.Parameters.AddWithValue("@Update_Date", carrier.Update_Date);
            cmd.Parameters.AddWithValue("@Status", carrier.Status);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();

            return Ok(new { message = rowsAffected > 0 ? "Carrier Updated" : "Carrier Not Found" });
        }
        [HttpGet]
        [Route("SearchCarriers")]
        public IActionResult SearchCarriers(string? sjpNumber, string? carrierCode, DateTime? createDate, string? status)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("StoreDbConnection"));
            string query = "SELECT * FROM CarrierInfo WHERE 1=1";

            if (!string.IsNullOrEmpty(sjpNumber)) query += " AND SjpNumber = @SjpNumber";
            if (!string.IsNullOrEmpty(carrierCode)) query += " AND Carrier_Code = @Carrier_Code";
            if (createDate.HasValue) query += " AND CAST(Create_Date AS DATE) = @Create_Date";
            if (!string.IsNullOrEmpty(status)) query += " AND Status = @Status";

            using SqlCommand cmd = new SqlCommand(query, con);
            if (!string.IsNullOrEmpty(sjpNumber)) cmd.Parameters.AddWithValue("@SjpNumber", sjpNumber);
            if (!string.IsNullOrEmpty(carrierCode)) cmd.Parameters.AddWithValue("@Carrier_Code", carrierCode);
            if (createDate.HasValue) cmd.Parameters.AddWithValue("@Create_Date", createDate.Value.Date);
            if (!string.IsNullOrEmpty(status)) cmd.Parameters.AddWithValue("@Status", status);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var carriers = new List<Carrier>();
            foreach (DataRow row in dt.Rows)
            {
                carriers.Add(new Carrier
                {
                    SjpNumber = row["SjpNumber"].ToString(),
                    Carrier_Code = row["Carrier_Code"].ToString(),
                    Carrier_Name = row["Carrier_Name"].ToString(),
                    Create_Date = Convert.ToDateTime(row["Create_Date"]),
                    Update_Date = Convert.ToDateTime(row["Update_Date"]),
                    Status = row["Status"].ToString(),
                    Reject_Reason = row["Reject_Resaon"].ToString()
                });
            }

            return Ok(carriers);
        }
        [HttpPatch]
        [Route("ApproveCarrier")]
        public IActionResult ApproveCarrier(string sjpNumber)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("StoreDbConnection"));
            con.Open();

            SqlCommand getCmd = new SqlCommand("SELECT Status FROM CarrierInfo WHERE SJP_Number = @SjpNumber", con);
            getCmd.Parameters.AddWithValue("@SjpNumber", sjpNumber);
            var currentStatus = getCmd.ExecuteScalar()?.ToString()?.ToLower();

            // ✅ First: Prevent approval if rejected
            if (currentStatus == "reject")
            {
                con.Close();
                return BadRequest(new { message = "status is rejected, cannot approve" });
            }

            // ✅ Second: Handle empty or invalid status
            if (string.IsNullOrEmpty(currentStatus) || !(new[] { "approve 1", "approve 2", "complete" }.Contains(currentStatus)))
            {
                currentStatus = "wait for approve";
            }

            if (currentStatus == "complete")
            {
                con.Close();
                return BadRequest(new { message = "already completed" });
            }

            // ✅ Determine next status
            string nextStatus = currentStatus switch
            {
                "wait for approve" => "approve 1",
                "approve 1" => "approve 2",
                "approve 2" => "complete",
                _ => "approve 1"
            };

            // ✅ Update logic
            SqlCommand updateCmd;
            if (nextStatus == "complete")
            {
                // Also update Approve_Date
                updateCmd = new SqlCommand("UPDATE CarrierInfo SET Status = @Status, Approve_Date = GETDATE(), Update_Date = GETDATE() WHERE SJP_Number = @SjpNumber", con);
            }
            else
            {
                updateCmd = new SqlCommand("UPDATE CarrierInfo SET Status = @Status, Update_Date = GETDATE() WHERE SJP_Number = @SjpNumber", con);
            }

            updateCmd.Parameters.AddWithValue("@SjpNumber", sjpNumber);
            updateCmd.Parameters.AddWithValue("@Status", nextStatus);
            updateCmd.ExecuteNonQuery();

            con.Close();
            return Ok(new { message = $"status updated to '{nextStatus}'" });
        }
        [HttpPatch]
        [Route("RejectCarrier")]
        public IActionResult RejectCarrier(string sjpNumber)
        {
            using SqlConnection con = new SqlConnection(_configuration.GetConnectionString("StoreDbConnection"));
            con.Open();

            SqlCommand getCmd = new SqlCommand("SELECT Status FROM CarrierInfo WHERE SJP_Number = @SjpNumber", con);
            getCmd.Parameters.AddWithValue("@SjpNumber", sjpNumber);
            var currentStatus = getCmd.ExecuteScalar()?.ToString()?.ToLower();

            if (string.IsNullOrEmpty(currentStatus))
            {
                currentStatus = "wait for approve"; // default
            }

            if (currentStatus == "reject" || currentStatus == "complete")
            {
                con.Close();
                return BadRequest(new { message = "cannot reject at this stage" });
            }

            SqlCommand updateCmd = new SqlCommand("UPDATE CarrierInfo SET Status = @Status, Update_Date = GETDATE() WHERE SJP_Number = @SjpNumber", con);
            updateCmd.Parameters.AddWithValue("@SjpNumber", sjpNumber);
            updateCmd.Parameters.AddWithValue("@Status", "reject");
            updateCmd.ExecuteNonQuery();

            con.Close();
            return Ok(new { message = "status updated to 'reject'" });
        }

    }
}
