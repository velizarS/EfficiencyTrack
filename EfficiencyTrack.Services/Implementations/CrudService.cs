using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class CrudService<T> : ICrudService<T> where T : BaseEntity
{
    protected readonly EfficiencyTrackDbContext _context;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    public CrudService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>()
                             .AsNoTracking()
                             .Where(e => !e.IsDeleted)
                             .OrderByDescending(e => e.CreatedOn)
                             .ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>()
                             .AsNoTracking()
                             .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        SetAuditFields(entity, isNew: true);

        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        SetAuditFields(entity, isNew: false);

        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null) return false;

        entity.IsDeleted = true;
        entity.DeletedBy = GetCurrentUserName();

        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        string? sortBy = null,
        bool sortAsc = true,
        int pageNumber = 1,
        int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        IQueryable<T> query = _context.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);

        query = ApplySearch(query, searchTerm);
        query = ApplySorting(query, sortBy, sortAsc);

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    protected virtual IQueryable<T> ApplySearch(IQueryable<T> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.Trim();
        var property = typeof(T).GetProperty("Name");
        if (property == null || property.PropertyType != typeof(string))
            return query;

        return query.Where(e => EF.Functions.Like(EF.Property<string>(e, "Name"), $"%{searchTerm}%"));
    }

    protected virtual IQueryable<T> ApplySorting(IQueryable<T> query, string? sortBy, bool sortAsc)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(e => e.CreatedOn);

        try
        {
            return sortAsc
                ? query.OrderBy(e => EF.Property<object>(e, sortBy))
                : query.OrderByDescending(e => EF.Property<object>(e, sortBy));
        }
        catch
        {
            return query.OrderByDescending(e => e.CreatedOn);
        }
    }

    private void SetAuditFields(T entity, bool isNew)
    {
        var now = DateTime.UtcNow;
        var user = GetCurrentUserName();

        if (isNew)
        {
            entity.CreatedOn = now;
            entity.CreatedBy = user;
        }
        else
        {
            entity.ModifiedOn = now;
            entity.ModifiedBy = user;
        }
    }

    private string GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
    }
}
