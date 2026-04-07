//HintName: BinarySerializer.TestNamespaceBookValueSerializer.g.cs

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
    public class TestNamespaceBookValueSerializer : IValueSerializer<global::TestNamespace.Book> {
        public static int FromBinary(Span<byte> data, out global::TestNamespace.Book value) {
            string typeName = IBinarySerializable.PeekTypeName(data);
            int size = global::EZBinarySerializer.BinarySerializer.DeserializerMethodsByTypeName[typeName](
                data,
                out IBinarySerializable serializable
            );
            value = (global::TestNamespace.Book)serializable;
            return size;
        }

        public static Memory<byte> ToBinary(global::TestNamespace.Book value) {
            return global::EZBinarySerializer.BinarySerializer.SerializerMethodsByTypeName[value.FullyQualifiedTypeName](
                value as global::EZBinarySerializer.IBinarySerializable
            );
        }
    }
}
namespace TestNamespace {
    public partial class Book : global::EZBinarySerializer.IBinarySerializable {
        public abstract string FullyQualifiedTypeName { get; }
        public static Memory<byte> ToBinary(global::EZBinarySerializer.IBinarySerializable value) {
            return global::EZBinarySerializer.ValueSerializers.TestNamespaceBookValueSerializer.ToBinary((global::TestNamespace.Book)value);
        }

        public static int FromBinary(Span<byte> data, out global::EZBinarySerializer.IBinarySerializable value) {
            int size = global::EZBinarySerializer.ValueSerializers.TestNamespaceBookValueSerializer.FromBinary(data, out global::TestNamespace.Book result);
            value = (global::EZBinarySerializer.IBinarySerializable)result;
            return size;
        }
    }
}