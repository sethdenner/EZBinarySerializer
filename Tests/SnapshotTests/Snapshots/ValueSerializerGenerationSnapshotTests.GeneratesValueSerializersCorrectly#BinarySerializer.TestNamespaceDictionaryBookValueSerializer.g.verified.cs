//HintName: BinarySerializer.TestNamespaceDictionaryBookValueSerializer.g.cs

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

namespace EZBinarySerializer.ValueSerializers {
    class TestNamespaceDictionaryBookValueSerializer : IValueSerializer<global::TestNamespace.DictionaryBook> {
        public static int FromBinary(Span<byte> data, out global::TestNamespace.DictionaryBook value) {
            int cursor = 0;
            cursor += global::EZBinarySerializer.Tests.BinarySerializer.FromBinary(
                data[
                    cursor..
                ],
                out int size
            );
            cursor += global::EZBinarySerializer.Tests.BinarySerializer.FromBinary(
                data[
                    cursor..
                ],
                out string typeName
            );
            cursor += SystemStringSystemStringSystemCollectionsGenericDictionaryValueSerializer.FromBinary(
                data[cursor..],
                out global::System.Collections.Generic.Dictionary<global::System.String, global::System.String> __ez__definitionsbywordname
            );
            value = new() {
                DefinitionsByWordName = __ez__definitionsbywordname,
            };

            return cursor;
        }
        public static Memory<byte> ToBinary(global::TestNamespace.DictionaryBook value) {
            int size = 0;
            List<Memory<byte>> bytesList = [];
            bytesList.Add(SystemStringSystemStringSystemCollectionsGenericDictionaryValueSerializer.ToBinary(
                value.DefinitionsByWordName
            ));
            size += bytesList[bytesList.Count - 1].Length;
            var typeNameBytes = global::EZBinarySerializer.Tests.BinarySerializer.ToBinary(value.FullyQualifiedTypeName);
            size += typeNameBytes.Length;
            size += sizeof(int);
            var sizeBytes = global::EZBinarySerializer.Tests.BinarySerializer.ToBinary(size);
            Memory<byte> data = new byte[size];
            int cursor = 0;
            sizeBytes.CopyTo(
                data[cursor..(cursor += sizeBytes.Length)]
            );
            typeNameBytes.CopyTo(
                data[cursor..(cursor += typeNameBytes.Length)]
            );
            foreach (var bytes in bytesList) {
                bytes.CopyTo(
                    data[cursor..(cursor += bytes.Length)]
                );
            }
            
            return data;
        }
    }
}
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out global::TestNamespace.DictionaryBook value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceDictionaryBookValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(global::TestNamespace.DictionaryBook value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceDictionaryBookValueSerializer.ToBinary(value);
        }
    }
}
namespace TestNamespace {
    public partial class DictionaryBook : global::EZBinarySerializer.IBinarySerializable {
        public override string FullyQualifiedTypeName {
            get {
                return "global::TestNamespace.DictionaryBook";
            }
        }
        public new static Memory<byte> ToBinary(global::EZBinarySerializer.IBinarySerializable value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceDictionaryBookValueSerializer.ToBinary((global::TestNamespace.DictionaryBook)value);
        }

        public new static int FromBinary(Span<byte> data, out global::EZBinarySerializer.IBinarySerializable value) {
            int size = global::EZBinarySerializer.ValueSerializers.TestNamespaceDictionaryBookValueSerializer.FromBinary(data, out global::TestNamespace.DictionaryBook result);
            value = (global::EZBinarySerializer.IBinarySerializable)result;
            return size;
        }
    }
}