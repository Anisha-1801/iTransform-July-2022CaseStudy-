﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HavenInn_Library.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.EntityFrameworkCore.Internal;

namespace HavenInn_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    #region RoomsController
    public class RoomsController : ControllerBase
    {
        private readonly HavenInnContext _context;

        public RoomsController(HavenInnContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        [Authorize(Roles = "Receptionist,Manager,Owner")]
        #region Get all Rooms by RoomType API
        ///<summary>Get all Rooms by RoomType</summary> 
        public async Task<ActionResult<IEnumerable<Room>>> GetRoom()
        {
            try
            {
                //var rooms = _context.Room.Include("RoomType");
                return await _context.Room.Include(i => i.RoomType).ToListAsync();
            }
              catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Server error {e.Message}");
            }
        }
        #endregion

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Receptionist,Manager,Owner")]
        #region Get Room by ID API
        ///<summary>Get Room by ID</summary> 
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            try
            { 
            var room = await _context.Room.Where(r => r.RoomId == id).Include("RoomType").FirstAsync();

            if (room == null)
            {
                return NotFound();
            }

            return room;
            }
              catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Server error {e.Message}");
            }
        }
        #endregion

        [HttpGet("available")]
        [Authorize(Roles = "Receptionist,Manager,Owner")]
        #region Get all Available Rooms API
        ///<summary>Get all Availabe Rooms</summary> 
        public async Task<ActionResult<IEnumerable<Room>>> availablerooms()
        {
            IQueryable<Room> query = _context.Room;
            
            try
            {
                var rooms = await query.Where(r=>r.IsAvailable== true).ToListAsync();
                if (rooms == null)
                {
                    return NotFound();
                }

                return rooms;

            }
              catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Server error {e.Message}");
            }

        }
        #endregion


        [HttpGet("Search")]
        [Authorize(Roles = "Receptionist,Manager,Owner")]
        #region Search Room API
        ///<summary>Search Room</summary> 
        public async Task<ActionResult<Room>> SearchRoomByRoomType(string roomType,bool status)
        {
            IQueryable<Room> query = _context.Room;
            try
            {
                
                var rooms = await query.Include("RoomType").Where(room => room.RoomType.RoomTypeName == roomType)
                            .Where(room => room.IsAvailable == status).ToListAsync();
                if (rooms == null)
                {
                    return NotFound("Room not available! Try again later");
                }
                else
                    return Ok(rooms);
            }
              catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Server error {e.Message}");
            }

        }
        #endregion

        // PUT: api/Rooms/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [Authorize(Roles = "Receptionist,Manager,Owner")]
        #region Update Room by ID API
        ///<summary>Update Room by ID</summary> 
        public async Task<IActionResult> PutRoom(int id, Room room)
        {
            if (id != room.RoomId)
            {
                return BadRequest();
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
              catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        #endregion

        // POST: api/Rooms
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "Manager,Owner")]
        #region Add Room API
        ///<summary>Add Room</summary> 
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            try
            { 
            _context.Room.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = room.RoomId }, room);
            }
              catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Server error {e.Message}");
            }
        }
        #endregion

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Owner")]
        #region Delete Room by ID API
        ///<summary>Delete Room by ID</summary> 
        public async Task<ActionResult<Room>> DeleteRoom(int id)
        {
            try
            { 
            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Room.Remove(room);
            await _context.SaveChangesAsync();

            return room;
            }
              catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Server error {e.Message}");
            }
        }
        #endregion

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }
    }
    #endregion
}
