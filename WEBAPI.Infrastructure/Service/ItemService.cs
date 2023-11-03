using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using WEBAPI.Application.Interfaces;
using WEBAPI.Domain.Helpers;
using WEBAPI.Domain.Models;
using WEBAPI.Infrastructure.DB;

namespace WEBAPI.Infrastructure.Service
{
    public class ItemService : IItemService
    {

        ApplicationDbContext AppDb;

        private readonly AppSettings _appSettings;

        public ItemService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            AppDb = new ApplicationDbContext(_appSettings.ConnectionString);
          
        }

        public IEnumerable<Item> GetAllItems()
        {
            string query = "select Id,Name,Description from Items";
            DataTable dt = AppDb.GetData(query);

            List<Item> items = AppDb.ConvertDataTableToList<Item>(dt);

            return items;
        }

        public bool CreateItem(Item item)
        {
            item.Id = Guid.NewGuid();
            string query = "insert into Items (Id, Name, Description) values (@Id, @Name, @Description);";
            var parameters = new IDataParameter[]
            {
     
            new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = item.Id },
            new SqlParameter("@Name", item.Name),
            new SqlParameter("@Description", item.Description),
    
            };

            return AppDb.ExecuteData(query, parameters) > 0;
        }

        public Item GetItemById(Guid id)
        {
            string query = "SELECT * FROM Items WHERE Id = @Id";
            var parameters = new IDataParameter[] { new SqlParameter("@Id", id) };
            DataTable dt = AppDb.GetData(query, parameters);

            if (dt.Rows.Count > 0)
            {

                return MapDataRowToItem(dt.Rows[0]);
            }

            return null; 
        }

        private Item MapDataRowToItem(DataRow row)
        {
            return new Item
            {
                Id = (Guid)row["Id"],
                Name = row["Name"].ToString(),
                Description = row["Description"].ToString()
            };
        }

        public bool UpdateItem(Guid id, Item item)
        {
            string updateProcedureName = "UpdateItem";
            var parameters = new IDataParameter[]
            {
        new SqlParameter("@Id", id),
        new SqlParameter("@Name", item.Name),
        new SqlParameter("@Description", item.Description),
            };

            return AppDb.ExecuteStoredProcedure(updateProcedureName, parameters) > 0;
        }

        public bool DeleteItem(Guid id)
        {
            string query = "delete from Items where Id = @Id";
            var parameters = new IDataParameter[] { new SqlParameter("@Id", id) };

            return AppDb.ExecuteData(query, parameters) > 0;
        }
    }
}
