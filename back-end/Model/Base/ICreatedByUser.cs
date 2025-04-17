namespace back_end.Model.Base;

public interface ICreatedByUser<TUserKey>
{
    DateTimeOffset CreationDate { get; set; }
    TUserKey BlockAuthorId { get; set; }
    ApplicationUser? BlockAuthor { get; set; }
}
