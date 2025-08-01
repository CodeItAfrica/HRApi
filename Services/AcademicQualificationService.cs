using HRApi.Data;
using HRApi.Models;
using HRApi.Services;
using Microsoft.EntityFrameworkCore;

namespace HRApi.Services
{
    public class AcademicQualificationService
    {
        private readonly AppDbContext _context;

        public AcademicQualificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AcademicQualificationResponseDto>> GetAllAsync()
        {
            var qualifications = await _context
                .AcademicQualifications.Include(aq => aq.Employee)
                .ToListAsync();

            return qualifications.Select(MapToResponseDto);
        }

        public async Task<AcademicQualificationResponseDto?> GetByIdAsync(int id)
        {
            var qualification = await _context
                .AcademicQualifications.Include(aq => aq.Employee)
                .FirstOrDefaultAsync(aq => aq.Id == id);

            return qualification != null ? MapToResponseDto(qualification) : null;
        }

        public async Task<IEnumerable<AcademicQualificationResponseDto>> GetByEmployeeIdAsync(
            string employeeId
        )
        {
            var qualifications = await _context
                .AcademicQualifications.Include(aq => aq.Employee)
                .Where(aq => aq.EmployeeId == employeeId)
                .ToListAsync();

            return qualifications.Select(MapToResponseDto);
        }

        public async Task<AcademicQualificationResponseDto> CreateAsync(
            AcademicQualificationCreateDto dto
        )
        {
            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {dto.EmployeeId} not found.");
            }

            if (dto.YearOfCompletion < dto.YearStarted)
            {
                throw new ArgumentException("Year of completion cannot be before year started.");
            }

            var qualification = new AcademicQualification
            {
                EmployeeId = dto.EmployeeId,
                QualificationName = dto.QualificationName,
                YearStarted = dto.YearStarted,
                YearOfCompletion = dto.YearOfCompletion,
                InstitutionName = dto.InstitutionName,
                CertificateId = dto.CertificateId,
                Description = dto.Description,
                Photo = dto.Photo,
                CreatedAt = DateTime.UtcNow,
            };

            _context.AcademicQualifications.Add(qualification);
            await _context.SaveChangesAsync();

            await _context.Entry(qualification).Reference(aq => aq.Employee).LoadAsync();

            return MapToResponseDto(qualification);
        }

        public async Task<AcademicQualificationResponseDto?> UpdateAsync(
            int id,
            AcademicQualificationUpdateDto dto
        )
        {
            var qualification = await _context
                .AcademicQualifications.Include(aq => aq.Employee)
                .FirstOrDefaultAsync(aq => aq.Id == id);

            if (qualification == null)
            {
                return null;
            }

            if (dto.YearOfCompletion < dto.YearStarted)
            {
                throw new ArgumentException("Year of completion cannot be before year started.");
            }

            qualification.QualificationName = dto.QualificationName;
            qualification.YearStarted = dto.YearStarted;
            qualification.YearOfCompletion = dto.YearOfCompletion;
            qualification.InstitutionName = dto.InstitutionName;
            qualification.CertificateId = dto.CertificateId;
            qualification.Description = dto.Description;
            qualification.Photo = dto.Photo;

            await _context.SaveChangesAsync();

            return MapToResponseDto(qualification);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var qualification = await _context.AcademicQualifications.FindAsync(id);
            if (qualification == null)
            {
                return false;
            }

            _context.AcademicQualifications.Remove(qualification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AcademicQualifications.AnyAsync(aq => aq.Id == id);
        }

        private static AcademicQualificationResponseDto MapToResponseDto(
            AcademicQualification qualification
        )
        {
            return new AcademicQualificationResponseDto
            {
                Id = qualification.Id,
                EmployeeId = qualification.EmployeeId,
                QualificationName = qualification.QualificationName,
                YearStarted = qualification.YearStarted,
                YearOfCompletion = qualification.YearOfCompletion,
                InstitutionName = qualification.InstitutionName,
                CertificateId = qualification.CertificateId,
                Description = qualification.Description,
                Photo = qualification.Photo,
                CreatedAt = qualification.CreatedAt,
            };
        }
    }
}
