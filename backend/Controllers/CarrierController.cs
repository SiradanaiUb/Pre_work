using backend.Commands.ApproveCarrierCommand;
using backend.Commands.CreateCarrierCommand;
using backend.Commands.DeleteCarrierCommand;
using backend.Commands.RejectCarrierCommand;
using backend.Commands.UpdateCarrierCommand;
using backend.DTOs;
using backend.Models;
using backend.Queries.GetAllCarriersQuery;
using backend.Queries.SearchCarriersQuery;
using Microsoft.AspNetCore.Mvc;
using System;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrierController : ControllerBase
    {
        private readonly GetAllCarriersQueryHandler _getAllCarriersQueryHandler;
        private readonly SearchCarriersQueryHandler _searchCarriersQueryHandler;
        private readonly CreateCarrierCommandHandler _createCarrierCommandHandler;
        private readonly UpdateCarrierCommandHandler _updateCarrierCommandHandler;
        private readonly DeleteCarrierCommandHandler _deleteCarrierCommandHandler;
        private readonly ApproveCarrierCommandHandler _approveCarrierCommandHandler;
        private readonly RejectCarrierCommandHandler _rejectCarrierCommandHandler;

        public CarrierController(
            GetAllCarriersQueryHandler getAllCarriersQueryHandler,
            SearchCarriersQueryHandler searchCarriersQueryHandler,
            CreateCarrierCommandHandler createCarrierCommandHandler,
            UpdateCarrierCommandHandler updateCarrierCommandHandler,
            DeleteCarrierCommandHandler deleteCarrierCommandHandler,
            ApproveCarrierCommandHandler approveCarrierCommandHandler,
            RejectCarrierCommandHandler rejectCarrierCommandHandler)
        {
            _getAllCarriersQueryHandler = getAllCarriersQueryHandler;
            _searchCarriersQueryHandler = searchCarriersQueryHandler;
            _createCarrierCommandHandler = createCarrierCommandHandler;
            _updateCarrierCommandHandler = updateCarrierCommandHandler;
            _deleteCarrierCommandHandler = deleteCarrierCommandHandler;
            _approveCarrierCommandHandler = approveCarrierCommandHandler;
            _rejectCarrierCommandHandler = rejectCarrierCommandHandler;
        }

        [HttpGet]
        [Route("GetAllCarriers")]
        public async Task<IActionResult> GetCarriers()
        {
            var query = new GetAllCarriersQuery();
            var carriers = await _getAllCarriersQueryHandler.Handle(query);

            if (carriers != null && carriers.Any())
            {
                return Ok(carriers);
            }
            else
            {
                return NotFound(new ResponseDto 
                { 
                    StatusCode = 100, 
                    Message = "No Data Found" 
                });
            }
        }

        [HttpGet]
        [Route("SearchCarriers")]
        public async Task<IActionResult> SearchCarriers(string? sjpNumber, string? carrierCode, DateTime? createDate, string? status)
        {
            var query = new SearchCarriersQuery
            {
                SjpNumber = sjpNumber,
                CarrierCode = carrierCode,
                CreateDate = createDate,
                Status = status
            };

            var carriers = await _searchCarriersQueryHandler.Handle(query);
            return Ok(carriers);
        }

        [HttpPost]
        [Route("CreateCarrier")]
        public async Task<IActionResult> CreateCarrier([FromBody] Carrier carrier)
        {
            var command = new CreateCarrierCommand(carrier.Carrier_Name);
            var (sjpNumber, carrierCode) = await _createCarrierCommandHandler.Handle(command);

            return Ok(new ResponseDto
            {
                StatusCode = 200,
                Message = "Carrier Created",
                Data = new
                {
                    SjpNumber = sjpNumber,
                    Carrier_Code = carrierCode,
                    Created_At = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }
            });
        }

        [HttpPut]
        [Route("EditCarrier")]
        public async Task<IActionResult> EditCarrier([FromBody] Carrier carrier)
        {
            var command = new UpdateCarrierCommand(
                carrier.SjpNumber,
                carrier.Carrier_Code,
                carrier.Carrier_Name,
                carrier.Status);

            var result = await _updateCarrierCommandHandler.Handle(command);

            if (result)
            {
                return Ok(new ResponseDto
                {
                    StatusCode = 200,
                    Message = "Carrier Updated"
                });
            }
            else
            {
                return NotFound(new ResponseDto
                {
                    StatusCode = 404,
                    Message = "Carrier Not Found"
                });
            }
        }

        [HttpDelete]
        [Route("DeleteCarrier/{sjpNumber}")]
        public async Task<IActionResult> DeleteCarrier(string sjpNumber)
        {
            var command = new DeleteCarrierCommand(sjpNumber);
            var result = await _deleteCarrierCommandHandler.Handle(command);

            if (result)
            {
                return Ok(new ResponseDto
                {
                    StatusCode = 200,
                    Message = "Carrier Deleted"
                });
            }
            else
            {
                return NotFound(new ResponseDto
                {
                    StatusCode = 404,
                    Message = "Carrier Not Found"
                });
            }
        }

        [HttpPatch]
        [Route("ApproveCarrier")]
        public async Task<IActionResult> ApproveCarrier(string sjpNumber)
        {
            var command = new ApproveCarrierCommand(sjpNumber);
            var (success, message) = await _approveCarrierCommandHandler.Handle(command);

            if (success)
            {
                return Ok(new ResponseDto
                {
                    StatusCode = 200,
                    Message = message
                });
            }
            else
            {
                return BadRequest(new ResponseDto
                {
                    StatusCode = 400,
                    Message = message
                });
            }
        }

        [HttpPatch]
        [Route("RejectCarrier")]
        public async Task<IActionResult> RejectCarrier(string sjpNumber, string rejectReason)
        {
            var command = new RejectCarrierCommand(sjpNumber, rejectReason);
            var (success, message) = await _rejectCarrierCommandHandler.Handle(command);

            if (success)
            {
                return Ok(new ResponseDto
                {
                    StatusCode = 200,
                    Message = message
                });
            }
            else
            {
                return BadRequest(new ResponseDto
                {
                    StatusCode = 400,
                    Message = message
                });
            }
        }
    }
}