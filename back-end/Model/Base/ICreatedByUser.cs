namespace back_end.Model.Base;

public interface ICreatedByUser<TUserKey>
{
    DateTimeOffset CreationDate { get; set; }
    TUserKey AuthorId { get; set; }
    ApplicationUser? Author { get; set; }
}
