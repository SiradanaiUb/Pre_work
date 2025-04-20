using backend.DTOs;
using backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!string.IsNullOrEmpty(query.SjpNumber))
            {
                // Fetch all carriers to allow partial match on SjpNumber
                var allCarriers = await _repository.GetAllCarriers();

                var filteredCarriers = allCarriers.Where(c =>
                    (string.IsNullOrEmpty(query.SjpNumber) || 
                        (c.SjpNumber != null && c.SjpNumber.Contains(query.SjpNumber, StringComparison.OrdinalIgnoreCase))) &&
                    (string.IsNullOrEmpty(query.CarrierCode) || 
                        c.Carrier_Code == query.CarrierCode) &&
                    (!query.CreateDate.HasValue || 
                        (c.Create_Date.HasValue && c.Create_Date.Value.Date == query.CreateDate.Value.Date)) &&
                    (string.IsNullOrEmpty(query.Status) || 
                        c.Status == query.Status)
                ).ToList();

                return filteredCarriers;
            }
            else
            {
                // Use optimized repository query when partial match not needed
                return await _repository.SearchCarriers(
                    query.SjpNumber,
                    query.CarrierCode,
                    query.CreateDate,
                    query.Status
                );
            }
        }
    }
}
