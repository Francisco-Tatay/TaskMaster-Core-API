using TaskManagerPro.TaskManagerPro.Interfaces;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Common;

public class IdGenerator:IIdGenerator
{
    public Guid NewId()
    {
        return Guid.NewGuid();
    }
}