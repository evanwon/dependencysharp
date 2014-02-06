DependencySharp for .NET
=========================

*Unmanaged Dependencies Suck*™

This library allows you to embed unmanaged dependencies within your managed code (as a byte array), and at runtime check if the dependency exists in a specified location. If it doesn't, the dependency is automatically written to the correct location.

## How to Install ##

[DependencySharp is available via NuGet](https://www.nuget.org/packages/DependencySharp/).

## Usage ##

1. Add your unmanaged dependencies to your managed library as a binary resource. 
  1. To do this: Navigate to **Project Properties** in Visual Studio
  2. Click the **Resources** tab (a prompt may notify you that a default Resources file does not exist - create it)
  3. Click **Add Resource** and select the unmanaged dependency. It will now be added to your project as a byte array.

Before you call a method that requires an external dependency, use DependencySharp to check for its existence, and it will automatically create the file if it is missing or (optionally) if the version is out-of-date.

Example, with an unmanaged DLL called `Interop.CoreScanner.dll`, which I expect to exist in the same folder where my application is executing from:

```csharp
	// The full path where you expect the dependency to exist on disk
	// Helper methods are included to provide easy access to the directory
	// where your assmebly is being executed from (which is where
	// you typically need to place unmanaged dependencies).
	var expectedPath = AssemblyUtilities.ExecutingAssemblyPath + "Interop.CoreScanner.dll";

	// The dependency as a byte array, stored in your library's Properties/Resources.resx file
	var dependency = Resources.Interop_CoreScanner;

	// (optional) the expected version of the dependency on disk
	var expectedVersion = new Version(1, 0, 1);

	var dependencies = new List<UnmanagedDependency>()
	{
		new UnmanagedDependency(expectedPath, dependency, expectedVersion)
	};

	var dependencyManager = new DependencyManager();

	dependencyManager.VerifyDependenciesAndExtractIfMissing(dependencies);
```
