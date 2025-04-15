namespace back_end.Utilities.Helper;

public class BatchExecutor
{
    private readonly List<Task> _tasks = [];

    public void Register(Task task)
    {
        _tasks.Add(task);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.WhenAll(_tasks);
    }

    public static BatchExecutor Create(List<Task> tasks)
    {
        var batchExecutor = new BatchExecutor();
        foreach (var task in tasks)
        {
            batchExecutor.Register(task);
        }
        return batchExecutor;
    }
}
