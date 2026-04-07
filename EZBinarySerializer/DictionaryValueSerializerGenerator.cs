/*
 *  EZBinarySerializer serialize objects while maintaining AOT compatibility.
 *  Copyright (C) 2026 Seth Adam Denner
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as
 *  published by the Free Software Foundation, either version 3 of the
 *  License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EZBinarySerializer {
    internal class DictionaryValueSerializerGenerator {
        private string? AssemblyName = null;
        private List<string> GeneratedFileNames = [];

        public DictionaryValueSerializerGenerator() {
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
            TypeSyntax? memberTypeSyntax = null;
            TypeDeclarationSyntax? typeDeclarationSyntax = null;
            SyntaxList<AttributeListSyntax> memberAttributeLists;
            SyntaxTokenList memberModifiers = new();
            if (node is PropertyDeclarationSyntax propertySyntax) {
                memberTypeSyntax = propertySyntax.Type;
                memberModifiers = propertySyntax.Modifiers;
                memberAttributeLists = propertySyntax.AttributeLists;
                typeDeclarationSyntax = propertySyntax.Parent as TypeDeclarationSyntax;
            } else if (node is FieldDeclarationSyntax fieldSyntax) {
                memberTypeSyntax = fieldSyntax.Declaration.Type;
                memberAttributeLists = fieldSyntax.AttributeLists;
                memberModifiers = fieldSyntax.Modifiers;
                typeDeclarationSyntax = fieldSyntax.Parent as TypeDeclarationSyntax;
            }

            if (
                null == memberTypeSyntax ||
                null == typeDeclarationSyntax
            ) {
                return false;
            }


            if (memberTypeSyntax is not GenericNameSyntax) {
                return false;
            }
            if (!memberTypeSyntax.GetText().ToString().Contains("Dictionary")) {
                return false;
            }

            if (
                null == typeDeclarationSyntax.AttributeLists ||
                !typeDeclarationSyntax.AttributeLists.Any(
                    s => s.GetText()
                    .ToString()
                    .Contains(
                        "BinarySerializable"
                    )
                )
            ) {
                return false;
            }
            if (memberAttributeLists.Any(
                s => s.GetText()
                .ToString()
                .Contains(
                    "BinarySerializerIgnore"
                )
            )) {
                return false;
            }
            if (typeDeclarationSyntax.Modifiers.Any(
                s => s.Text == "abstract"
            )) {
                return false;
            }
            if (!memberModifiers.Any(
                s => s.Text == "public"
            )) {
                return false;
            }
            return true;
        }

        public static BinarySerializableTypeInfo? GetSemanticTargetForGeneration(
            GeneratorSyntaxContext context
        ) {
            var memberTypeSymbol = BinarySerializerGenerator.GetValidMemberTypeSymbol(context);

            if (memberTypeSymbol is not INamedTypeSymbol memberNamedTypeSymbol) {
                return null;
            }

            if (memberNamedTypeSymbol.Name != "Dictionary") {
                return null;
            }
            if (memberNamedTypeSymbol.Arity != 2
            ) {
                return null;
            }

            return new(memberNamedTypeSymbol);
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
                        BinarySerializerStaticSourceHelper sourceHelper = new(AssemblyName);
                        string result = string.Join(
                            "", [
                                sourceHelper.LicenseHeader,
                                GenerateValueSerializerSource(type)
                            ]
                        );
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
    public class {1} : IValueSerializer<{0}<{2}, {4}>> {{
        public static int FromBinary(Span<byte> data, out {0}<{2}, {4}> value) {{
            int cursor = 0;
            int count = BitConverter.ToInt32(data[
                cursor..(cursor += sizeof(int))
            ]);
            value = [];
            for (int i = 0; i < count; ++i) {{
                cursor += {3}.FromBinary(
                    data[cursor..],
                    out {2} key
                );
                cursor += {5}.FromBinary(
                    data[cursor..],
                    out {4} result
                );
                value[key] = result;
            }}
            return cursor;
        }}

        public static Memory<byte> ToBinary({0}<{2}, {4}> value) {{
            int size = 0;
            Span<byte> countBytes = BitConverter.GetBytes(value.Count);
            size += countBytes.Length;
            List<Memory<byte>> itemBytesList = [];
            foreach (var key in value.Keys) {{
                var keyBytes = {3}.ToBinary(key);
                size += keyBytes.Length;
                itemBytesList.Add(keyBytes);
                var valueBytes = {5}.ToBinary(value[key]);
                size += valueBytes.Length;
                itemBytesList.Add(valueBytes);
            }}

            Memory<byte> data = new byte[size];

            int cursor = 0;
            countBytes.CopyTo(data[
                cursor..(cursor += countBytes.Length)
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
}}",
                info.GetFullyQualifiedTypeName(),
                info.GetValueSerializerName(),
                info.TypeArguments[0].GetFullyQualifiedTypeName(),
                info.TypeArguments[0].GetValueSerializerName(),
                info.TypeArguments[1].GetFullyQualifiedTypeName(),
                info.TypeArguments[1].GetValueSerializerName(),
                AssemblyName
            );
            return builder.ToString();
        }
    }
}

