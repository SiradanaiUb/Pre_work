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
            // Get all carriers if we need to do partial matching on SjpNumber
            if (!string.IsNullOrEmpty(query.SjpNumber))
            {
                var allCarriers = await _repository.GetAllCarriers();
                
                // Apply partial matching for SjpNumber
                var filteredCarriers = allCarriers.Where(c => 
                    c.SjpNumber != null && 
                    c.SjpNumber.Contains(query.SjpNumber, StringComparison.OrdinalIgnoreCase)).ToList();
                
                // Apply any other filters if needed
                if (!string.IsNullOrEmpty(query.CarrierCode))
                {
                    filteredCarriers = filteredCarriers.Where(c => 
                        c.Carrier_Code == query.CarrierCode).ToList();
                }
                
                if (query.CreateDate.HasValue)
                {
                    filteredCarriers = filteredCarriers.Where(c => 
                        c.Create_Date.HasValue && 
                        c.Create_Date.Value.Date == query.CreateDate.Value.Date).ToList();
                }
                
                if (!string.IsNullOrEmpty(query.Status))
                {
                    filteredCarriers = filteredCarriers.Where(c => 
                        c.Status == query.Status).ToList();
                }
                
                return filteredCarriers;
            }
            else
            {
                // If not doing partial matching on SjpNumber, use the repository's search method
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