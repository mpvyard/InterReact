﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using InterReact;
using InterReact.Tests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace InterReact.Tests.UnitTests.Analysis
{
    public class AttributeViewer : BaseUnitTest
    {
        private static readonly IEnumerable<TypeInfo> types =
            typeof(InterReactClient).GetTypeInfo().Assembly
            .DefinedTypes.Where(t => !t.Name.Contains("<>"));

        public AttributeViewer(ITestOutputHelper output) : base(output) {}

        [Fact]
        public void Find_Type_Attributes()
        {
            foreach (var group in types
                .Select(ti => ti.GetCustomAttributes(true).Select(a => new { a, ti }))
                .SelectMany(x => x)
                .Where(x =>
                    !(x.a is CompilerGeneratedAttribute) &&
                    !(x.a is ExtensionAttribute))
                .GroupBy(x => x.a))
            {
                Write(group.Key.ToString());
                foreach (var a in group.OrderBy(x => x.ti.FullName))
                    Write($"    {a.ti.FullName}");
            }
        }

        [Fact]
        public void Find_Member_Attributes()
        {
            foreach (var group in types
                .Select(t => t.DeclaredMembers.Where(m => !m.Name.StartsWith("<")).Select(m => new { t, m }))
                .SelectMany(x => x)
                .Select(x => x.m.GetCustomAttributes(false).Select(q => new { type = x.t, method = x.m, attr = q }))
                .SelectMany(x => x)
                .Where(x => !(
                        x.attr is CompilerGeneratedAttribute ||
                        x.attr is DebuggerHiddenAttribute ||
                        x.attr is SecuritySafeCriticalAttribute ||
                        x.attr is AsyncStateMachineAttribute ||
                        x.attr is DebuggerStepThroughAttribute))
                .GroupBy(x => x.attr))
            {
                Write(group.Key.ToString());
                foreach (var a in group.OrderBy(x=> x.type.FullName + x.method.Name))
                    Write($"     {a.type.FullName}  {a.method.Name}");
            }
        }

    }
}
