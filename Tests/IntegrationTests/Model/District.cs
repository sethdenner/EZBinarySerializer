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
using EZBinarySerializer;
using System.Security.Cryptography;

namespace IntegrationTests.Model {
    [BinarySerializable]
    public partial class District : IEquatable<District> {
        public List<IBuilding> Buildings = [];

        public bool Equals(District? other) {
            if (
                Buildings.Count != other?.Buildings.Count
            ) {
                return false;
            }

            for (int i = 0; i < Buildings.Count; ++i) {
                var building = Buildings[i];
                var otherBuilding = other?.Buildings[i];
                if (!building.Equals(otherBuilding)) {
                    return false;
                }
            }

            return true;
        }
        public static bool operator ==(District district1, District district2) {
            if (district1 is null) {
                return district2 is null;
            }

            return district1.Equals(district2);
        }

        public static bool operator !=(District district1, District district2) {
            if (district1 is null) {
                return district2 is not null;
            }

            return !district1.Equals(district2);
        }

        public override bool Equals(object? obj) {
            return Equals(obj as District);
        }

        public override int GetHashCode() {
            return BitConverter.ToInt32(
                MD5.HashData(
                    ToBinary(this).ToArray()
                ).AsSpan()[..sizeof(int)]
            );
        }
    }
}
