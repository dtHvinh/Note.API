namespace back_end.Model.Base;

public interface IBelongToObject<TKey>
{
    TKey ParentObjectId { get; set; }
}
