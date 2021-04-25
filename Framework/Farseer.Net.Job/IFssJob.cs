using FS.Job.GrpcServer;

namespace FS.Job
{
    public interface IFssJob
    {
        bool Execute(ReceiveContext context);
    }
}