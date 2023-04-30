using Application.Features.Cache;
using Application.Features.OrderFeatures.Commands;
using Application.Features.OrderFeatures.Dtos;
using Application.Features.OrderFeatures.Queries;
using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Contexts;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        protected IMediator _mediator;
        private readonly ICacheService _cacheService;
        private readonly BaseDbContext _context;


        public OrderController(IMediator mediator, ICacheService cacheService, BaseDbContext context)
        {
            _mediator = mediator;
            _cacheService = cacheService;
            _context = context;

        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]CreateOrderCommand createOrderCommand)
        {
            CreatedOrderResponse result = await Mediator.Send(createOrderCommand);
            _cacheService.RemoveData("orders");
            _context.SaveChanges();
            return Created("",result);
        }
    }
}
