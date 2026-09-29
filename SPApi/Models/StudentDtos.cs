using System.ComponentModel.DataAnnotations;
namespace SPApi.Models;

public record StudentDto(int ID, string Name, int Course);

public record CreateStudentDto(
    [Required] string Name,
    [Range(1,4)] int Course);

public record UpdateStudentDto(
    [Required] string Name,
    [Range(1, 4)] int Course);

    