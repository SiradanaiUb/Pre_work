namespace backend.Commands.UpdateCarrierCommand
{
    public class UpdateCarrierCommand
    {
        public string SjpNumber { get; set; }
        public string Carrier_Code { get; set; }
        public string Carrier_Name { get; set; }
        public string Status { get; set; }
        
        public UpdateCarrierCommand(string sjpNumber, string carrierCode, string carrierName, string status)
        {
            SjpNumber = sjpNumber;
            Carrier_Code = carrierCode;
            Carrier_Name = carrierName;
            Status = status;
        }
    }
}