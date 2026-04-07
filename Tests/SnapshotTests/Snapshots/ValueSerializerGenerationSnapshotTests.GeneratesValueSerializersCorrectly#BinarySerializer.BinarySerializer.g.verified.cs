//HintName: BinarySerializer.BinarySerializer.g.cs

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

namespace EZBinarySerializer {
    public partial class BinarySerializer {
        public delegate Memory<byte> SerializerDelegate(global::EZBinarySerializer.IBinarySerializable value);
        public delegate int DeserializerDelegate(Span<byte> data, out global::EZBinarySerializer.IBinarySerializable value);
        public static Dictionary<string, SerializerDelegate> SerializerMethodsByTypeName = new() {
            { "global::TestNamespace.DictionaryBook", global::TestNamespace.DictionaryBook.ToBinary },
            { "global::TestNamespace.Library", global::TestNamespace.Library.ToBinary },
            { "global::TestNamespace.Subject", global::TestNamespace.Subject.ToBinary },
            { "global::TestNamespace.Student", global::TestNamespace.Student.ToBinary },
            { "global::TestNamespace.Teacher", global::TestNamespace.Teacher.ToBinary }
        };
        public static Dictionary<string, DeserializerDelegate> DeserializerMethodsByTypeName = new() {
            { "global::TestNamespace.DictionaryBook", global::TestNamespace.DictionaryBook.FromBinary },
            { "global::TestNamespace.Library", global::TestNamespace.Library.FromBinary },
            { "global::TestNamespace.Subject", global::TestNamespace.Subject.FromBinary },
            { "global::TestNamespace.Student", global::TestNamespace.Student.FromBinary },
            { "global::TestNamespace.Teacher", global::TestNamespace.Teacher.FromBinary }
        };
    }
}
