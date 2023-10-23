using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyStoreApi.Data;
using MyStoreApi.Models;

namespace MyStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly List<string> listSubject = new List<string>()
        {
            "Order Status", "Refund Requested", "Job Application","Other"
        };
        [HttpGet("subjects")]
        public IActionResult GetSubject()
        {
            return Ok(listSubject);
        }
        public ContactsController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public IActionResult GetContacts(int? page)
        {
            if(page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPage = 0;

            decimal count = _context.Contacts.Count();

            totalPage = (int)Math.Ceiling(count/ pageSize);

            var contacts = _context.Contacts.
                OrderByDescending(c => c.Id)
                .Skip((int) (page -1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                Contacts = contacts,
                TotalPages = totalPage,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);
        }

        [HttpGet("{id}")] 
        public IActionResult GetContact(int id)
        {
            var contact = _context.Contacts.Find(id);
            if(contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }
        [HttpPost]
        public IActionResult CreateContact(ContactDto contactDto)
        {
            if(!listSubject.Contains(contactDto.Subject))
            {
                ModelState.AddModelError("Subject", "Please select a Valid Subject");
                return BadRequest(ModelState);
            }
            Contact contact = new Contact()
            {
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Email = contactDto.Email,
                Phone = contactDto.Phone ?? "",
                Subject = contactDto.Subject,
                Messege = contactDto.Messege,
                CreatedAt = DateTime.Now
            };
            _context.Contacts.Add(contact);
            _context.SaveChanges();
            return Ok(contact);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id,  ContactDto contactDto)
        {
            if (!listSubject.Contains(contactDto.Subject))
            {
                ModelState.AddModelError("Subject", "Please select a Valid Subject");
                return BadRequest(ModelState);
            }
            var contact = _context.Contacts.Find(id);
            if(contact == null)
            {
                return NotFound();
            }

            contact.FirstName = contactDto.FirstName;
            contact.LastName = contactDto.LastName;
            contact.Email = contactDto.Email;
            contact.Phone = contactDto.Phone ?? "";
            contact.Subject = contactDto.Subject;
            contact.Messege = contactDto.Messege;

            _context.SaveChanges();
            return Ok(contact);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            try
            {
                var contact = new Contact() { Id = id };
                _context.Contacts.Remove(contact);
                _context.SaveChanges();
            }
            catch(Exception)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
