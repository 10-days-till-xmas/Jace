using System.Globalization;
using Jace.Execution;
using Jace.Util;
using Xunit;

namespace Jace.Tests;

public sealed class JaceOptionsTests
{
    [Fact]
    public void SetFlags_WorksAsExpected()
    {
        var flags = JaceEngineFlags.None;
        Assert.Equal(0, (int)flags);
        flags = flags.SetFlag(true, JaceEngineFlags.CacheEnabled);
        Assert.Equal(JaceEngineFlags.CacheEnabled, flags);
        flags = flags.SetFlag(true, JaceEngineFlags.OptimizerEnabled);
        Assert.Equal(JaceEngineFlags.CacheEnabled | JaceEngineFlags.OptimizerEnabled, flags);
        flags = flags.SetFlag(false, JaceEngineFlags.CacheEnabled);
        Assert.Equal(JaceEngineFlags.OptimizerEnabled, flags);
    }
    [Fact]
    public void HasFlag_WorksAsExpected()
    {
        var flags = JaceEngineFlags.None;
        Assert.Equal(0, (int)flags);
        flags |= JaceEngineFlags.CacheEnabled;
        Assert.True(flags.HasFlagFast(JaceEngineFlags.CacheEnabled));
        Assert.False(flags.HasFlagFast(JaceEngineFlags.OptimizerEnabled));
        flags |= JaceEngineFlags.OptimizerEnabled;
        Assert.True(flags.HasFlagFast(JaceEngineFlags.OptimizerEnabled));
        Assert.True(flags.HasFlagFast(JaceEngineFlags.CacheEnabled));
        flags &= ~JaceEngineFlags.CacheEnabled;
        Assert.False(flags.HasFlagFast(JaceEngineFlags.CacheEnabled));
        Assert.True(flags.HasFlagFast(JaceEngineFlags.OptimizerEnabled));
    }
    [Fact]
    public void Ctor_WorksAsExpected()
    {
        var options = new JaceOptions(); // keep this consistent of course
                                         // (the cache sizes can change though since they're internal implementation details)
        Assert.Equal(JaceOptions.Default, options);
        Assert.Equal(CultureInfo.InvariantCulture, options.CultureInfo);
        Assert.Equal(ExecutionMode.Compiled, options.ExecutionMode);
        Assert.Equal(JaceEngineFlags.Default, options.JaceOptionFlags);

        options = new JaceOptions { ExecutionMode = ExecutionMode.Interpreted };
        Assert.Equal(ExecutionMode.Interpreted, options.ExecutionMode);

        options = new JaceOptions { CacheEnabled = false };
        Assert.False(options.CacheEnabled);

        options = new JaceOptions(CultureInfo.CurrentCulture, ExecutionMode.Interpreted)
            { CacheEnabled = false, OptimizerEnabled = false, CaseSensitive = true };
        Assert.Equal(CultureInfo.CurrentCulture, options.CultureInfo);
        Assert.Equal(ExecutionMode.Interpreted, options.ExecutionMode);
        Assert.False(options.CacheEnabled);
        Assert.False(options.OptimizerEnabled);
        Assert.True(options.CaseSensitive);
    }
}