using APBD_ZAO_CW5.DAL;
using APBD_ZAO_CW5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace APBD_ZAO_CW5.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IDbService _dbService;

        public WarehousesController(IDbService dbService)
        {
            _dbService = dbService;
        }


        [HttpPost]
        public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductWarehouse productWarehouse)
        {
            int idProductWarehouse;
            try { idProductWarehouse = await _dbService.AddProductToWarehouseAsync(productWarehouse); 
            }
            catch (Exception e) { 
                return NotFound(e.Message); 
            }
            return Ok($"Succsesfully added! ID: {idProductWarehouse}!");
        }
    }
}
