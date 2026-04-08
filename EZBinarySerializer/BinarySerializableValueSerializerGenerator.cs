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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EZBinarySerializer {
    internal class BinarySerializableValueSerializerGenerator {
        public string? AssemblyName = null;
        public List<BinarySerializableTypeInfo> BinarySerializableTypes = [];

        public BinarySerializableValueSerializerGenerator() {
        }

        public IncrementalValuesProvider<BinarySerializableTypeInfo?> Initialize(IncrementalGeneratorInitializationContext context) {
            /*
            if (!System.Diagnostics.Debugger.IsAttached) {
                try {
                    System.Diagnostics.Debugger.Launch();
                } catch (Exception _) { }
            }
            */
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
            if (node is not TypeDeclarationSyntax typeSyntax) {
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

            return true;
        }

        public static BinarySerializableTypeInfo? GetSemanticTargetForGeneration(
            GeneratorSyntaxContext context
        ) {
            // we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol typeSymbol) {
                return null;
            }

            if (!typeSymbol.GetAttributes().Any(
                (a) => a.AttributeClass is INamedTypeSymbol attributeSymbol && (
                    attributeSymbol.Name.Equals("BinarySerializable") ||
                    attributeSymbol.Name.Equals("BinarySerializableAttribute")
                )
            )) {
                return null;
            }

            return new(
                typeSymbol
            );
        }

        void Execute(
            SourceProductionContext context,
            (ImmutableArray<BinarySerializableTypeInfo?> Types, string? AssemblyName) info
        ) {
            AssemblyName = info.AssemblyName;
            foreach (var type in info.Types) {
                if (type is { } value) {
                    BinarySerializerStaticSourceHelper sourceHelper = new(AssemblyName);
                    string result = string.Join(
                        "", [
                            sourceHelper.LicenseHeader,
                            GenerateValueSerializerSource(type)
                        ]
                    );
                    context.AddSource(
                        $"BinarySerializer.{value.GetValueSerializerName()}.g.cs",
                        SourceText.From(result, Encoding.UTF8)
                    );
                    if (!value.IsAbstract) {
                        BinarySerializableTypes.Add(value);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        private StringBuilder GenerateConcreteValueSerializer(
            BinarySerializableTypeInfo? info,
            StringBuilder builder
        ) {
            if (info is null) {
                return builder;
            }
            builder.AppendFormat(@"
namespace EZBinarySerializer.ValueSerializers {{
    public class {0}{3} : IValueSerializer<{1}{3}> {4}{{
        public static int FromBinary(Span<byte> data, out {1}{3} value) {{
            int cursor = 0;
            cursor += SystemInt32ValueSerializer.FromBinary(
                data[
                    cursor..
                ],
                out int size
            );
            cursor += SystemStringValueSerializer.FromBinary(
                data[
                    cursor..
                ],
                out string typeName
            );",
                info.Value.GetValueSerializerName(),
                info.Value.GetFullyQualifiedTypeName(),
                AssemblyName,
                info.Value.GetTypeParameterString(),
                info.Value.TypeParameterConstraintString
            );
            for (int i = 0; i < info.Value.SerializableMemberInfo.Count; ++i) {
                var memberInfo = info.Value.SerializableMemberInfo[i];
                if (null != memberInfo.MemberTypeInfo.EnumUnderlyingType) {
                    builder.AppendFormat(@"
            cursor += {2}.FromBinary(
                data[cursor..],
                out {0} __ez__enum__{1}
            );
            {3} __ez__{1} =
                ({3})__ez__enum__{1};",
                        memberInfo.MemberTypeInfo.EnumUnderlyingType.GetFullyQualifiedTypeName(),
                        memberInfo.MemberName.ToLower(),
                        memberInfo.MemberTypeInfo.EnumUnderlyingType.GetValueSerializerName(),
                        memberInfo.MemberTypeInfo.GetFullyQualifiedTypeName(),
                        memberInfo.MemberTypeInfo.GetTypeArgumentString()
                    );
                } else {
                    builder.AppendFormat(@"
            cursor += {2}.FromBinary(
                data[cursor..],
                out {0}{3} __ez__{1}
            );",
                        memberInfo.MemberTypeInfo.GetFullyQualifiedTypeName(),
                        memberInfo.MemberName.ToLower(),
                        memberInfo.MemberTypeInfo.GetValueSerializerName(),
                        memberInfo.MemberTypeInfo.GetTypeArgumentString()
                    );
                }
            }
            builder.Append(@"
            value = new() {"
            );
            for (int i = 0; i < info.Value.SerializableMemberInfo.Count; ++i) {
                var memberInfo = info.Value.SerializableMemberInfo[i];
                builder.AppendFormat(@"
                {0} = __ez__{1},",
                    memberInfo.MemberName,
                    memberInfo.MemberName.ToLower()
                );
            }
            builder.Append(@"
            };

            return cursor;
        }"
            );
            builder.AppendFormat(@"
        public static Memory<byte> ToBinary({0}{1} value) {{
            int size = 0;
            List<Memory<byte>> bytesList = [];",
                info.Value.GetFullyQualifiedTypeName(),
                info.Value.GetTypeParameterString()
            );
            foreach (var propertyInfo in info.Value.SerializableMemberInfo) {
                if (null != propertyInfo.MemberTypeInfo.EnumUnderlyingType) {
                    builder.AppendFormat(@"
            bytesList.Add({0}.ToBinary(
                ({2})value.{1}
            ));
            size += bytesList[bytesList.Count - 1].Length;",
                        propertyInfo.MemberTypeInfo.EnumUnderlyingType.GetValueSerializerName(),
                        propertyInfo.MemberName,
                        propertyInfo.MemberTypeInfo.EnumUnderlyingType.TypeName
                    );
                } else {
                    builder.AppendFormat(@"
            bytesList.Add({0}.ToBinary(
                value.{1}
            ));
            size += bytesList[bytesList.Count - 1].Length;",
                        propertyInfo.MemberTypeInfo.GetValueSerializerName(),
                        propertyInfo.MemberName,
                        propertyInfo.MemberTypeInfo.GetTypeArgumentString()
                    );
                }
            }
            builder.AppendFormat(@"
            var typeNameBytes = SystemStringValueSerializer.ToBinary(value.FullyQualifiedTypeName);
            size += typeNameBytes.Length;
            size += sizeof(int);
            var sizeBytes = SystemInt32ValueSerializer.ToBinary(size);
            Memory<byte> data = new byte[size];
            int cursor = 0;
            sizeBytes.CopyTo(
                data[cursor..(cursor += sizeBytes.Length)]
            );
            typeNameBytes.CopyTo(
                data[cursor..(cursor += typeNameBytes.Length)]
            );
            foreach (var bytes in bytesList) {{
                bytes.CopyTo(
                    data[cursor..(cursor += bytes.Length)]
                );
            }}
            
            return data;
        }}
    }}
}}",
                AssemblyName
            );
            return builder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        private StringBuilder GenerateAbstractValueSerializer(
            BinarySerializableTypeInfo? info,
            StringBuilder builder
        ) {
            if (info is null) {
                return builder;
            }
            builder.AppendFormat(@"
namespace EZBinarySerializer.ValueSerializers {{
    public class {0} : IValueSerializer<{1}> {{
        public static int FromBinary(Span<byte> data, out {1} value) {{
            string typeName = IBinarySerializable.PeekTypeName(data);
            int size = global::EZBinarySerializer.BinarySerializer.DeserializerMethodsByTypeName[typeName](
                data,
                out IBinarySerializable serializable
            );
            value = ({1})serializable;
            return size;
        }}

        public static Memory<byte> ToBinary({1} value) {{
            return global::EZBinarySerializer.BinarySerializer.SerializerMethodsByTypeName[value.FullyQualifiedTypeName](
                value as global::EZBinarySerializer.IBinarySerializable
            );
        }}
    }}
}}",
                info.Value.GetValueSerializerName(),
                info.Value.GetFullyQualifiedTypeName(),
                AssemblyName
            );
            return builder;
        }
        private string GenerateValueSerializerSource(BinarySerializableTypeInfo? info) {
            if (null == info) {
                return string.Empty;
            }

            StringBuilder builder = new();
            if (info.Value.IsAbstract) {
                builder = GenerateAbstractValueSerializer(
                    info,
                    builder
                );
            } else {
                builder = GenerateConcreteValueSerializer(
                    info,
                    builder
                );
            }
            builder.AppendFormat(@"
namespace {0} {{
    public partial {1} {2}{3}{4} {5}{{",
                info.Value.ContainingNamespaceName,
                info.Value.TypeFlavor,
                info.Value.TypeName,
                info.Value.GetTypeParameterString(),
                info.Value.TypeFlavor.Equals("interface") ? string.Empty : " : global::EZBinarySerializer.IBinarySerializable",
                info.Value.TypeParameterConstraintString
            );
            if (info.IsAbstract) {
                builder.Append(@"
        public abstract string FullyQualifiedTypeName { get; }"
                );
            } else {
                builder.AppendFormat(@"
        public {1}string FullyQualifiedTypeName {{
            get {{
                return ""{0}"";
            }}
        }}",
                    info.Value.GetFullyQualifiedTypeName(), (
                        info.Value.AbstractParentTypeInfo != null &&
                        info.Value.AbstractParentTypeInfo.TypeName != "Object" &&
                        info.Value.AbstractParentTypeInfo.TypeName != "ValueType"
                    ) ? "override " : info.Value.TypeFlavor.Equals("class") ? "virtual " : string.Empty

                );
            }
            builder.AppendFormat(@"
        public {2}static Memory<byte> ToBinary(global::EZBinarySerializer.IBinarySerializable value) {{
            return global::EZBinarySerializer.ValueSerializers.{0}{4}.ToBinary(({1}{4})value);
        }}

        public {2}static int FromBinary(Span<byte> data, out global::EZBinarySerializer.IBinarySerializable value) {{
            int size = global::EZBinarySerializer.ValueSerializers.{0}{4}.FromBinary(data, out {1}{4} result);
            value = (global::EZBinarySerializer.IBinarySerializable)result;
            return size;
        }}
    }}
}}",
                    info.Value.GetValueSerializerName(),
                    info.Value.GetFullyQualifiedTypeName(), (
                        info.Value.AbstractParentTypeInfo != null &&
                        info.Value.AbstractParentTypeInfo.TypeName != "Object" &&
                        info.Value.AbstractParentTypeInfo.TypeName != "ValueType" &&
                        !info.Value.TypeFlavor.Equals("interface")
                    ) ? "new " : string.Empty,
                    AssemblyName,
                    info.GetTypeParameterString()
                );
            return builder.ToString();
        }
    }
}
