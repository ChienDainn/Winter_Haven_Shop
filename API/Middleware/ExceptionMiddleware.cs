using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    // Middleware này dùng để xử lý lỗi toàn cục trong ứng dụng ASP.NET Core
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next; // Đại diện cho request tiếp theo trong pipeline
        private readonly ILogger<ExceptionMiddleware> _logger; // Logger để ghi log lỗi
        private readonly IHostEnvironment _env; // Môi trường chạy ứng dụng (Development, Production, v.v.)

        // Constructor nhận vào các dependency cần thiết
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        // Phương thức xử lý request và bắt lỗi nếu có
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Tiếp tục pipeline nếu không có lỗi
                await _next(context);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi ra console hoặc file log
                _logger.LogError(ex, ex.Message);

                // Thiết lập kiểu dữ liệu trả về là JSON
                context.Response.ContentType = "application/json";

                // Thiết lập mã lỗi HTTP 500 (Internal Server Error)
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Nếu đang ở môi trường Development, trả về thông tin chi tiết của lỗi
                // Nếu ở môi trường Production, chỉ trả về lỗi chung chung để tránh lộ thông tin nhạy cảm
                var response = _env.IsDevelopment()
                    ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    : new ApiException((int)HttpStatusCode.InternalServerError);

                // Tùy chỉnh JSON để sử dụng camelCase (chuẩn JSON phổ biến)
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                // Chuyển đổi object response thành chuỗi JSON
                var json = JsonSerializer.Serialize(response, options);

                // Ghi JSON vào response để trả về client
                await context.Response.WriteAsync(json);
            }
        }
    }
}
