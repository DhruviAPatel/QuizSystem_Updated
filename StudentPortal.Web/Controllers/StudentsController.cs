using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;

namespace StudentPortal.Web.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class StudentsController : Controller
    {

        private readonly ApplicationDbContext dbContext;
        public StudentsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Add(AddStudentViewModel viewModel)
        {
            // Check for duplicate email
            var existingStudent = await dbContext.Students.FirstOrDefaultAsync(s => s.Email == viewModel.Email);
            if (existingStudent != null)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel); // Pass the model back to the view
            }

            var student = new Student
            {
                Name = viewModel.Name,
                Email = viewModel.Email,
                Phone = viewModel.Phone,
                subscribed = viewModel.subscribed
            };

            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List", "Students");
        }


        //public  async Task<IActionResult> Add(AddStudentViewModel viewModel)
        //{
        //    var existingStudent= await dbContext.Students.FirstOrDefaultAsync(s=>s.Email==viewModel.Email);
        //    if (existingStudent !=null)
        //    {
        //        ModelState.AddModelError("Email", "This email is already registered");
        //        return View(viewModel);
        //    }


        //    //if (string.IsNullOrWhiteSpace(viewModel.Phone) || !System.Text.RegularExpressions.Regex.IsMatch(viewModel.Phone, @"^\d{10}$"))
        //    //{
        //    //    ModelState.AddModelError("Phone", "The phone number must contain exactly 10 digits.");
        //    //    return View(viewModel);
        //    //}

        //    var student = new Student
        //    {
        //        Name = viewModel.Name,
        //        Email = viewModel.Email,
        //        Phone = viewModel.Phone,
        //        subscribed = viewModel.subscribed
        //    };

        //    if (ModelState.IsValid)
        //    {
        //        await dbContext.Students.AddAsync(student);
        //        await dbContext.SaveChangesAsync();
        //        return RedirectToAction("List", "Students");
        //    }

        //    return View();
        //}

        [HttpGet]

        public async Task<IActionResult> List()
        {
            var students = await dbContext.Students.ToListAsync();

            return View(students);
        }

        [HttpGet]

        public async Task<IActionResult> Edit(int Id)
        {
            var student = await dbContext.Students.FindAsync(Id);
            return View(student);
        }

        [HttpPost]

        public async Task<IActionResult> Edit(Student viewModel) 
        {
            var student = await dbContext.Students.FindAsync(viewModel.ID);

            if (student is not null)
            {
                student.Name = viewModel.Name;
                student.Email = viewModel.Email;
                student.Phone = viewModel.Phone;
                student.subscribed = viewModel.subscribed;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List","Students");

        }

        [HttpPost]

        public async Task<IActionResult> Delete(Student viewModel)
        {
            var student = await dbContext.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == viewModel.ID);

            if (student is not null)
            { 
                dbContext.Students.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Students");
        }
    }
}
