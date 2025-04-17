using back_end.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Model;

public class Block : IEntityBase<string>, ICreatedByUser<int>, IUpdateableEntity<int>, IArchivableEntity
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public string? ParentId { get; set; }
    [Column(TypeName = "jsonb")] public required string Data { get; set; }

    public bool IsArchived { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    [ForeignKey(nameof(BlockAuthor))] public int BlockAuthorId { get; set; }
    public ApplicationUser? BlockAuthor { get; set; }

    public DateTimeOffset LastUpdated { get; set; }

    [ForeignKey(nameof(UpdateAuthor))] public int UpdateAuthorId { get; set; }
    public ApplicationUser? UpdateAuthor { get; set; }
}