using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopDev.Common.Filters;
using ShopDev.S3Bucket;
using ShopDev.Utils.Net.Request;
using ShopDev.WebAPIBase;
using ShopDev.WebAPIBase.Controllers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopDev.Authentication.API.Controllers
{
    //[Authorize]
    [Route("api/auth/value")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ValuesController : BaseController
    {
        private readonly IS3ManagerFile _s3ManagerFile;
        public ValuesController(IS3ManagerFile s3ManagerFile) 
        {
            _s3ManagerFile = s3ManagerFile;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var result = HttpContext
                .AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme)
                .Result;
            var test = HttpContext;
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        [PermissionFilter]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost("upfile")]
        [AllowAnonymous]
        public async Task<ApiResponse> Post(params IFormFile[] input)
        {
            return new(await _s3ManagerFile.UploadAsync(input));
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value) { }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) { }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id) { }
    }
}
