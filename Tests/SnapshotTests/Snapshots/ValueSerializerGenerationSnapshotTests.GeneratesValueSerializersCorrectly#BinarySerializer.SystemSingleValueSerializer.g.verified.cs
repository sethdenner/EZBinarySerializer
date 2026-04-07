//HintName: BinarySerializer.SystemSingleValueSerializer.g.cs

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
    public class SystemSingleValueSerializer : IValueSerializer<float> {
        public static int FromBinary(Span<byte> data, out float value) {
            value = BitConverter.ToSingle(
                data[..sizeof(float)]
            );
            return sizeof(float);
        }

        public static Memory<byte> ToBinary(float value) {
            if (value is float f) {
                return BitConverter.GetBytes(f);
            } else {
                return Memory<byte>.Empty;
            }
        }
    }
}
