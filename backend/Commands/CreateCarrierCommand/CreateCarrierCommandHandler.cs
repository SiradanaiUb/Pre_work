using backend.Models;
using backend.Repositories;
using System;
using System.Threading.Tasks;

namespace backend.Commands.CreateCarrierCommand
{
    public class CreateCarrierCommandHandler
    {
        private readonly ICarrierRepository _repository;

        public CreateCarrierCommandHandler(ICarrierRepository repository)
        {
            _repository = repository;
        }

        public async Task<(string SjpNumber, string CarrierCode)> Handle(CreateCarrierCommand command)
        {
            // Generate SJP Number and Carrier Code
            var (sjpNumber, carrierCode) = await _repository.GenerateNewIdentifiers();

            var carrier = new Carrier
            {
                SjpNumber = sjpNumber,
                Carrier_Code = carrierCode,
                Carrier_Name = command.Carrier_Name,
                Create_Date = DateTime.Now,
                Update_Date = DateTime.Now,
                Status = "wait for approve"
            };

            await _repository.CreateCarrier(carrier);

            return (sjpNumber, carrierCode);
        }
    }
}