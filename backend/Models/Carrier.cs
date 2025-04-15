namespace backend.Models
{
    public class Carrier
    {
        public string SjpNumber { get; set; }
        public string Carrier_Code { get; set; }
        public string Carrier_Name { get; set; }
        public DateTime Create_Date { get; set; }
        public DateTime Update_Date { get; set; }
        public string Status { get; set; }
        public string Reject_Reason { get; set; }
        public DateTime Approve_Date { get; set; }

    }
}
