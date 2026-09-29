using SPApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using SPApi.Data;
using Microsoft.EntityFrameworkCore;


namespace SPApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudentsController(AppDbContext db)
    {
        _db = db;
    }

    private static StudentDto ToDto(Student student) =>
        new(student.Id, student.Name, student.Course);

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll(int? course) {
        IQueryable<Student> query = _db.Students;

        if (course.HasValue)
            query = query.Where(s => s.Course == course.Value);

        var result = await query
            .Select(s => new StudentDto(s.Id, s.Name, s.Course))
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async<ActionResult<StudentDto>> GetById(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student is null)
            return NotFound();

         return Ok(ToDto(student));
    }
    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create(CreateStudentDto request)
    {
        var newStudent = new Student{
            Name = request.Name,
            Course = request.Course
        };
        
        await _db.Students.AddAsync(newStudent);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new {id = newStudent.Id}, ToDto(newStudent));
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateStudentDto request)
    {
        var student = await _db.Students.FindAsync(id);

        if (student is null)
    }
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);

        if (student == null)
        {
            return NotFound();
        }
        Students.Remove(student);
        return NoContent();
    }
}



