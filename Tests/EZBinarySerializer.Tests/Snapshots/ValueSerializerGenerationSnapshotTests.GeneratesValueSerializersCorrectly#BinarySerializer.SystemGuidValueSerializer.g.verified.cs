//HintName: BinarySerializer.SystemGuidValueSerializer.g.cs

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
    public class SystemGuidValueSerializer : IValueSerializer<Guid> {
        public static int FromBinary(Span<byte> data, out Guid value) {
            value = new(
                data[..16]
            );
            return 16;
        }

        public static Memory<byte> ToBinary(Guid value) {
            var result = new byte[16];
            value.TryWriteBytes(result);
            return result;
        }
    }
}
namespace EZBinarySerializer.Tests {
    public partial class BinarySerializer {
        public static int FromBinary(Span<byte> data, out Guid value) {
            return EZBinarySerializer.ValueSerializers.SystemGuidValueSerializer.FromBinary(data, out value);
        }

        public static Memory<byte> ToBinary(Guid value) {
            return EZBinarySerializer.ValueSerializers.SystemGuidValueSerializer.ToBinary(value);
        }
    }
}
