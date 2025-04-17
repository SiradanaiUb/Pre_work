using backend.DTOs;
using backend.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Queries.SearchCarriersQuery
{
    public class SearchCarriersQueryHandler
    {
        private readonly ICarrierRepository _repository;

        public SearchCarriersQueryHandler(ICarrierRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CarrierDto>> Handle(SearchCarriersQuery query)
        {
            var carriers = await _repository.SearchCarriers(
                query.SjpNumber,
                query.CarrierCode,
                query.CreateDate,
                query.Status
            );
            
            return carriers;
        }
    }
}