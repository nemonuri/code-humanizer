$project = Join-Path $PSScriptRoot ".." "Nemonuri.Study.CSharpAICommentor.Core.UnitTests.csproj" -Resolve

& dotnet test $project -- --filter-method "*.TrySeparateComplexArgumentExpressions" --report-xunit