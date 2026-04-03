//HintName: BinarySerializer.SystemInt16ValueSerializer.g.cs

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
    public class SystemInt16ValueSerializer : IValueSerializer<short> {
        public static int FromBinary(Span<byte> data, out short value) {
            value = BitConverter.ToInt16(
                data[..sizeof(short)]
            );
            return sizeof(short);
        }

        public static Memory<byte> ToBinary(short value) {
            if (value is short s) {
                return BitConverter.GetBytes(s);
            } else {
                return Memory<byte>.Empty;
            }
        }
    }
}
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out short value) {
            return global::EZBinarySerializer.ValueSerializers.SystemInt16ValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(short value) {
            return global::EZBinarySerializer.ValueSerializers.SystemInt16ValueSerializer.ToBinary(value);
        }
    }
}
