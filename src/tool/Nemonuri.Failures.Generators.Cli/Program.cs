
using System.CommandLine;
using System.Diagnostics;
using Nemonuri.Failures.Generators;

Argument<FileInfo> inputJsonFileArgument = new Argument<FileInfo>("InputJsonFile")
{
}.AcceptExistingOnly();

Argument<FileInfo> outputFileArgument = new Argument<FileInfo>("OutputFile")
{
}.AcceptLegalFilePathsOnly();

RootCommand rootCommand = new RootCommand()
{
    inputJsonFileArgument, outputFileArgument
};

FileInfo? inputFileInfo = null, outputFileInfo = null;
rootCommand.SetAction(pr =>
{
    inputFileInfo = pr.GetRequiredValue(inputJsonFileArgument);
    outputFileInfo = pr.GetRequiredValue(outputFileArgument);
});

var resultCode = rootCommand.Parse(args).Invoke();
if (resultCode != 0) { return resultCode; }


Debug.Assert(inputFileInfo is not null);
Debug.Assert(outputFileInfo is not null);

string inputJsonText = File.ReadAllText(inputFileInfo.FullName);
GenerateCodeEntryData.Parse(inputJsonText).TryGenerateCode(out string? code);

Debug.Assert(code is not null);

File.WriteAllText(outputFileInfo.FullName, code);

return 0;