namespace backend.Commands.DeleteCarrierCommand
{
    public class DeleteCarrierCommand
    {
        public string SjpNumber { get; set; }
        
        public DeleteCarrierCommand(string sjpNumber)
        {
            SjpNumber = sjpNumber;
        }
    }
}