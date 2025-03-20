using Application.Features.Schools;
using Application.Features.Schools.Commands;
using Application.Features.Schools.Query;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(SchoolConstants.Create,SchoolFeature.Schools)]
        public async Task<IActionResult> CreateSchoolAsync([FromBody] CreateSchoolRequest request)
        {
            var response = await Sender.Send(new CreateSchoolCommand { CreateSchool = request});
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("update")]
        [ShouldHavePermission(SchoolConstants.Update, SchoolFeature.Schools)]
        public async Task<IActionResult> UpdateSchoolAsync([FromBody] UpdateSchoolRequest request)
        {
            var response = await Sender.Send(new UpdateSchoolCommand { MyProperty = request });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
        [HttpDelete("{schoolId}")]
        [ShouldHavePermission(SchoolConstants.Delete, SchoolFeature.Schools)]
        public async Task<IActionResult> DeleteSchoolAsync(int schoolId )
        {
            var response = await Sender.Send(new DeleteSchoolCommand { SchoolId = schoolId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
        [HttpGet("by-id/{schoolId}")]
        [ShouldHavePermission(SchoolConstants.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetSchoolByIdAsync(int schoolId)
        {
            var response = await Sender.Send(new GetSchoolByIdQuery { SchoolId = schoolId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("by-name/{name}")]
        [ShouldHavePermission(SchoolConstants.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetSchoolByNameAsync(string name)
        {
            var response = await Sender.Send(new GetSchoolByNameQuery { SchoolName = name });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpGet("all")]
        [ShouldHavePermission(SchoolConstants.Read, SchoolFeature.Schools)]
        public async Task<IActionResult> GetAllSchoolsAsync()
        {
            var response = await Sender.Send(new GetSchoolQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return NotFound(response);
        }
    }
}
