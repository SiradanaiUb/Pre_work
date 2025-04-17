using backend.Repositories;
using System.Threading.Tasks;

namespace backend.Commands.DeleteCarrierCommand
{
    public class DeleteCarrierCommandHandler
    {
        private readonly ICarrierRepository _repository;

        public DeleteCarrierCommandHandler(ICarrierRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteCarrierCommand command)
        {
            return await _repository.DeleteCarrier(command.SjpNumber);
        }
    }
}