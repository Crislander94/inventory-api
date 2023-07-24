using Amazon.Runtime.SharedInterfaces;
using InventarioAPI.DAL_s;
using InventarioAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class CategoryController : ControllerBase
    {
        private readonly IConfiguration _Config;

        public CategoryController(IConfiguration iconfig)
        {
            _Config = iconfig;
        }


        // POST api/<CategoryController>
        [HttpPost]
        public Response createCategory([FromBody] Category category)
        {
            try
            {
                Response _response = new Response();
                CategoryDAL action_category = new CategoryDAL(_Config);
                Category new_category = action_category.createCategoryDAL(category);
                _response.statusCode = 200;
                _response.message = "Se creo la categoria correctamente";
                _response.responseText = JsonSerializer.Serialize(new_category);
                return _response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public Response getCategories()
        {
            Response _response = new Response();
            CategoryDAL action_category = new CategoryDAL(_Config);
            List<Category> list_category = action_category.getCategoryDAL();
            List<Category> new_list_category;
            if (list_category.Count > 0)
            {
                _response.statusCode = 200;
                _response.message = "Se encontraron categorias";
            }
            else
            {
                _response.statusCode = 404;
                _response.message = "No existen categorias creadas";
            }

            _response.responseText = JsonSerializer.Serialize(new
            {
                _Count = list_category.Count,
                Categories = list_category
            });

            //_response.responseText = JsonSerializer.Serialize(new{
            //    _Count = list_category.Count,
            //    Categories = Helpers.Helpers.generateResponseCategories(list_category)
            //});
            return _response;
        }

        //// GET api/<CategoryController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}
        //// PUT api/<CategoryController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<CategoryController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
