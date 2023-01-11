using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly StoreContext _context;
    public BuggyController(StoreContext context) 
    { 
        _context = context;
    }

    [HttpGet("notfound")]
    public ActionResult GetNotFoundRequest()
    {
        var result = this._context.Products.Find(42);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("servererror")]
    public ActionResult GetServerError()
    {
        var result = this._context.Products.Find(42);

        var resultToReturn = result.ToString();

        return Ok();
    }

    [HttpGet("badrequest")]
    public ActionResult GetBadRequest()
    {
        return BadRequest();
    }

    [HttpGet("badrequest/{id}")]
    public ActionResult GetNotFoundRequest(int id)
    {
        return Ok();
    }
}
