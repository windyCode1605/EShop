using System.Data;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace eShop.API.Services.Shared;
public class FirebaseStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;
    public FirebaseStorageService(IConfiguration configuration, IWebHostEnvironment env)
    {
        _bucketName = configuration["Firebase:StorageBucket"]
                        ?? throw new ArgumentNullException("Thiếu cấu hình Firebase:StorageBucket");
        string credentialPath = Path.Combine(env.ContentRootPath,
                        configuration["Firebase:ServiceAccountKeyPath"]);
        var credential = GoogleCredential.FromFile(credentialPath);
        _storageClient = StorageClient.Create(credential);
    }
    public async Task<string> UploadFileAsync(IFormFile file, string folderName = "products")
    {
        if (file == null || file.Length == 0) return string.Empty;
        var fileName = $"{folderName}/{Guid.NewGuid()}_{file.FileName}";

        using var memorystream = new MemoryStream();
        await file.CopyToAsync(memorystream);
        var bytes = memorystream.ToArray();

        await _storageClient.UploadObjectAsync(
            bucket: _bucketName,
            objectName: fileName,
            contentType: file.ContentType,
            source: new MemoryStream(bytes)
        );

        // trả về URL public chuẩn của Firebase storage để Angular có thể đọc được
        return
        $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(fileName)}?alt=media";
    }
}