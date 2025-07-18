﻿using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
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

    public virtual async Task AddAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        entity.CreatedOn = DateTime.UtcNow;
        entity.CreatedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        _ = _context.Set<T>().Add(entity);
        _ = await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        _ = _context.Set<T>().Update(entity);
        _ = await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        T? entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            _ = await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm, string? sortBy, bool sortAsc, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        IQueryable<T> query = _context.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e => EF.Property<string>(e, "Name").Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            try
            {
                query = sortAsc
                    ? query.OrderBy(e => EF.Property<object>(e, sortBy))
                    : query.OrderByDescending(e => EF.Property<object>(e, sortBy));
            }
            catch
            {
                query = query.OrderByDescending(e => e.CreatedOn);
            }
        }
        else
        {
            query = query.OrderByDescending(e => e.CreatedOn);
        }

        int totalCount = await query.CountAsync();

        List<T> items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
