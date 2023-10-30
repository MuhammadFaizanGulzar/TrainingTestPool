using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEBAPI.Domain.Models;

namespace WEBAPI.Application.Interfaces
{
    public interface IItemService
    {
        IEnumerable<Item> GetAllItems();
        Item GetItemById(Guid id);
        bool CreateItem(Item item);
        bool UpdateItem(Guid id, Item item);
        bool DeleteItem(Guid id);
    }
}
