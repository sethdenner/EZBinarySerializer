/*
    EZBinarySerializer
    A fast, easy to use and AOT compatible serialization solution.
    Copyright (C) 2026 Seth Denner

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

    Also add information on how to contact you by electronic and paper mail.
 */
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EZBinarySerializerSourceGeneration {
    internal class ListValueSerializerGenerator {
        private string? AssemblyName = null;
        private static List<string> GeneratedFileNames = [];

        public ListValueSerializerGenerator() {
        }

        public IncrementalValuesProvider<BinarySerializableTypeInfo?> Initialize(IncrementalGeneratorInitializationContext context) {
            /*
            if (!System.Diagnostics.Debugger.IsAttached) {
                try {
                    System.Diagnostics.Debugger.Launch();
                } catch (Exception _) { }
            }
            */

            GeneratedFileNames.Clear();

            return context.SyntaxProvider
           .CreateSyntaxProvider(
               predicate: static (node, _) => IsSyntaxTargetForGeneration(node),
               transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
           .Where(static m => m is not null); // Filter out errors that we don't care about

        }

        public void RegisterSourceOutput(
            IncrementalGeneratorInitializationContext context,
            IncrementalValueProvider<(ImmutableArray<BinarySerializableTypeInfo?> Types, string? AssemblyName)> info
        ) {
            context.RegisterSourceOutput(
                info,
                Execute
            );
        }

        public static bool IsSyntaxTargetForGeneration(SyntaxNode node) {
            if (node is not PropertyDeclarationSyntax propertySyntax) {
                return false;
            }
            if (propertySyntax.Type is not GenericNameSyntax) {
                return false;
            }
            if (!propertySyntax.Type.GetText().ToString().Contains("List")) {
                return false;
            }
            if (propertySyntax.Parent is not TypeDeclarationSyntax typeSyntax) {
                return false;
            }
            if (
                null == typeSyntax.AttributeLists ||
                !typeSyntax.AttributeLists.Any(
                    s => s.GetText()
                    .ToString()
                    .Contains(
                        "BinarySerializable"
                    )
                )
            ) {
                return false;
            }
            if (propertySyntax.AttributeLists.Any(
                s => s.GetText()
                .ToString()
                .Contains(
                    "BinarySerializerIgnore"
                )
            )) {
                return false;
            }
            if (typeSyntax.Modifiers.Any(
                s => s.Text == "abstract"
            )) {
                return false;
            }
            if (!propertySyntax.Modifiers.Any(
                s => s.Text == "public"
            )) {
                return false;
            }
            return true;
        }

        public static BinarySerializableTypeInfo? GetSemanticTargetForGeneration(
            GeneratorSyntaxContext context
        ) {
            BinarySerializableTypeInfo? info = null;
            var memberTypeSymbol = BinarySerializerGenerator.GetValidMemberTypeSymbol(context);

            if (memberTypeSymbol is not INamedTypeSymbol memberNamedTypeSymbol) {
                return null;
            }

            if (memberNamedTypeSymbol.Name != "List") {
                return null;
            }

            if (memberNamedTypeSymbol.Arity != 1
            ) {
                return null;
            }

            info = new(memberNamedTypeSymbol);

            return info;

        }

        void Execute(
            SourceProductionContext context,
            (ImmutableArray<BinarySerializableTypeInfo?> Types, string? AssemblyName) info
        ) {
            AssemblyName = info.AssemblyName;
            foreach (var type in info.Types) {
                if (type is { } value) {
                    string fileName = $"BinarySerializer.{value.GetValueSerializerName()}.g.cs";
                    if (!GeneratedFileNames.Contains(fileName)) {
                        string result = GenerateValueSerializerSource(value);
                        context.AddSource(
                            fileName,
                            SourceText.From(result, Encoding.UTF8)
                        );
                        GeneratedFileNames.Add(fileName);
                    }
                }
            }
        }

        private string GenerateValueSerializerSource(BinarySerializableTypeInfo? info) {
            if (null == info) {
                return string.Empty;
            }

            StringBuilder builder = new();
            builder.AppendFormat(@"
namespace EZBinarySerializer.ValueSerializers {{
    public class {1} : IValueSerializer<{0}<{2}>> {{
        public static int FromBinary(Span<byte> data, out {0}<{2}> value) {{
            int cursor = 0;
            int count = BitConverter.ToInt32(data[
                cursor..(cursor += sizeof(int))
            ]);
            value = [];
            for (int i = 0; i < count; ++i) {{
                cursor += {3}.FromBinary(
                    data[cursor..],
                    out {2} result
                );
                value.Add(result);
            }}
            return cursor;
        }}

        public static Memory<byte> ToBinary({0}<{2}> value) {{
            int size = 0;
            Span<byte> lengthBytes = BitConverter.GetBytes(value.Count);
            size += lengthBytes.Length;
            List<Memory<byte>> itemBytesList = [];
            for (int i = 0; i < value.Count; ++i) {{
                var itemBytes = {3}.ToBinary(value[i]);
                size += itemBytes.Length;
                itemBytesList.Add(itemBytes);
            }}

            Memory<byte> data = new byte[size];

            int cursor = 0;
            lengthBytes.CopyTo(data[
                cursor..(cursor += lengthBytes.Length)
            ].Span);
            for (int i = 0; i < itemBytesList.Count; ++i) {{
                var item = itemBytesList[i];
                item.CopyTo(data[
                    cursor..(cursor += item.Length)
                ]);
            }}

            return data;
        }}
    }}
}}
namespace EZBinarySerializer.{4} {{
    public partial class BinarySerializer {{
        public static int FromBinary(Span<byte> data, out {0}<{2}> value) {{
            return EZBinarySerializer.ValueSerializers.{1}.FromBinary(data, out value);
        }}

        public static Memory<byte> ToBinary({0}<{2}> value) {{
            return EZBinarySerializer.ValueSerializers.{1}.ToBinary(value);
        }}
    }}
}}",
                info.GetFullyQualifiedTypeName(),
                info.GetValueSerializerName(),
                info.TypeArguments[0].GetFullyQualifiedTypeName(),
                info.TypeArguments[0].GetValueSerializerName(),
                AssemblyName
            );
            return builder.ToString();
        }
    }
}

