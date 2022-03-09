﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostsAPI.Data;
using PostsAPI.Models;

namespace PostsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly SnackisContext _context;

        public PostsController(SnackisContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _context.Post.Include(p => p.Images).ToListAsync();

            return posts;
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            //var post = await _context.Post.Include(p => p.Images).Include(p => p.Posts).FirstOrDefaultAsync(p => p.Id == id); ////Den här tar inte med all nivåer av svar till appen

            var posts = await _context.Post.Include(p => p.Images).ToListAsync();
            var post = posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }


        // GET: api/Posts/parentposts/2
        [HttpGet("answers/{parentId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts(int parentId)
        {
            List<Post> answers = await _context.Post.Include(p => p.Images).Where(p => p.PostId == parentId).ToListAsync();
            return answers;
        }


        // GET: api/Posts/subjectname/name
        [HttpGet("subjectname/name")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts(string name)
        {
            var subject = await _context.Subject.FirstOrDefaultAsync(s => s.Name == name);

            if (subject == null)
            {
                return NotFound();
            }

            List<Post> posts = await _context.Post.Include(p => p.Images).Where(p => p.SubjectId == subject.Id).ToListAsync();
            return posts;
        }


        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            _context.Post.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Post>> DeletePost(int id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            return post;
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
