//HintName: BinarySerializer.SystemInt64ValueSerializer.g.cs

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
    public class SystemInt64ValueSerializer : IValueSerializer<long> {
        public static int FromBinary(Span<byte> data, out long value) {
            value = BitConverter.ToInt64(
                data[..sizeof(long)]
            );
            return sizeof(long);
        }

        public static Memory<byte> ToBinary(long value) {
            if (value is long l) {
                return BitConverter.GetBytes(l);
            } else {
                return Memory<byte>.Empty;
            }
        }
    }
}
