using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jace.Execution;

namespace Jace.Tests.Mocks;
// I'm not sure how this'll be tested. TODO: Add tests for MockConstantRegistry (verify that it can use the constants pi and e?)
public static class MockConstantRegistry
{
    public static ConstantRegistry GetPresetConstantRegistry()
    {
        var registry = new ConstantRegistry(false);
        registry.RegisterConstant("pi", Math.PI, true);
        registry.RegisterConstant("e", Math.E, true);
        return registry;
    }
}