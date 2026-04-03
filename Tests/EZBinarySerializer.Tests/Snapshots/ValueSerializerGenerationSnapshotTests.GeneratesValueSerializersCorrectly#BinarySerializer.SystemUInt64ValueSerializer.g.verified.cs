//HintName: BinarySerializer.SystemUInt64ValueSerializer.g.cs

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
    public class SystemUInt64ValueSerializer : IValueSerializer<ulong> {
        public static int FromBinary(Span<byte> data, out ulong value) {
            value = BitConverter.ToUInt64(
                data[..sizeof(ulong)]
            );
            return sizeof(ulong);
        }

        public static Memory<byte> ToBinary(ulong value) {
            if (value is ulong u) {
                return BitConverter.GetBytes(u);
            } else {
                return Memory<byte>.Empty;
            }
        }
    }
}
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out ulong value) {
            return global::EZBinarySerializer.ValueSerializers.SystemUInt64ValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(ulong value) {
            return global::EZBinarySerializer.ValueSerializers.SystemUInt64ValueSerializer.ToBinary(value);
        }
    }
}
