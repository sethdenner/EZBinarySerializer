using EZBinarySerializerSourceGeneration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EZBinarySerializer.Tests {
    internal class SourceVerification {
        public static Task VerifySource(string source) {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            IEnumerable<PortableExecutableReference> references = [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            ];
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: [syntaxTree],
                references: references
            );

            var generator = new BinarySerializerGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);

            var settings = new VerifySettings();
            settings.UseDirectory("./Snapshots");
            return Verify(driver, settings);
        }
    }
}
