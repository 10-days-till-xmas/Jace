using Yace.Util;
using Xunit;

namespace Yace.Tests;

public sealed class MemoryCacheTests
{
    [Fact]
    public void TestCacheCleanupOnlyAdd()
    {
        var cache = new MemoryCache<string, int>(3, 1);
        cache.GetOrAdd("test1", _ => 1);
        cache.GetOrAdd("test2", _ => 2);
        cache.GetOrAdd("test3", _ => 3);
        cache.GetOrAdd("test4", _ => 3);

        Assert.False(cache.ContainsKey("test1"));
        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void TestCacheCleanupRetrieve()
    {
        var cache = new MemoryCache<string, int>(3, 1);
        cache.GetOrAdd("test1", _ => 1);
        cache.GetOrAdd("test2", _ => 2);
        cache.GetOrAdd("test3", _ => 3);

        Assert.Equal(1, cache["test1"]); // order is now test2, test3, test1

        cache.GetOrAdd("test4", _ => 3);

        Assert.True(cache.ContainsKey("test1"));
        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void TestCacheCleanupBiggerThanCacheSize()
    {
        var cache = new MemoryCache<string, int>(1, 3);
        cache.GetOrAdd("test1", _ => 1);
        cache.GetOrAdd("test2", _ => 2);
        cache.GetOrAdd("test3", _ => 3);

        Assert.True(cache.ContainsKey("test3"));
        Assert.Equal(1, cache.Count);
    }
}