using Microsoft.AspNetCore.Mvc;
using Distributed_Cache.Services;
using Distributed_Cache.Database;
using Distributed_Cache.Models;
using Microsoft.EntityFrameworkCore;

namespace Distributed_Cache.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        protected readonly ILogger<UserController> _logger;
        protected readonly ICacheService _cacheService;

        protected readonly AppDbContext _context;

        public UserController(ILogger<UserController> logger, ICacheService cacheService, AppDbContext context)
        {
            _logger = logger;
            _cacheService = cacheService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllusers()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Users>>("users");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = await _context.Users.ToListAsync();

            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<IEnumerable<Users>>("users", cacheData, expirationTime);

            return Ok(cacheData);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(Users value)
        {
            var newUser = await _context.Users.AddAsync(value);
            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<Users>($"users{value.Id}", newUser.Entity, expirationTime);

            await _context.SaveChangesAsync();

            return Ok(newUser.Entity);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var exist = _context.Users.FirstOrDefault(x => x.Id.Equals(id));

            if (exist != null)
            {
                _context.Remove(exist);
                _cacheService.RemoveData($"users{id}");
                await _context.SaveChangesAsync();

                return NoContent();
            }

            return NotFound();
        }
    }
}