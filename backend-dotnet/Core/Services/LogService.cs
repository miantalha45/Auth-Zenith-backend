using backend_dotnet.Core.DbContext;
using backend_dotnet.Core.Dtos.Log;
using backend_dotnet.Core.Entities;
using backend_dotnet.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_dotnet.Core.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveNewLog(string UserName, string Description)
        {
            var newLog = new Log()
            {
                UserName = UserName,
                Description = Description
            };

            await _context.Logs.AddAsync(newLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetLogDto>> GetLogAsync()
        {
            var Logs = await _context.Logs
                .Select(q => new GetLogDto
                {
                    CreatedAt = q.CreatedAt,
                    Description = q.Description,
                    UserName = q.UserName,
                })
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
            return Logs;
        }

        public async Task<IEnumerable<GetLogDto>> GetMyLogAsync(ClaimsPrincipal User)
        {
            var Logs = await _context.Logs
                .Where(q => q.UserName == User.Identity.Name)
                .Select(q => new GetLogDto
                {
                    CreatedAt = q.CreatedAt,
                    Description = q.Description,
                    UserName = q.UserName,
                })
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
            return Logs;
        }
    }
}
