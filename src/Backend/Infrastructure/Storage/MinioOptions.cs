namespace Infrastructure.Storage;

public class MinioOptions
{
    public string Endpoint { get; set; } = "minio:9000";
    public string AccessKey { get; set; } = "minioadmin";
    public string SecretKey { get; set; } = "minioadmin";
    public string BucketName { get; set; } = "pdf-templates";
    public bool UseSsl { get; set; }
}
