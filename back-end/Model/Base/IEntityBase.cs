namespace back_end.Model.Base;

public interface IEntityBase : IEntityBase<int>
{

}

public interface IEntityBase<TKey> : IEntity<TKey>
{
}
