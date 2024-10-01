using Microsoft.AspNetCore.Mvc;
using RebelsOpdrachtenAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RebelsOpdrachtenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;

        public ItemsController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] string? lastEvaluatedKey = null, [FromQuery] int limit = 9)
        {
            var totalCount = await _itemRepository.GetTotalCountAsync();
            var result = await _itemRepository.GetItemsAsync(lastEvaluatedKey, limit);

            var nextKey = result.LastEvaluatedKey != null && result.LastEvaluatedKey.ContainsKey("UUID")
                ? result.LastEvaluatedKey["UUID"].S
                : null;

            return Ok(new 
            {
                TotalCount = totalCount,
                Items = result.Items, 
                LastEvaluatedKey = nextKey 
            });
        }
    }
}