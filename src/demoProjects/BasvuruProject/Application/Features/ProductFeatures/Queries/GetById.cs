using Application.Features.Cache;
using Application.Features.ProductFeatures.Dtos;
using Application.Features.ProductFeatures.Models;
using Application.Services;
using Application.Services.Repositories;
using AutoMapper;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ProductFeatures.Queries
{
    public class GetById : IRequest<ProductGetByIdDto>
    {
        public int Id { get; set; }


        public class GetByIdHandler : IRequestHandler<GetById, ProductGetByIdDto>
        {
            private readonly IProductRepository _productRepository;
            private readonly IMapper _mapper;
            private readonly IDatabaseAsync _cache;

            public GetByIdHandler(IProductRepository productRepository, IMapper mapper, IDatabaseAsync cacheService)
            {
                _productRepository = productRepository;
                _mapper = mapper;
                _cache = cacheService;
            }

            public async Task<ProductGetByIdDto> Handle(GetById request, CancellationToken cancellationToken)
            {
                string cacheKey = $"ProductGetById_{request.Id}";
                string cachedProductJson = await _cache.StringGetAsync(cacheKey);



                ;

                if (!string.IsNullOrEmpty(cachedProductJson))
                {
                    ProductGetByIdDto cachedProduct = JsonConvert.DeserializeObject<ProductGetByIdDto>(cachedProductJson);
                    return cachedProduct;
                }

                Product? product = await _productRepository.GetAsync(b => b.Id == request.Id);
                ProductGetByIdDto productDto = _mapper.Map<ProductGetByIdDto>(product);
                await _cache.StringSetAsync(cacheKey, JsonConvert.SerializeObject(productDto), TimeSpan.FromMinutes(10));

                return productDto;
            }
        }
    }
}
