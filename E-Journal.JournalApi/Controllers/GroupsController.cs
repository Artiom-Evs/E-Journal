using E_Journal.JournalApi.Models;
using E_Journal.JournalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Journal.JournalApi.Controllers
{
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
        public ActionResult<Group> Get(int id)
        {
            Group? group = _repostory.Get(id);

            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        // POST api/Groups
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Group))]
        public ActionResult<Group> Post([FromBody] Group group)
        {
            if (group.Id != 0 || string.IsNullOrWhiteSpace(group.Name))
            {
                return BadRequest();
            }

            var storedGroup = _repostory.Create(group.Name);

            if (storedGroup == null)
            {
                return Conflict();
            }

            return CreatedAtAction(nameof(Get), new { Id = storedGroup.Id }, storedGroup);
        }

        // PUT api/Groups/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult Put(int id, [FromBody] Group group)
        {
            if (id != group.Id)
            {
                return BadRequest();
            }

            if (!_repostory.IsExists(id))
            {
                return NotFound();
            }

            _repostory.Update(group);

            return NoContent();
        }

        // DELETE api/Groups/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult Delete(int id)
        {
            if (_repostory.IsExists(id))
            {
                return NotFound();
            }

            _repostory.Delete(id);

            return NoContent();
        }
    }
}
