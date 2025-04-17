using backend.DTOs;
using backend.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Queries.GetAllCarriersQuery
{
    public class GetAllCarriersQueryHandler
    {
        private readonly ICarrierRepository _repository;

        public GetAllCarriersQueryHandler(ICarrierRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CarrierDto>> Handle(GetAllCarriersQuery query)
        {
            var carriers = await _repository.GetAllCarriers();
            return carriers;
        }
    }
}