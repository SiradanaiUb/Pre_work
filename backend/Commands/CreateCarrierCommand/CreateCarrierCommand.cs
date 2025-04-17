namespace backend.Commands.CreateCarrierCommand
{
    public class CreateCarrierCommand
    {
        public string Carrier_Name { get; set; }
        
        public CreateCarrierCommand(string carrierName)
        {
            Carrier_Name = carrierName;
        }
    }
}