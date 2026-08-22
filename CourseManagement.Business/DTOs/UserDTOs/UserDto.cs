using CourseManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.UserDTOs;

public class UserDto
{
    required public string EmailAddress { get; set; } 
    required public string FullName { get; set; }
}
