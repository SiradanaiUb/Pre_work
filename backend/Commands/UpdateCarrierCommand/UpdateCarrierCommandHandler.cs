using backend.Models;
using backend.Repositories;
using System;
using System.Threading.Tasks;

namespace backend.Commands.UpdateCarrierCommand
{
    public class UpdateCarrierCommandHandler
    {
        private readonly ICarrierRepository _repository;

        public UpdateCarrierCommandHandler(ICarrierRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateCarrierCommand command)
        {
            var carrier = new Carrier
            {
                SjpNumber = command.SjpNumber,
                Carrier_Code = command.Carrier_Code,
                Carrier_Name = command.Carrier_Name,
                Update_Date = DateTime.Now,
                Status = command.Status
            };

            return await _repository.UpdateCarrier(carrier);
        }
    }
}