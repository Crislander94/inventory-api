using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using InventarioAPI.Models;
using InventarioAPI.DAL_s;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Text.Json;
using InventarioAPI.Helpers;

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class ProductController : Controller
    {
        private readonly IConfiguration _Config;

        public ProductController(IConfiguration iconfig)
        {
            _Config = iconfig;
        }
        //config["AppSettings:Domain"]

        [HttpPost]
        public Response CreateProduct([FromBody] Product product)
        {
            try
            {
                Response response = new Response();
                ProductDAL ProductDal = new ProductDAL(_Config);
                Product new_product = ProductDal.createProductDAL(product);
                if (new_product == null)
                {
                    response.statusCode = 400;
                    response.message = "Error al crear el producto";
                }
                else
                {
                    response.statusCode = 200;
                    response.message = "Registro exitoso";
                }
                response.responseText = JsonSerializer.Serialize(new_product);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public Response GetProducts()
        {
            try
            {
                Response response = new Response();
                ProductDAL ProductDal = new ProductDAL(_Config);
                List<ProductSearch> list_products = ProductDal.getProductsDAL();

                if (list_products.Count == 0)
                {
                    response.statusCode = 404;
                    response.message = "No existen productos creados";
                    return response;
                }
                
                response.statusCode = 200;
                response.message = "Se encontraron productos";
                response.responseText = JsonSerializer.Serialize(new
                {
                    _Count = list_products.Count,
                    Products = list_products
                });
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{id}")]
        public Response GetProductsById(string id)
        {
            try
            {
                Response response = new Response();
                ProductDAL ProductDal = new ProductDAL(_Config);
                Product product = ProductDal.getProductIdDAL(new ObjectId(id));

                if (product == null)
                {
                    response.statusCode = 404;
                    response.message = "No existen productos creados";
                    return response;
                }
                
                response.statusCode = 200;
                response.message = "Se encontro el producto";
                response.responseText = JsonSerializer.Serialize(product);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("{id}")]
        public Response UpdateProduct(string id, [FromBody] Product product)
        {
            try
            {
                //TODO: VALIDAR SI EL MONGOID DE PRODUCTO Y CATEGORIA ES VALIDO
                ProductDAL action_products = new ProductDAL(_Config);
                Response reponse = action_products.updateProduct(id, product);
                return reponse;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        
        [HttpPut("delete{id}")]
        public Response DeleteProduct(string id)
        {
            try
            {
                ProductDAL action_products = new ProductDAL(_Config);
                Response reponse = action_products.DeleteProduct(id);
                return reponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
