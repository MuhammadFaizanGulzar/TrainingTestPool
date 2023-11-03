using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WEBAPI.Application.Interfaces;
using WEBAPI.Domain.Models;

namespace Task5_WEBAPI_AJAX.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class ItemController : Controller
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }


        [HttpGet("getAllItems")]
        public ActionResult<IEnumerable<Item>> Get()
        {
            var items = _itemService.GetAllItems();
            return Ok(items);
        }


        [HttpGet("getById/{id}")]
        public ActionResult<Item> GetItemById(Guid id)
        {
            var item = _itemService.GetItemById(id);

            if (item != null)
            {
                return Ok(item);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("createItem")]
        public ActionResult Post([FromBody] Item item)
        {
            if (_itemService.CreateItem(item))
            {
                return Ok(new { Result = "Saved" });
            }
            else
            {
                return NotFound(new { Result = "something went wrong" });
            }
        }

        [HttpPut("updateItem/{id}")]
        public ActionResult Put(Guid id, [FromBody] Item item)
        {
            bool updated = _itemService.UpdateItem(id, item);

            if (updated)
            {
                return Ok(new { Result = "Updated" });
            }
            else
            {
                return NotFound(new { Result = "Item not found or something went wrong" });
            }
        }

        [HttpDelete("deleteItem/{id}")]
        public ActionResult Delete(Guid id)
        {
            bool deleted = _itemService.DeleteItem(id);

            if (deleted)
            {
                return Ok(new { Result = "Deleted" });
            }
            else
            {
                return NotFound(new { Result = "Item not found or something went wrong" });
            }
        }
    }
}
