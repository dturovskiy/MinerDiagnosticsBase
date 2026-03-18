namespace Miner.Diagnostics
{
    public interface IGameDiagSnapshotProvider
    {
        HeroDiagSnapshot BuildSnapshot();
    }
}
