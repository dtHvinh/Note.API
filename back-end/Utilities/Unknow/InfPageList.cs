namespace back_end.Utilities.Unknow;

public class InfPageList<T>
{
    public List<T> Items { get; set; } = [];
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}
