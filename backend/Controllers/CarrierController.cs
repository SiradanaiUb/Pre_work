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
            
    }
}
