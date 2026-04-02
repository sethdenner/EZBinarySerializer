//HintName: BinarySerializer.SystemDoubleValueSerializer.g.cs

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
    public class SystemDoubleValueSerializer : IValueSerializer<double> {
        public static int FromBinary(Span<byte> data, out double value) {
            value = BitConverter.ToDouble(
                data[..sizeof(double)]
            );
            return sizeof(double);
        }

        public static Memory<byte> ToBinary(double value) {
            if (value is double d) {
                return BitConverter.GetBytes(d);
            } else {
                return Memory<byte>.Empty;
            }
        }
    }
}
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out double value) {
            return EZBinarySerializer.ValueSerializers.SystemDoubleValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(double value) {
            return EZBinarySerializer.ValueSerializers.SystemDoubleValueSerializer.ToBinary(value);
        }
    }
}
