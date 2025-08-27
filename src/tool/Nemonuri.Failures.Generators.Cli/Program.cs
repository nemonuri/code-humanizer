
using System.CommandLine;
using System.Diagnostics;
using System.Text;
using Nemonuri.Failures.Generators;

Argument<FileInfo> inputJsonFileArgument = new Argument<FileInfo>("InputJsonFile")
{
}.AcceptExistingOnly();

Argument<FileSystemInfo> outputFileOrDirectoryArgument = new Argument<FileSystemInfo>("OutputFileOrDirectory")
{
}.AcceptLegalFilePathsOnly();

RootCommand rootCommand = new RootCommand()
{
    inputJsonFileArgument, outputFileOrDirectoryArgument
};

FileInfo? inputFileInfo = null;
FileSystemInfo? outputFileOrDirectoryInfo = null;
rootCommand.SetAction(pr =>
{
    inputFileInfo = pr.GetRequiredValue(inputJsonFileArgument);
    outputFileOrDirectoryInfo = pr.GetRequiredValue(outputFileOrDirectoryArgument);
});

var resultCode = rootCommand.Parse(args).Invoke();
if (resultCode != 0) { return resultCode; }
Debug.Assert(inputFileInfo is not null);
Debug.Assert(outputFileOrDirectoryInfo is not null);

string inputJsonText = File.ReadAllText(inputFileInfo.FullName);
GenerateCodeEntryData entry = GenerateCodeEntryData.Parse(inputJsonText);

FileInfo? outputFileInfo = null;
if (Directory.Exists(outputFileOrDirectoryInfo.FullName))
{
    StringBuilder sb = new();
    if (entry.RootClass is { } rootClass)
    {
        sb.Append(rootClass).Append('.');
    }
    sb.Append(entry.MethodAlias)
        .Append("Result.g.cs");

    string newFileName = sb.ToString();
    outputFileInfo = new FileInfo(Path.Combine(outputFileOrDirectoryInfo.FullName, newFileName));
}
else
{
    outputFileInfo = new FileInfo(outputFileOrDirectoryInfo.FullName);
}

entry.TryGenerateCode(out string? code);

Debug.Assert(code is not null);

File.WriteAllText(outputFileInfo.FullName, code);

return 0;