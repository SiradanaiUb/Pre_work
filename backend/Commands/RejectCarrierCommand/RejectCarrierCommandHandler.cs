using backend.Repositories;
using System.Threading.Tasks;

namespace backend.Commands.RejectCarrierCommand
{
    public class RejectCarrierCommandHandler
    {
        private readonly ICarrierRepository _repository;

        public RejectCarrierCommandHandler(ICarrierRepository repository)
        {
            _repository = repository;
        }

        public async Task<(bool Success, string Message)> Handle(RejectCarrierCommand command)
        {
            // Get current status
            var currentStatus = await _repository.GetCarrierStatus(command.SjpNumber);
            
            if (currentStatus == null)
            {
                return (false, "Carrier not found");
            }
            
            // Lower case for consistent comparison
            currentStatus = currentStatus.ToLower();

            if (currentStatus == "reject" || currentStatus == "complete")
            {
                return (false, "Cannot reject at this stage");
            }

            // Update status to reject and add reject reason
            bool result = await _repository.RejectCarrier(command.SjpNumber, command.RejectReason);

            return (result, "Status updated to 'reject'");
        }
    }
}