using Amazon.S3;
using Amazon.S3.Model;
using VizsgaremekBackend.Interfaces;

namespace VizsgaremekBackend.Services;

public class S3FileService(IAmazonS3 s3, IConfiguration config) : IFileService
{
    private readonly string _bucket =
        config["S3:Bucket"] 
        ?? throw new InvalidOperationException("S3 Bucket not configured");
    
    public async Task<(Stream, string)> GetAsync(string key, CancellationToken ct = default)
    {
        GetObjectResponse res = await s3.GetObjectAsync(_bucket, key, ct);
        return (res.ResponseStream, res.Headers.ContentType);
    }

    public async Task PostAsync(string key,
        Stream value,
        string contentType,
        CancellationToken ct = default)
    {
        await s3.PutObjectAsync(new PutObjectRequest()
        {
            BucketName = _bucket,
            Key = key,
            InputStream = value,
            ContentType = contentType,
            DisablePayloadSigning = true,
        }, ct);
    }

    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        await s3.DeleteObjectAsync(_bucket, key, ct);
    }
}