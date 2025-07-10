using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class CrudService<T> : ICrudService<T> where T : BaseEntity
{
    private readonly EfficiencyTrackDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CrudService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _context.Set<T>()
                         .AsNoTracking()
                         .Where(e => !e.IsDeleted)
                         .ToListAsync();

    public virtual async Task<T?> GetByIdAsync(Guid id)
        => await _context.Set<T>()
                         .AsNoTracking()
                         .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    public virtual async Task AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        entity.CreatedOn = DateTime.UtcNow;
        entity.CreatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            await _context.SaveChangesAsync();
        }
    }
}
