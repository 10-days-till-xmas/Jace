using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jace.Util;
using Xunit;

namespace Jace.Tests;


public class MemoryCacheTests
{
    [Fact]
    public void TestCacheCleanupOnlyAdd()
    {
        var cache = new MemoryCache<string, int>(3, 1);
        cache.GetOrAdd("test1", k => 1);
        cache.GetOrAdd("test2", k => 2);
        cache.GetOrAdd("test3", k => 3);
        cache.GetOrAdd("test4", k => 3);

        Assert.False(cache.ContainsKey("test1"));
        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void TestCacheCleanupRetrieve()
    {
        var cache = new MemoryCache<string, int>(3, 1);
        cache.GetOrAdd("test1", k => 1);
        cache.GetOrAdd("test2", k => 2);
        cache.GetOrAdd("test3", k => 3);
            
        Assert.Equal(1, cache["test1"]);
            
        cache.GetOrAdd("test4", k => 3);

        Assert.True(cache.ContainsKey("test1"));
        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void TestCacheCleanupBiggerThanCacheSize()
    {
        var cache = new MemoryCache<string, int>(1, 3);
        cache.GetOrAdd("test1", k => 1);
        cache.GetOrAdd("test2", k => 2);
        cache.GetOrAdd("test3", k => 3);

        Assert.True(cache.ContainsKey("test3"));
        Assert.Equal(1, cache.Count);
    }
}