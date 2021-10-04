namespace FS.Data.Features
{
    /// <summary>
    /// ClickHouse的MergeTree类型
    /// </summary>
    public enum EumTableEnginesType
    {
        MergeTree,
        VersionedCollapsingMergeTree,
        GraphiteMergeTree,
        AggregatingMergeTree,
        CollapsingMergeTree,
        ReplacingMergeTree,
        SummingMergeTree,
        Log,
        StripeLog,
        TinyLog,
    }
}