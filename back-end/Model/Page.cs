using back_end.Model.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.Model;

[Table("Pages")]
public class Page :
    IEntityBase<Guid>, ICreatedByUser<int>, IUpdateableEntity<int>,
    IArchivableEntity, IBelongToObject<string?>, IOwnObjects<string>, IDelete
{
    public Guid Id { get; set; } = default!;
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
    public bool IsArchived { get; set; }
    public bool IsInTrash { get; set; }
    public string Name { get; set; } = default!;

    public PageIcon PageIcon { get; set; } = default!;

    public int AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    public int UpdateAuthorId { get; set; }
    public ApplicationUser? UpdateAuthor { get; set; }

    public string? ParentObjectId { get; set; }
    public ICollection<string>? ChildrenId { get; set; }
}

[Owned]
public class PageIcon
{
    [Column(TypeName = "character varying")] public PageIconTypes Type { get; set; }
    public string Data { get; set; } = default!;
}

/// <summary>
/// Emoji, Svg, Image
/// </summary>
public enum PageIconTypes
{
    Emoji,
    Svg,
    Image
}
