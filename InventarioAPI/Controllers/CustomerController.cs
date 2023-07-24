using InventarioAPI.DAL_s;
using InventarioAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _Config;

        public CustomerController(IConfiguration iconfig)
        {
            _Config = iconfig;
        }

        // GET: api/<CustomerController>
        [HttpPost]
        public Response CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                CustomerDAL action_customer = new CustomerDAL(_Config);
                Response response = action_customer.createCustomerDAL(customer);
                return response;
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        // GET api/<CustomerController>/5
        [HttpGet]
        public Response GetCustomers()
        {
            try
            {
                CustomerDAL action_customer = new CustomerDAL(_Config);
                Response response = new Response();
                List<Customer> list_customer = action_customer.getCustomersDAL();
                if(list_customer.Count > 0)
                {
                    response.statusCode = 200;
                    response.message = "Se encontraron customers";
                    response.responseText = JsonSerializer.Serialize(new
                    {
                        _Count = list_customer.Count,
                        Customers = list_customer
                    });
                }
                else
                {
                    response.statusCode = 404;
                    response.message = "No existen registros";
                }

                return response; 

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{id}")]
        public Response GetCustomerById(string id)
        {
            try
            {
                Response response = new Response();
                CustomerDAL action_customer = new CustomerDAL(_Config);
                Customer customer = action_customer.getCustomerIdDAL(new ObjectId(id));
                if(customer == null)
                {
                    response.statusCode = 404;
                    response.message = "No se encontro ningún customer con el id";
                }
                else
                {
                    response.statusCode = 200;
                    response.message = "Se encontro un customer";
                    response.responseText = JsonSerializer.Serialize(customer);
                }

                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public Response updateCustomer(string id, [FromBody] Customer customer)
        {
            try
            {
                Response response = new Response();
                CustomerDAL action_customer = new CustomerDAL(_Config);
                response = action_customer.updateCustomerDAL(id, customer);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE api/<CustomerController>/5
        [HttpPut("delete/{id}")]
        public Response deleteCustomer(string id)
        {
            try
            {
                Response response = new Response();
                CustomerDAL action_customer = new CustomerDAL(_Config);
                response = action_customer.deleteCustomerDAL(id);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
