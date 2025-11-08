using System.Globalization;
using Yace.Execution;
using Yace.Util;
using Xunit;

namespace Yace.Tests;

public sealed class YaceOptionsTests
{
    [Fact]
    public void SetFlags_WorksAsExpected()
    {
        var flags = YaceEngineFlags.None;
        Assert.Equal(0, (int)flags);
        flags = flags.SetFlag(true, YaceEngineFlags.CacheEnabled);
        Assert.Equal(YaceEngineFlags.CacheEnabled, flags);
        flags = flags.SetFlag(true, YaceEngineFlags.OptimizerEnabled);
        Assert.Equal(YaceEngineFlags.CacheEnabled | YaceEngineFlags.OptimizerEnabled, flags);
        flags = flags.SetFlag(false, YaceEngineFlags.CacheEnabled);
        Assert.Equal(YaceEngineFlags.OptimizerEnabled, flags);
    }
    [Fact]
    public void HasFlag_WorksAsExpected()
    {
        var flags = YaceEngineFlags.None;
        Assert.Equal(0, (int)flags);
        flags |= YaceEngineFlags.CacheEnabled;
        Assert.True(flags.HasFlagFast(YaceEngineFlags.CacheEnabled));
        Assert.False(flags.HasFlagFast(YaceEngineFlags.OptimizerEnabled));
        flags |= YaceEngineFlags.OptimizerEnabled;
        Assert.True(flags.HasFlagFast(YaceEngineFlags.OptimizerEnabled));
        Assert.True(flags.HasFlagFast(YaceEngineFlags.CacheEnabled));
        flags &= ~YaceEngineFlags.CacheEnabled;
        Assert.False(flags.HasFlagFast(YaceEngineFlags.CacheEnabled));
        Assert.True(flags.HasFlagFast(YaceEngineFlags.OptimizerEnabled));
    }
    [Fact]
    public void Ctor_WorksAsExpected()
    {
        var options = new YaceOptions(); // keep this consistent of course
                                         // (the cache sizes can change though since they're internal implementation details)
        Assert.Equal(YaceOptions.Default, options);
        Assert.Equal(CultureInfo.InvariantCulture, options.CultureInfo);
        Assert.Equal(ExecutionMode.Compiled, options.ExecutionMode);
        Assert.Equal(YaceEngineFlags.Default, options.YaceOptionFlags);

        options = new YaceOptions { ExecutionMode = ExecutionMode.Interpreted };
        Assert.Equal(ExecutionMode.Interpreted, options.ExecutionMode);

        options = new YaceOptions { CacheEnabled = false };
        Assert.False(options.CacheEnabled);

        options = new YaceOptions(CultureInfo.CurrentCulture, ExecutionMode.Interpreted)
            { CacheEnabled = false, OptimizerEnabled = false, CaseSensitive = true };
        Assert.Equal(CultureInfo.CurrentCulture, options.CultureInfo);
        Assert.Equal(ExecutionMode.Interpreted, options.ExecutionMode);
        Assert.False(options.CacheEnabled);
        Assert.False(options.OptimizerEnabled);
        Assert.True(options.CaseSensitive);
    }
}