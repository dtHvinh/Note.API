namespace back_end.Model.Base;

public interface IUpdateableEntity<TUserKey>
{
    public DateTimeOffset LastUpdated { get; set; }
    TUserKey UpdateAuthorId { get; set; }
    ApplicationUser? UpdateAuthor { get; set; }
}
