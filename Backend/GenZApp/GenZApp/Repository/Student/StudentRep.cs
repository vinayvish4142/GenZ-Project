using GenZApp.Models;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Infrastructure.Data;

namespace GenZApp.Repository.Student
{
    public class StudentRep : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRep(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL STUDENTS
        public async Task<IEnumerable<StudentModel>> GetAllAsync()
        {
            return await _context.Students.AsNoTracking().ToListAsync();
        }

        // GET STUDENT BY ID
        public async Task<StudentModel> GetByIdAsync(int id)
        {
            return await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
        }

        // CREATE STUDENT
        public async Task<StudentModel> CreateAsync(StudentModel student)
        {
            await _context.Students.AddAsync(student);

            await _context.SaveChangesAsync();

            return student;
        }

        // UPDATE STUDENT
        public async Task<bool> UpdateAsync(StudentModel student)
        {
            _context.Students.Update(student);

            return await _context.SaveChangesAsync() > 0;
        }

        // DELETE STUDENT
        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students
                .FindAsync(id);

            if (student == null)
                return false;

            _context.Students.Remove(student);

            return await _context.SaveChangesAsync() > 0;
        }

        Task<IEnumerable<StudentResponseDto>> IStudentRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<StudentResponseDto?> IStudentRepository.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<StudentResponseDto> CreateAsync(StudentCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, StudentUpdateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}

