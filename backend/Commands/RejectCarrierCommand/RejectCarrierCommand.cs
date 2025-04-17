namespace backend.Commands.RejectCarrierCommand
{
    public class RejectCarrierCommand
    {
        public string SjpNumber { get; set; }
        public string RejectReason { get; set; }
        
        public RejectCarrierCommand(string sjpNumber, string rejectReason)
        {
            SjpNumber = sjpNumber;
            RejectReason = rejectReason;
        }
    }
}