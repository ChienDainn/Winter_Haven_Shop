using System.Linq.Expressions;

namespace Core.Specifications
{
    // Lớp cơ sở để xây dựng các tiêu chí truy vấn động
    public class BaseSpecification<T> : ISpecification<T>
    {
        // Constructor mặc định, không áp dụng tiêu chí lọc nào
        public BaseSpecification()
        {
        }

        // Constructor nhận vào một biểu thức lambda để làm điều kiện lọc
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria; // Gán tiêu chí lọc vào thuộc tính Criteria
        }

        // Thuộc tính Criteria lưu trữ điều kiện lọc (WHERE)
        public Expression<Func<T, bool>> Criteria { get; }

        // Danh sách các điều kiện Include để tải dữ liệu liên quan
        public List<Expression<Func<T, object>>> Includes { get; } =
            new List<Expression<Func<T, object>>>(); // Khởi tạo danh sách rỗng

        // Điều kiện sắp xếp tăng dần (ORDER BY)
        public Expression<Func<T, object>> OrderBy { get; private set; }

        // Điều kiện sắp xếp giảm dần (ORDER BY DESC)
        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        // Số lượng bản ghi tối đa cần lấy (Take dùng cho phân trang)
        public int Take { get; private set; }
        // Cờ kiểm tra xem có áp dụng phân trang không

        public bool IsPagingEnabled { get; private set; }

        // Số lượng bản ghi cần bỏ qua (Skip dùng cho phân trang)
        public int Skip { get; private set; }







        // Thêm điều kiện Include để truy vấn dữ liệu liên quan
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression); // Thêm điều kiện Include vào danh sách
        }

        // Thiết lập sắp xếp tăng dần
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression; // Gán biểu thức sắp xếp tăng dần
        }

        // Thiết lập sắp xếp giảm dần
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression; // Gán biểu thức sắp xếp giảm dần
        }

        // Áp dụng phân trang (Skip và Take)
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip; // Gán số lượng bản ghi cần bỏ qua
            Take = take; // Gán số lượng bản ghi cần lấy
            IsPagingEnabled = true;
        }
    }
}

//  BaseSpecification<T> trong C# giúp xây dựng các tiêu chí truy vấn động cho Specification Pattern. Nó được sử dụng để định nghĩa các điều kiện lọc, sắp xếp, phân trang và bao gồm dữ liệu liên quan khi truy vấn từ database bằng Entity Framework (EF).