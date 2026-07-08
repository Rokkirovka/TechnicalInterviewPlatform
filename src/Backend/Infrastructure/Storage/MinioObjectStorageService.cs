using Application.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Storage;

public class MinioObjectStorageService(
    IMinioClient minioClient,
    IOptions<MinioOptions> options)
    : IObjectStorageService
{
    private readonly MinioOptions _options = options.Value;

    public async Task PutAsync(
        string objectName,
        Stream content,
        long size,
        string contentType,
        CancellationToken ct)
    {
        await EnsureBucketExistsAsync(ct);

        var args = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName)
            .WithStreamData(content)
            .WithObjectSize(size)
            .WithContentType(contentType);

        await minioClient.PutObjectAsync(args, ct);
    }

    public async Task<Stream> GetAsync(string objectName, CancellationToken ct)
    {
        var output = new MemoryStream();
        var args = new GetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(output));

        await minioClient.GetObjectAsync(args, ct);
        output.Position = 0;
        return output;
    }

    public async Task DeleteAsync(string objectName, CancellationToken ct)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName);

        await minioClient.RemoveObjectAsync(args, ct);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(_options.BucketName);
        if (await minioClient.BucketExistsAsync(existsArgs, ct))
            return;

        var makeArgs = new MakeBucketArgs().WithBucket(_options.BucketName);
        await minioClient.MakeBucketAsync(makeArgs, ct);
    }
}
