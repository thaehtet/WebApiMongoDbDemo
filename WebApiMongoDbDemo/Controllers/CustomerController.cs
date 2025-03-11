using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApiMongoDbDemo.Data;
using WebApiMongoDbDemo.Entity;

namespace WebApiMongoDbDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMongoCollection<Customer> _customers;
        public CustomerController(MongoDbService mongoDbService)
        {
            _customers = mongoDbService.database?.GetCollection<Customer>("customer");

        }
        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            return await _customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer?>> GetById(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(x => x.Id, id);
            var customer = await _customers.Find(filter).FirstOrDefaultAsync();
            return customer is null ? Ok(customer) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Customer customer)
        {
            await _customers.InsertOneAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        [HttpPut]
        public async Task<ActionResult> Update(Customer customer)
        {
            var filter = Builders<Customer>.Filter.Eq(x => x.Id,customer.Id);
            //var update = Builders<Customer>.Update
            //    .Set(x => x.CustomerName, customer.CustomerName)
            //    .Set(x => x.Email, customer.Email);
            //await _customers.UpdateOneAsync(filter, update);
            await _customers.ReplaceOneAsync(filter, customer);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Customer>.Filter.Eq(x => x.Id, id);
            await _customers.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
