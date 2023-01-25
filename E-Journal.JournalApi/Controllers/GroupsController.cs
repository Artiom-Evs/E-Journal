using E_Journal.JournalApi.Models;
using E_Journal.JournalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupsController : ControllerBase
{
    private readonly IBaseRepository<Group> _repostory;

    public GroupsController(IBaseRepository<Group> repostory)
    {
        _repostory = repostory;
    }

    // GET: api/Groups
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Group[]))]
    public async Task<ActionResult<IEnumerable<Group>>> GetAsync()
    {
        return await _repostory.Items.ToListAsync();
    }

    // GET api/Groups/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Group))]
    public async Task<IActionResult> GetAsync(int id)
    {
        Group? group = await _repostory.GetAsync(id);

        if (group == null)
        {
            return NotFound();
        }

        return Ok(group);
    }

    // POST api/Groups
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Group))]
    public async Task<IActionResult> PostAsync([FromBody] Group group)
    {
        if (group.Id != 0 || string.IsNullOrWhiteSpace(group.Name))
        {
            return BadRequest();
        }

        var storedGroup = await _repostory.CreateAsync(group);

        return Created($"/api/groups/{storedGroup.Id}", storedGroup);
    }

    // PUT api/Groups/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PutAsync(int id, [FromBody] Group group)
    {
        if (id != group.Id)
        {
            return BadRequest();
        }

        var updatedGroup = await _repostory.UpdateAsync(group);

        if (updatedGroup == null)
        {
            return NotFound();
        }

        return Ok();
    }

    // DELETE api/Groups/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _repostory.IsExistsAsync(id))
        {
            return NotFound();
        }

        await _repostory.DeleteAsync(id);

        return NoContent();
    }
}
