using EZBinarySerializer;
using SampleModel;
using System.Numerics;

namespace IntegrationTests.Model {
    [BinarySerializable]
    public partial class Auditorium : IBuilding {
        public Vector2 GeoCoordinates {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public bool Equals(IBuilding? other) {
            throw new NotImplementedException();
        }
    }
}
