namespace FS.Data
{
    public class BuildTask : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            Log.LogMessage("Hello Task!-----------------------------------");
            return true;
        }
    }
}