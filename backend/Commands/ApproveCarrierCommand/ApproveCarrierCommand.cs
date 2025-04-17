namespace backend.Commands.ApproveCarrierCommand
{
    public class ApproveCarrierCommand
    {
        public string SjpNumber { get; set; }
        
        public ApproveCarrierCommand(string sjpNumber)
        {
            SjpNumber = sjpNumber;
        }
    }
}