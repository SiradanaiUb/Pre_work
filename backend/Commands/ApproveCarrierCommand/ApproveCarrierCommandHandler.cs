using backend.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace backend.Commands.ApproveCarrierCommand
{
    public class ApproveCarrierCommandHandler
    {
        private readonly ICarrierRepository _repository;

        public ApproveCarrierCommandHandler(ICarrierRepository repository)
        {
            _repository = repository;
        }

        public async Task<(bool Success, string Message)> Handle(ApproveCarrierCommand command)
        {
            // Get current status
            var currentStatus = await _repository.GetCarrierStatus(command.SjpNumber);
            
            if (currentStatus == null)
            {
                return (false, "Carrier not found");
            }
            
            // Lower case for consistent comparison
            currentStatus = currentStatus.ToLower();

            // Prevent approval if rejected
            if (currentStatus == "reject")
            {
                return (false, "Status is rejected, cannot approve");
            }

            // Handle empty or invalid status
            if (string.IsNullOrEmpty(currentStatus) || 
                !(new[] { "approve 1", "approve 2", "complete" }.Contains(currentStatus)))
            {
                currentStatus = "wait for approve";
            }

            if (currentStatus == "complete")
            {
                return (false, "Already completed");
            }

            // Determine next status
            string nextStatus = currentStatus switch
            {
                "wait for approve" => "approve 1",
                "approve 1" => "approve 2",
                "approve 2" => "complete",
                _ => "approve 1"
            };

            // Update logic
            bool result;
            if (nextStatus == "complete")
            {
                // Also update Approve_Date
                result = await _repository.UpdateCarrierStatusWithApproveDate(command.SjpNumber, nextStatus);
            }
            else
            {
                result = await _repository.UpdateCarrierStatus(command.SjpNumber, nextStatus);
            }

            return (result, $"Status updated to '{nextStatus}'");
        }
    }
}