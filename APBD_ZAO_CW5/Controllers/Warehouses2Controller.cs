using APBD_ZAO_CW5.DAL;
using APBD_ZAO_CW5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace APBD_ZAO_CW5.Controllers
{
    [Route("api/warehouses2")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {
        private readonly IDbProcedureService _dbProcedureService;
        public Warehouses2Controller(IDbProcedureService DbProcedureService)
        {
            _dbProcedureService = DbProcedureService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductWarehouse productWarehouse)
        {
            int idProductWarehouse;
            try { idProductWarehouse = await _dbProcedureService.AddProductToWarehouseAsync(productWarehouse); 
            }
            catch (Exception e) { 
                return NotFound(e.Message); 
            }
            return Ok($"Succsesfully added! ID: {idProductWarehouse}!");
        }
    }
}
