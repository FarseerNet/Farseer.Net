using System.Threading.Tasks;
using FS.Job.Entity;

namespace FS.Job
{
    public interface IFssJob
    {
        Task<bool> Execute(ReceiveContext context);
    }
}