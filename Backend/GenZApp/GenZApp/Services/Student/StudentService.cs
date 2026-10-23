using GenZApp.Models;
using GenZApp.Repository.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenZApp.Services.Student
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        // GET ALL STUDENTS
        public async Task<IEnumerable<StudentResponseDto>> GetAllAsync()
        {
            var students = await _repository.GetAllAsync();

            return students.Select(x => new StudentResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Age = x.Age,
                Address = x.Address
            }).ToList();
        }

        // GET STUDENT BY ID
        public async Task<StudentResponseDto?> GetByIdAsync(int id)
        {
            var student = await _repository.GetByIdAsync(id);

            if (student == null)
                return null;

            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Age = student.Age,
                Address = student.Address
            };
        }

        // CREATE STUDENT
        public async Task<StudentResponseDto> CreateAsync(StudentCreateDto dto)
        {
            var result = await _repository.CreateAsync(dto);
            return new StudentResponseDto
            {
                Id = result.Id,
                Name = result.Name,
                Email = result.Email,
                Age = result.Age,
                Address = result.Address
            };
        }

        // UPDATE STUDENT
        public async Task<bool> UpdateAsync(int id, StudentUpdateDto dto)
        {
            var student = await _repository.GetByIdAsync(id);

            if (student == null)
                return false;

            return await _repository.UpdateAsync(id, dto);
        }

        // DELETE STUDENT
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }

}
