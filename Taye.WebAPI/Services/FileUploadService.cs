namespace Taye.WebAPI.Services;

public interface IFileUploadService
{
    Task<string?> SaveImageAsync(IFormFile file, string subFolder = "images");
    void DeleteImage(string imagePath);
}

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadService> _logger;

    // 允许的图片格式
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

    public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string?> SaveImageAsync(IFormFile file, string subFolder = "images")
    {
        if (file == null || file.Length == 0)
            return null;

        // 验证文件大小
        if (file.Length > _maxFileSize)
        {
            _logger.LogWarning("文件太大: {Size} bytes", file.Length);
            throw new InvalidOperationException($"文件大小不能超过 {_maxFileSize / 1024 / 1024}MB");
        }

        // 验证文件扩展名
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("不支持的文件格式: {Extension}", extension);
            throw new InvalidOperationException($"不支持的文件格式，仅支持: {string.Join(", ", _allowedExtensions)}");
        }

        try
        {
            // 创建上传目录
            var uploadPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", subFolder);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 生成唯一文件名
            var fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);
            var relativePath = Path.Combine(subFolder, fileName).Replace("\\", "/");

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("图片保存成功: {Path}", relativePath);
            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存图片失败");
            throw;
        }
    }

    public void DeleteImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return;

        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", imagePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("图片删除成功: {Path}", imagePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除图片失败: {Path}", imagePath);
        }
    }
}
