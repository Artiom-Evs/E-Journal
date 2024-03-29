﻿using E_Journal.JournalApi.Models;
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
        public async Task<ActionResult<Group>> GetAsync(int id)
        {
            Group? group = await _repostory.GetAsync(id);

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
        public async Task<ActionResult<Group>> PostAsync([FromBody] Group group)
        {
            if (group.Id != 0 || string.IsNullOrWhiteSpace(group.Name))
            {
                return BadRequest();
            }

            var storedGroup = await _repostory.CreateAsync(group.Name);

            if (storedGroup == null)
            {
                return Conflict();
            }

            return CreatedAtAction(nameof(GetAsync), new { Id = storedGroup.Id }, storedGroup);
        }

        // PUT api/Groups/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PutAsync(int id, [FromBody] Group group)
        {
            if (id != group.Id)
            {
                return BadRequest();
            }

            if (!await _repostory.IsExistsAsync(id))
            {
                return NotFound();
            }

            await _repostory.UpdateAsync(group);

            return NoContent();
        }

        // DELETE api/Groups/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(int id)
        {
            if (await _repostory.IsExistsAsync(id))
            {
                return NotFound();
            }

            _repostory.DeleteAsync(id);

            return NoContent();
        }
    }
}
