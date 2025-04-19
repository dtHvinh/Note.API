using back_end.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Model;

[Table("Blocks")]
public class Block :
    IEntityBase<string>, ICreatedByUser<int>, IUpdateableEntity<int>, IArchivableEntity, IBelongToObject<string?>, IOwnObjects<string>
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    [Column(TypeName = "jsonb")] public required string Data { get; set; }
    public bool IsArchived { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public string? ParentObjectId { get; set; }
    public ICollection<string>? ChildrenId { get; set; }

    [ForeignKey(nameof(Author))] public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }


    [ForeignKey(nameof(UpdateAuthor))] public int UpdateAuthorId { get; set; }
    public ApplicationUser? UpdateAuthor { get; set; }
}