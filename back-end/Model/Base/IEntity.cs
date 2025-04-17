namespace back_end.Model.Base;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
