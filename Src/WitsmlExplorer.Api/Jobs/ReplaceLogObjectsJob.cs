namespace WitsmlExplorer.Api.Jobs
{
    public record ReplaceLogObjectsJob : Job
    {
        public DeleteLogObjectsJob DeleteJob { get; init; }
        public CopyLogJob CopyJob { get; init; }

        public override string Description()
        {
            return $"{GetType().Name} - {DeleteJob.Description()}\t\n{CopyJob.Description()};";
        }

        public override string GetObjectName()
        {
            return CopyJob.GetObjectName();
        }

        public override string GetWellboreName()
        {
            return CopyJob.GetWellboreName();
        }

        public override string GetWellName()
        {
            return CopyJob.GetWellName();
        }
    }
}
