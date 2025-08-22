
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using Nemonuri.Failures.Generators;

//var node = GenerateCodeEntryData.GetJsonSchemaAsNode();
JsonSerializerOptions options = JsonSerializerOptions.Default;
JsonSchemaExporterOptions exporterOptions = new()
{
    TreatNullObliviousAsNonNullable = true,
};
JsonNode schema = options.GetJsonSchemaAsNode(typeof(GenerateCodeEntryData), exporterOptions);
Console.WriteLine(schema.ToString());