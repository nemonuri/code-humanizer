
using Nemonuri.Failures.Generators;

var code = GeneratingTheory.GenerateCode
(
    usings: ["System.Numerics"],
    @namespace: "Nemonuri.Failures.Generators.Sample1.Generated",
    rootClass: null,
    methodAlias: "MyMethod1",
    valueType: "(Vector2, Vector3)",
    failSlots: [new("Canceled"), new("AnotherError", "(int, string)")]
);

Console.WriteLine(code);