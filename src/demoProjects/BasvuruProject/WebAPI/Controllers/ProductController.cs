using Application.Features.ProductFeatures.Command;
using Application.Features.ProductFeatures.Dtos;
using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using Application.Services;
using Core.Application.Requests;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Contexts;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        private static object _lock = new object();
        private readonly ICacheService _cacheService;


        private readonly BaseDbContext _baseDbContext;
        protected IMediator _mediator;

        public ProductController(IMediator mediator, ICacheService cacheService, BaseDbContext baseDbContext)
        {
            _baseDbContext = baseDbContext;
            _mediator = mediator;
            _cacheService = cacheService;

        }
        [HttpGet]
        public async Task<IActionResult> GetByCategory(string Category)
        {
            ProductListModel result = await Mediator.Send(new GetListProductByCategoryQuery() { Category=Category, BypassCache = false });
            return Ok(result);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] GetById getById)
        {
            
            ProductGetByIdDto result = await Mediator.Send(getById);
            
            return Ok(result);

        }
        [HttpGet("products")]
        public IEnumerable<Product> Get()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null)
            {
                return cacheData;
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            cacheData = _baseDbContext.Products.ToList();
            _cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
            return cacheData;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody] CreateProductCommand createProductCommand)
        {

            var result = await Mediator.Send(createProductCommand);
            _cacheService.RemoveData("product");

            return Ok(result); 
           
        }


    }
}
