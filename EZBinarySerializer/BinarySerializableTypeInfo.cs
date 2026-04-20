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
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace EZBinarySerializer {
    internal class BinarySerializableTypeInfo {
        public readonly string TypeName;
        public readonly string ContainingNamespaceName;
        public readonly string TypeFlavor;
        public readonly List<string> TypeParameterNames;
        public readonly string TypeParameterConstraintString;
        public readonly List<BinarySerializableTypeInfo> TypeArguments;
        public readonly List<BinarySerializableMemberInfo> SerializableMemberInfo;
        public readonly bool IsAbstract;
        public readonly BinarySerializableTypeInfo? AbstractParentTypeInfo;
        public readonly BinarySerializableTypeInfo? EnumUnderlyingType;
        public readonly bool IsArray;
 
        public BinarySerializableTypeInfo Value {
            get {
                return this;
            }
        }

        public BinarySerializableTypeInfo(INamedTypeSymbol typeSymbol) {
            TypeName = GetTypeName(typeSymbol);
            ContainingNamespaceName = GetFullNamespace(typeSymbol);
            TypeFlavor = GetTypeFlavor(typeSymbol);
            TypeParameterNames = GetTypeParameterNames(typeSymbol);
            TypeParameterConstraintString = GetTypeParameterConstraintString(typeSymbol);
            TypeArguments = GetTypeArguments(typeSymbol);
            SerializableMemberInfo = GetSerializableMemberInfo(typeSymbol);
            IsAbstract = typeSymbol.IsAbstract;
            AbstractParentTypeInfo = GetAbstractParentTypeInfo(typeSymbol);
            EnumUnderlyingType = GetEnumUnderlyingType(typeSymbol);
            IsArray = false;
        }
        public BinarySerializableTypeInfo(IArrayTypeSymbol arraySymbol) {
            var typeSymbol = (INamedTypeSymbol)arraySymbol.ElementType;
            TypeName = GetTypeName(typeSymbol);
            ContainingNamespaceName = GetFullNamespace(typeSymbol);
            TypeFlavor = GetTypeFlavor(typeSymbol);
            TypeParameterNames = GetTypeParameterNames(typeSymbol);
            TypeParameterConstraintString = GetTypeParameterConstraintString(typeSymbol);
            TypeArguments = GetTypeArguments(typeSymbol);
            SerializableMemberInfo = GetSerializableMemberInfo(typeSymbol);
            IsAbstract = typeSymbol.IsAbstract;
            AbstractParentTypeInfo = GetAbstractParentTypeInfo(typeSymbol);
            EnumUnderlyingType = GetEnumUnderlyingType(typeSymbol);
            IsArray = true;
        }

        private string GetTypeName(INamedTypeSymbol typeSymbol) {
            List<string> typeNames = [];
            var containingType = typeSymbol.ContainingType;
            while (null != containingType) {
                typeNames.Add(containingType.Name);
                containingType = containingType.ContainingType;
            }
            typeNames.Reverse();
            typeNames.Add(typeSymbol.Name);
            return string.Join(
                ".",
                typeNames
            );
        }

        private BinarySerializableTypeInfo? GetEnumUnderlyingType(INamedTypeSymbol typeSymbol) {
            if (typeSymbol.EnumUnderlyingType is INamedTypeSymbol underlyingTypeSymbol) {
                return new(underlyingTypeSymbol);
            } else {
                return null;
            }
        }

        private List<string> GetTypeParameterNames(INamedTypeSymbol typeSymbol) {
            List<string> typeParameterNames = [];
            foreach (var typeParameter in typeSymbol.TypeParameters) {
                typeParameterNames.Add(typeParameter.Name);
            }
            return typeParameterNames;
        }

        private string GetTypeParameterConstraintString(INamedTypeSymbol typeSymbol) {
            var builder = new StringBuilder();
            foreach (var typeParameter in typeSymbol.TypeParameters) {
                List<string> constraints = [];
                if (typeParameter.HasConstructorConstraint) {
                    constraints.Add("new()");
                }
                if (typeParameter.HasNotNullConstraint) {
                    constraints.Add("notnull");
                }
                if (typeParameter.HasReferenceTypeConstraint) {
                    constraints.Add("class");
                }
                if (typeParameter.HasUnmanagedTypeConstraint) {
                    constraints.Add("unmanaged");
                }
                if (typeParameter.HasValueTypeConstraint) {
                    constraints.Add("struct");
                }

                if (0 < typeParameter.ConstraintTypes.Count()) {
                    builder.AppendFormat(
                        "where {0} : {1} ",
                        typeParameter.Name,
                        string.Join(
                            ", ", [
                                string.Join(
                                    ", ",
                                    constraints
                                ),
                                string.Join(
                                    ", ",
                                    typeParameter.ConstraintTypes.Select(
                                        c => {
                                            if (c is INamedTypeSymbol type) {
                                                return new BinarySerializableTypeInfo(type).GetFullyQualifiedTypeName();
                                            } else {
                                                return null;
                                            }
                                        }
                                    )
                                )
                            ]
                        )
                    );
                }
            }
            return builder.ToString();
        }

        public string GetFullyQualifiedTypeName() {
            var builder = new StringBuilder();
            builder.Append("global::");
            builder.Append(ContainingNamespaceName);
            builder.Append(".");
            builder.Append(TypeName);
            if (IsArray) {
                builder.Append("[]");
            }
            return builder.ToString();
        }

        public string GetTypeParameterString() {
            var builder = new StringBuilder();
            if (TypeParameterNames.Count > 0) {
                builder.Append("<");
                foreach (var parameterName in TypeParameterNames) {
                    builder.Append(parameterName);
                    builder.Append(", ");
                }
                builder.Remove(builder.Length - 2, 2);
                builder.Append(">");
            }
            return builder.ToString();
        }

        public string GetTypeArgumentString() {
            var builder = new StringBuilder();
            if (TypeArguments.Count > 0) {
                builder.Append("<");
                foreach (var argument in TypeArguments) {
                    builder.Append(argument.GetFullyQualifiedTypeName());
                    builder.Append(", ");
                }
                builder.Remove(builder.Length - 2, 2);
                builder.Append(">");
            }
            return builder.ToString();
        }

        public string GetValueSerializerName() {
            return string.Join(
                string.Empty, [
                    GetTypeArgumentString(),
                    GetFullyQualifiedTypeName(),
                    IsArray ? "Array" : string.Empty,
                    "ValueSerializer"
                ]
            ).Replace(
                "<", string.Empty
            ).Replace(
                ">", string.Empty
            ).Replace(
                ",", string.Empty
            ).Replace(
                " ", string.Empty
            ).Replace(
                ".", string.Empty
            ).Replace(
                "global::", string.Empty
            ).Replace(
                "[]", string.Empty
            );
        }

        private static BinarySerializableTypeInfo? GetAbstractParentTypeInfo(INamedTypeSymbol typeSymbol) {
            if (typeSymbol.BaseType is INamedTypeSymbol parentTypeSymbol) {
                if (
                    parentTypeSymbol.Name == "Object" ||
                    parentTypeSymbol.Name == "ValueType" ||
                    parentTypeSymbol.TypeKind == TypeKind.Interface
                ) {
                    return null;
                } else {
                    return new(parentTypeSymbol);
                }
            } else {
                return null;
            }
        }

        private static List<BinarySerializableMemberInfo> GetInheritedSerializableMemberInfo(INamedTypeSymbol parentTypeSymbol) {
            List<BinarySerializableMemberInfo> memberInfo = [];
            if (parentTypeSymbol.BaseType is INamedTypeSymbol baseType) {
                memberInfo.AddRange(GetInheritedSerializableMemberInfo(baseType));
            }

            var members = parentTypeSymbol.GetMembers();
            foreach (var memberSymbol in members) {
                if (memberSymbol is IPropertySymbol propertySymbol) {
                    if (
                        propertySymbol.IsWriteOnly ||
                        propertySymbol.IsStatic ||
                        propertySymbol.DeclaredAccessibility != Accessibility.Public ||
                        propertySymbol.IsOverride
                    ) {
                        continue;
                    }

                    if (propertySymbol.GetAttributes().Any(
                        (att) =>
                            att.AttributeClass is INamedTypeSymbol attributeSymbol && (
                                attributeSymbol.Name.Equals("BinarySerializerIgnore") ||
                                attributeSymbol.Name.Equals("BinarySerializerIgnoreAttribute")
                            )
                        )
                    ) {
                        continue;
                    }

                    if (propertySymbol.Type is INamedTypeSymbol propertyTypeSymbol) {
                        memberInfo.Add(new(
                            propertySymbol.Name,
                            new(propertyTypeSymbol)
                        ));
                    } else if (propertySymbol.Type is IArrayTypeSymbol arraySymbol) {
                        memberInfo.Add(new(
                            propertySymbol.Name,
                            new(arraySymbol)
                        ));
                    }
                } else if (memberSymbol is IFieldSymbol fieldSymbol) {
                    if (
                        fieldSymbol.DeclaredAccessibility != Accessibility.Public ||
                        fieldSymbol.IsStatic
                    ) {
                        continue;
                    }

                    if (fieldSymbol.GetAttributes().Any(
                        (att) =>
                            att.AttributeClass is INamedTypeSymbol attributeSymbol && (
                                attributeSymbol.Name.Equals("BinarySerializerIgnore") ||
                                attributeSymbol.Name.Equals("BinarySerializerIgnoreAttribute")
                            )
                    )) {
                        continue;
                    }

                    if (fieldSymbol.Type is INamedTypeSymbol fieldTypeSymbol) {
                        memberInfo.Add(new(
                            fieldSymbol.Name,
                            new(fieldTypeSymbol)
                        ));
                    } else if (fieldSymbol.Type is IArrayTypeSymbol arraySymbol) {
                        memberInfo.Add(new(
                            fieldSymbol.Name,
                            new(arraySymbol)
                        ));
                    }
                }
            }

            return memberInfo;
        }

        private static List<BinarySerializableMemberInfo> GetSerializableMemberInfo(INamedTypeSymbol typeSymbol) {
            List<BinarySerializableMemberInfo> result = [];

            if (!typeSymbol.GetAttributes().Any(
                (att) =>
                    att.AttributeClass is INamedTypeSymbol attributeSymbol && (
                        attributeSymbol.Name.Equals("BinarySerializable") ||
                        attributeSymbol.Name.Equals("BinarySerializableAttribute")
                    )
                )
            ) {
                return result;
            }

            return GetInheritedSerializableMemberInfo(typeSymbol);
        }

        private static List<BinarySerializableTypeInfo> GetTypeArguments(INamedTypeSymbol typeSymbol) {
            List<BinarySerializableTypeInfo> result = [];
            if (typeSymbol.IsGenericType) {
                foreach (var typeArgumentSymbol in typeSymbol.TypeArguments) {
                    if (typeArgumentSymbol is INamedTypeSymbol namedTypeArgumentSymbol) {
                        result.Add(
                            new(namedTypeArgumentSymbol)
                        );
                    }
                }
            }
            return result;
        }

        private static string GetTypeFlavor(INamedTypeSymbol typeSymbol) {
            string flavor = string.Empty;
            if (TypeKind.Struct == typeSymbol.TypeKind) {
                flavor = "struct";
            } else if (TypeKind.Class == typeSymbol.TypeKind) {
                flavor = "class";
            } else if (TypeKind.Interface == typeSymbol.TypeKind) {
                flavor = "interface";
            }
            return flavor;
        }

        private static string GetFullNamespace(INamedTypeSymbol typeSymbol) {
            List<string> result = [];
            var containingNamespace = typeSymbol.ContainingNamespace;
            while (
                null != containingNamespace &&
                string.Empty != containingNamespace.Name
            ) {
                result.Add(containingNamespace.Name);
                containingNamespace = containingNamespace.ContainingNamespace;
            }
            result.Reverse();
            return string.Join(".", result);
        }

    }
}
