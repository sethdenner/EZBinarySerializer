# Build the NuGet package
dotnet pack ./EZBinarySerializer -c Release -o ./artifacts -p:Version=0.1.4-beta -p:Authors='Seth Adam Denner' -p:Description='Easily serialize and deserialize instances using a simple attribute based interface. Supports polymorphism and many standard types including System.Collections.Generic and System.Numerics types. Uses zero reflection maintaining AOT compatibility for your game development embeded systems and other applications where performance and portability is desired.'
# Restore the project using the custom config file, restoring packages to a local folder
dotnet restore ./Tests/NugetIntegrationTests --packages ./packages --configfile "./Tests/NugetIntegrationTests/nuget.integration-tests.config" 
# Build the project (no restore), using the packages restored to the local folder
dotnet build ./Tests/NugetIntegrationTests -c Release --packages ./packages --no-restore
# Test the project (no build or restore)
dotnet test ./Tests/NugetIntegrationTests -c Release --no-build --no-restore
