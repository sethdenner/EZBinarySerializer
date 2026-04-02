//HintName: BinarySerializer.SystemInt32ValueSerializer.g.cs

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
    public class SystemInt32ValueSerializer : IValueSerializer<int> {
        public static int FromBinary(Span<byte> data, out int value) {
            value = BitConverter.ToInt32(data);
            return sizeof(int);
        }

        public static Memory<byte> ToBinary(int value) {
            return BitConverter.GetBytes(value);
        }
    }
}
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out int value) {
            return EZBinarySerializer.ValueSerializers.SystemInt32ValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(int value) {
            return EZBinarySerializer.ValueSerializers.SystemInt32ValueSerializer.ToBinary(value);
        }
    }
}
