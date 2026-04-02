using System.Runtime.CompilerServices;

namespace EZBinarySerializer.Tests {
    public static class ModuleInitializer {
        [ModuleInitializer]
        public static void Init() {
            VerifySourceGenerators.Initialize();
        }
    }
}
