namespace back_end.Model.Base;

public interface IOwnObjects<TKey>
{
    ICollection<TKey>? ChildrenId { get; set; }
}
