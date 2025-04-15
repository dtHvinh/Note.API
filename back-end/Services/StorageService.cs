using BackEnd.Storage;
using Supabase.Storage;
using Supabase.Storage.Interfaces;

namespace back_end.Services;

public class StorageService(string url, string key) : FileStorage(url, key)
{
    private IStorageFileApi<FileObject> Bucket => Client.Storage.From("note");

    private readonly Supabase.Storage.FileOptions _options = new() { Upsert = true };

    public async Task<string> UploadAttachment(string documentId, IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);

        var extension = Path.GetExtension(file.FileName);

        var supabasePath = await Bucket.Upload(
            stream.ToArray(),
            $"attachments/{documentId}/${file.FileName}" + Random.Shared.NextInt64(1, 500) + extension,
            _options);

        return supabasePath;
    }
}
