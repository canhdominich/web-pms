using TLighthouse.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace TLighthouse.Api.Infrastructure;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected const long MaxUploadBytes = 5 * 1024 * 1024;

    protected FileContentResult Excel(FileContent file) => File(file.Content, FileContent.ExcelContentType, file.FileName);

    protected IActionResult Import(ImportResult result) => result.Succeeded ? Ok(result) : UnprocessableEntity(result);
}
