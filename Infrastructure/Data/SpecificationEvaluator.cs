using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    // Lớp giúp áp dụng Specification Pattern vào truy vấn cơ sở dữ liệu
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        // Phương thức lấy truy vấn đã áp dụng các điều kiện của Specification
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
            ISpecification<TEntity> spec)
        {
            var query = inputQuery; // Bắt đầu với truy vấn gốc

            // Áp dụng điều kiện lọc (WHERE) nếu có
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }
            // Áp dụng điều kiện lọc (WHERE) nếu có
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            // Áp dụng Include để tải dữ liệu liên quan (bảng liên kết)
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query; // Trả về truy vấn đã áp dụng tất cả điều kiện
        }
    }
}

// SpecificationEvaluator<TEntity>:

// Đây là một class generic giúp áp dụng các điều kiện từ Specification Pattern vào truy vấn Entity Framework (EF Core).
// Chỉ áp dụng cho các thực thể kế thừa BaseEntity.
// Phương thức GetQuery:

// Nhận vào một truy vấn gốc (IQueryable<TEntity> inputQuery) và một đối tượng specification (ISpecification<TEntity> spec).
// Bước 1: Nếu spec.Criteria tồn tại, áp dụng điều kiện WHERE.
// Bước 2: Nếu spec.OrderBy tồn tại, áp dụng ORDER BY ASC.
// Bước 3: Nếu spec.OrderByDescending tồn tại, áp dụng ORDER BY DESC.
// Bước 4: Nếu spec.IsPagingEnabled được bật, áp dụng Skip và Take để phân trang.
// Bước 5: Áp dụng Include() để lấy dữ liệu liên quan từ các bảng khác.
// Cuối cùng, trả về truy vấn đã được xử lý.
