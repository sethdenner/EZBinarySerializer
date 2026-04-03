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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace EZBinarySerializerSourceGeneration {
    [Generator]
    public class BinarySerializerGenerator : IIncrementalGenerator {
        public static ITypeSymbol? GetValidMemberTypeSymbol(GeneratorSyntaxContext context) {
            ITypeSymbol? memberTypeSymbol = null;
            ISymbol? declaredSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
            if (
                null == declaredSymbol &&
                context.Node is FieldDeclarationSyntax fieldSyntax
            ) {
                declaredSymbol = context.SemanticModel.GetDeclaredSymbol(
                    fieldSyntax.Declaration.Variables[0]
                );
            }
            bool isReadOnly = true;
            bool isWriteOnly = true;
            bool isAbstract = true;
            bool isPublic = false;

            if (declaredSymbol is IPropertySymbol propertySymbol) {
                memberTypeSymbol = propertySymbol.Type;
                isReadOnly = propertySymbol.IsReadOnly;
                isWriteOnly = propertySymbol.IsWriteOnly;
                isAbstract = propertySymbol.IsAbstract;
                isPublic = propertySymbol.DeclaredAccessibility == Accessibility.Public;
            } else if (declaredSymbol is IFieldSymbol fieldSymbol) {
                memberTypeSymbol = fieldSymbol.Type;
                isReadOnly = fieldSymbol.IsReadOnly;
                isWriteOnly = false;
                isAbstract = fieldSymbol.IsAbstract;
                isPublic = fieldSymbol.DeclaredAccessibility == Accessibility.Public;
            }
            if (
                isReadOnly ||
                isWriteOnly ||
                isAbstract ||
                !isPublic
            ) {
                return null;
            }

            return memberTypeSymbol;
        }

        public void Initialize(IncrementalGeneratorInitializationContext context) {
            /*
            if (!System.Diagnostics.Debugger.IsAttached) {
                try {
                    System.Diagnostics.Debugger.Launch();
                } catch (Exception _) { }
            }
            */

            IncrementalValueProvider<string?> assemblyName = context.CompilationProvider.Select((
                Compilation c,
                CancellationToken _
            ) => {
                // Grab the values from Compilation and CompilationOptions
                return c.AssemblyName;
            });

            IncrementalValueProvider<string?> inheritedAssemblyName = context.AnalyzerConfigOptionsProvider.Select((config, _) => {
                config.GlobalOptions.TryGetValue(
                    $"build_property.InheritedBinarySerializerAssemblyName",
                    out string? inheritedBinarySerializerAssemblyName
                );
                return inheritedBinarySerializerAssemblyName;
            });

            BinarySerializableValueSerializerGenerator binarySerializableValueGenerator = new();
            var binarySerializableValueGeneratorProvider =
                binarySerializableValueGenerator.Initialize(context).Collect().Combine(assemblyName);
            ArrayValueSerializerGenerator arrayValueSerializerGenerator = new();
            var arrayValueSerializerGeneratorProvider =
                arrayValueSerializerGenerator.Initialize(context).Collect().Combine(assemblyName);
            MemoryValueSerializerGenerator memoryValueSerializerGenerator = new();
            var memoryValueSerializerGeneratorProvider =
                memoryValueSerializerGenerator.Initialize(context).Collect().Combine(assemblyName);
            SpanValueSerializerGenerator spanValueSerializerGenerator = new();
            var spanValueSerializerGeneratorProvider =
                spanValueSerializerGenerator.Initialize(context).Collect().Combine(assemblyName);
            ListValueSerializerGenerator listValueSerializerGenerator = new();
            var listValueSerializerGeneratorProvider =
                listValueSerializerGenerator.Initialize(context).Collect().Combine(assemblyName);
            DictionaryValueSerializerGenerator dictionaryValueSerializerGenerator = new();
            var dictionaryValueSerializerGeneratorProvider =
                dictionaryValueSerializerGenerator.Initialize(context).Collect().Combine(assemblyName);

            binarySerializableValueGenerator.RegisterSourceOutput(
                context,
                binarySerializableValueGeneratorProvider
            );
            arrayValueSerializerGenerator.RegisterSourceOutput(
                context,
                arrayValueSerializerGeneratorProvider
            );
            memoryValueSerializerGenerator.RegisterSourceOutput(
                context,
                memoryValueSerializerGeneratorProvider
            );
            spanValueSerializerGenerator.RegisterSourceOutput(
                context,
                spanValueSerializerGeneratorProvider
            );
            listValueSerializerGenerator.RegisterSourceOutput(
                context,
                listValueSerializerGeneratorProvider
            );
            dictionaryValueSerializerGenerator.RegisterSourceOutput(
                context,
                dictionaryValueSerializerGeneratorProvider
            );

            context.RegisterSourceOutput(
                binarySerializableValueGeneratorProvider.Combine(inheritedAssemblyName), (
                    SourceProductionContext spc, (
                        (ImmutableArray<BinarySerializableTypeInfo?> Types,
                        string? AssemblyName
                    ) TypeInfo, string? InheritedAssemblyName) info
                ) => {
                    if (info.InheritedAssemblyName is null) {
                        GenerateStaticSourceFiles(
                            spc,
                            info.TypeInfo.AssemblyName
                        );
                    }
                    BinarySerializerStaticSourceHelper sourceHelper = new(info.TypeInfo.AssemblyName);
                    string result = string.Join(
                        "", [
                            sourceHelper.LicenseHeader,
                            GenerateBinarySerializerMethodDictionaries(info)
                        ]
                    );

                    spc.AddSource(
                        "BinarySerializer.BinarySerializer.g.cs",
                        result
                    );
                }
            );
        }

        private void GenerateStaticSourceFiles(
            SourceProductionContext context,
            string? assemblyName
        ) {
            BinarySerializerStaticSourceHelper helper = new(
                assemblyName
            );
            context.AddSource(
                "BinarySerializer.BinarySerializableAttribute.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.BinarySerializableAttributeSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.BinarySerializerIgnoreAttribute.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.BinarySerializerIgnoreAttributeSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.IBinarySerializable.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.IBinarySerializableSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.IValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.IValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.BinarySerializableHeader.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.BinarySerializableHeaderSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemBooleanValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemBooleanValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemByteValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemByteValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemCharValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemCharValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemDoubleValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemDoubleValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemDrawingRectangleFValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemDrawingRectangleFValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemSingleValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemSingleValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemInt16ValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemInt16ValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemInt32ValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemInt32ValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemInt64ValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemInt64ValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemNumericsMatrix4x4ValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemNumericsMatrix4x4ValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemNumericsVector2ValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemNumericsVector2ValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemNumericsVector3ValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemNumericsVector3ValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemStringValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemStringValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemUInt16ValueSerializer.g.cs",
                string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemUInt16ValueSerializerSource
                    ]
                )
            );
            context.AddSource(
                "BinarySerializer.SystemUInt32ValueSerializer.g.cs",
                 string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemUInt32ValueSerializerSource
                    ]
                 )
            );
            context.AddSource(
                "BinarySerializer.SystemUInt64ValueSerializer.g.cs",
                 string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemUInt64ValueSerializerSource
                    ]
                 )
            );
            context.AddSource(
                "BinarySerializer.SystemGuidValueSerializer.g.cs",
                 string.Join(
                    "", [
                        helper.LicenseHeader,
                        helper.SystemGuidValueSerializerSource
                    ]
                 )
            );
            context.AddSource(
                "LICENSE",
                helper.License
            );
        }

        private StringBuilder GenerateBinarySerializerClass((
            ImmutableArray<BinarySerializableTypeInfo?> Types,
            string? AssemblyName
        ) info) {
            StringBuilder builder = new();
            builder.AppendFormat(@"
namespace EZBinarySerializer.{0} {{
    public partial class BinarySerializer {{
        public delegate Memory<byte> SerializerDelegate(global::EZBinarySerializer.IBinarySerializable value);
        public delegate int DeserializerDelegate(Span<byte> data, out global::EZBinarySerializer.IBinarySerializable value);
        public static Dictionary<string, SerializerDelegate> SerializerMethodsByTypeName = new() {{",
               info.AssemblyName
           );
            foreach (var type in info.Types) {
                if (
                    type is not { } ||
                    type.IsAbstract ||
                    type.TypeParameterNames.Count > 0
                ) {
                    continue;
                }

                builder.AppendFormat(@"
            {{ ""{0}"", {0}.ToBinary }},",
                    type.GetFullyQualifiedTypeName()
                );
            }
            if (0 < info.Types.Length) {
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append(@"
        };"
            );
            builder.Append(@"
        public static Dictionary<string, DeserializerDelegate> DeserializerMethodsByTypeName = new() {"
            );
            foreach (var type in info.Types) {
                if (
                    type is not { } ||
                    type.IsAbstract ||
                    type.TypeParameterNames.Count > 0
                ) {
                    continue;
                }

                builder.AppendFormat(@"
            {{ ""{0}"", {0}.FromBinary }},",
                    type.GetFullyQualifiedTypeName()
                );
            }
            if (0 < info.Types.Length) {
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append(@"
        };"
            );
            builder.Append(@"
    }
}
"
            );
            return builder;
        }

        private StringBuilder GenerateBinarySerializerSubClass(((
            ImmutableArray<BinarySerializableTypeInfo?> Types,
            string? AssemblyName
        ) TypeInfo, string? InheritedAssemblyName) info) {
            StringBuilder builder = new();
            builder.AppendFormat(@"
namespace EZBinarySerializer.{0} {{
    public partial class BinarySerializer : global::EZBinarySerializer.{1}.BinarySerializer {{
        static BinarySerializer() {{",
                info.TypeInfo.AssemblyName,
                info.InheritedAssemblyName
            );
            foreach (var type in info.TypeInfo.Types) {
                if (
                    type is not { } ||
                    type.IsAbstract ||
                    type.TypeParameterNames.Count > 0
                ) {
                    continue;
                }

                builder.AppendFormat(@"
            SerializerMethodsByTypeName[""{0}""] = {0}.ToBinary;",
                    type.GetFullyQualifiedTypeName()
                );
            }
            foreach (var type in info.TypeInfo.Types) {
                if (
                    type is not { } ||
                    type.IsAbstract ||
                    type.TypeParameterNames.Count > 0
                ) {
                    continue;
                }

                builder.AppendFormat(@"
            DeserializerMethodsByTypeName[""{0}""] = {0}.FromBinary;",
                    type.GetFullyQualifiedTypeName()
                );
            }
            builder.Append(@"
        }
    }
}
"
            );
            return builder;
        }

        private string GenerateBinarySerializerMethodDictionaries(((
            ImmutableArray<BinarySerializableTypeInfo?> Types,
            string? AssemblyName
        ) TypeInfo, string? InheritedAssemblyName) info) {
            StringBuilder builder = new();
            if (null != info.InheritedAssemblyName) {
                builder = GenerateBinarySerializerSubClass(info);
            } else {
                builder = GenerateBinarySerializerClass(info.TypeInfo);
            }
            return builder.ToString();
        }
    }
}
