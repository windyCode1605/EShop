using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost("image")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "products")
    {
        try
        {
            // 1. Validate File
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "File không hợp lệ." });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { Message = "File quá lớn (tối đa 5MB)." });

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest(new { Message = "Chỉ chấp nhận định dạng JPG, PNG, WEBP." });

            // 2. Tạo đường dẫn thư mục lưu trữ (wwwroot/uploads/folder)
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads", folder);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 3. Tạo tên file ngẫu nhiên
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // 4. Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 5. TRẢ VỀ RELATIVE PATH THAY VÌ ABSOLUTE URL
            // Bỏ Request.Scheme, Request.Host để DB không bị gắn chặt với domain hiện tại (Render, Localhost...)
            var relativePath = $"/uploads/{folder}/{fileName}";

            return Ok(new { Url = relativePath });
        }
        catch (Exception ex)
        {
            // Trong thực tế, nên log lỗi qua hệ thống logging có cấu trúc thay vì trả thẳng chi tiết lỗi ra ngoài
            return StatusCode(500, new { Message = $"Lỗi server: {ex.Message}" });
        }
    }
}