
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Nemonuri.Trees;
using Nemonuri.Trees.CodeAnalysis.CSharp;
using Nemonuri.Trees.Indexes;

string sourceCode =
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ICollectionDebugView.cs
"""
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace System.Collections.Generic
{
    internal sealed class ICollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public ICollectionDebugView(ICollection<T> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);

            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] items = new T[_collection.Count];
                _collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}
""";

var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

var childProvider = new SyntaxNodeOrTokenChildProvider();

StringBuilder sb = new();
var aggregatePremise = new AdHocIndexedPathAggregatingPremise<SyntaxNodeOrToken, StringBuilder>
(
    defaultSeedProvider: () => sb,
    optionalAggregator: (_, _, source) =>
    {
        if (!source.IndexedPath.TryGetLastNode(out var lastNode))
        { throw new InvalidOperationException(); }

        sb.Append(source.IndexedPath.ToIndexSequence().ToString())
        .Append(", ")
        .Append($"Kind = {lastNode.Kind()}").Append(", ")
        .Append('{').Append(lastNode.ToString().Trim()).Append('}')
        .AppendLine();

        return (sb, true);
    }
);

if (WalkingTheory.TryWalkAsRoot(aggregatePremise, childProvider, syntaxTree.GetRoot(), out var walkedValue))
{
    Console.WriteLine(walkedValue);
    return 0;
}
else
{
    return 1;
}
