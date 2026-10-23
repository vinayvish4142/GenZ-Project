using GenZApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenZApp.Repository.Student
{
    public interface IStudentRepository 
    {
        Task<IEnumerable<StudentResponseDto>> GetAllAsync();

        Task<StudentResponseDto?> GetByIdAsync(int id);

        Task<StudentResponseDto> CreateAsync(StudentCreateDto dto);

        Task<bool> UpdateAsync(int id, StudentUpdateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
