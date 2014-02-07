DependencySharp for .NET
=========================

*Unmanaged Dependencies Suck*™

This library allows you to embed unmanaged COM libraries within your managed library (as a byte array), and at runtime check if the unmanaged library exists in a specified location. If it doesn't, the library is automatically written to the specified location.

These "self-contained" libraries are capable of managing their own unmanaged dependencies, providing a convenient way for your end-user to use and deploy your managed library.

## How to Install ##

[DependencySharp is available via NuGet](https://www.nuget.org/packages/DependencySharp/).

## Purpose of this library

Unmanaged COM libraries are often required for interaction with 3rd-party systems, and may be provided by an external vendor so you don't have access to its source.

There are three basic types of libraries in the Windows world:

1. **Unmanaged COM:** Typically must exist in `C:\Windows\System32` or `C:\Windows\SysWOW64` and registered with `regsvr32`. 
2. **[Unmanaged, registry-free COM](http://msdn.microsoft.com/en-us/library/ms973913.aspx):** Typically must exist in same directory as executing assembly
3. **Managed:** These are your standard "modern" .NET libraries (like DependencySharp); Visual Studio, the .NET Framework, and ClickOnce know how to handle these dependencies without assistance

DependencySharp helps with the second type: **unmanaged, registry-free COM** libraries. Since these dependencies aren't built on the .NET Framework, our modern development tools like Visual Studio and the ClickOnce deployment system are very limited.

Unmanaged COM dependencies can be cumbersome when deploying software with the ClickOnce deployment system. ClickOnce was built to deal with *managed* dependencies, and it does a great job with those. It's so easy you probably don't even think about it; managed depenedencies are identified by the system and automatically included in your deployment.

To deploy *unmanaged* dependencies with ClickOnce, you'd normally have to misuse Visual Studio's build parameters to include the dependency as "content" in your project (not a Reference). NuGet can help automate this process, but you have to script out an `install` PowerShell script to configure the dependencies in the project. If you need your end-users to update your managed library and also its unmanaged dependencies, this can get even trickier.

DependencySharp lets you piggy-back unmanaged libraries within a managed library, to create what I call a "self-contained DLL". This is possible because unmanaged libraries only need to exist *at runtime*, and they aren't required for compilation. 

If you've already created a managed wrapper for your unmanaged library, you can **embed the unmanaged library as a byte resource within the managed library**. Then, in your managed object's constructor, use methods provided by DependencySharp to automatically check if specific unmanaged dependencies exist, and write them to disk if necessary.

The result: your end-user gets a single dependency that takes care of its own dependencies. Think of it as a piñata full of dependency goodies.

![](http://i.imgur.com/vO8CH9u.png)

### Pros

- Visual Studio, ClickOnce, and NuGet love managed dependencies
- Consumers of self-contained DLLs don't need to worry (or even know about) about handling unmanaged DLLs
- Deployment and installation of software is simplified
- Unmanaged libraries can be verified on a variety of attributes: file size, version, path, etc.

### Cons

- Developers of the self-contained libraries need to know what they're doing
- Minor development overheard to build the self-contained library
- Potential for exceptions if unmanaged libraries can't be written to disk (although ClickOnce's deployment location is in a per-user, writable location)

## Basic Usage ##

First, add your unmanaged dependencies to your managed library as a byte resource. 

1. Navigate to **Project Properties** in Visual Studio
2. Click the **Resources** tab (a prompt may notify you that a default Resources file does not exist - create it)
3. Click **Add Resource** and select the unmanaged dependency. It will now be added to your project as a byte array.

Before you call a method that requires an external dependency, use DependencySharp to check for its existence, and it will automatically create the file if it is missing or (optionally) if the version is out-of-date.

Here's a basic example, with an unmanaged DLL called `Interop.CoreScanner.dll`, which I expect to exist in the same folder where my application is executing from:

```csharp
using DependencySharp;

// The full path where you expect the dependency to exist on disk
// Helper methods are included to provide easy access to the directory
// where your assmebly is being executed from (which is where
// you typically need to place unmanaged dependencies).
var expectedPath = Path.Combine(AssemblyUtilities.ExecutingAssemblyPath, "Interop.CoreScanner.dll");

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

If you need to update the unmanaged dependency in the future, just remove the old dependency resource and add the updated resource, and if necessary, update the version identifier in your code.

## Advanced Example

DependencySharp also contains a method for executing an [`Action`](http://msdn.microsoft.com/en-us/library/system.action%28v=vs.110%29.aspx) after the unmanaged dependency is written to disk.

For example, I recently worked with some hardware that required dozens of unmanaged libraries to interact with the system. Instead of adding each individual library to my managed library, I used [7-Zip](http://www.7-zip.org/) to create a self-extracting executable which contained all required libraries, and bundled that with my managed library. After the executable was extracted, I used an `Action` to provide commands to extract the bundle on disk.

```csharp
private static void HandleUnmanagedDependencies()
{
	// Verify unmanaged dependencies exist on disk
	// There are ~73 dependencies, so they've been combined into
	// a self-extracting executable for ease of deployment
	
	var expectedPath = Path.Join(AssemblyDirectory, "self-extracting-lib.exe");
	var dependency = Properties.Resources.self-extracting-lib;
	var dependencies = new List<UnmanagedDependency>
						   {
							   new UnmanagedDependency(expectedPath, dependency)
						   };

	// This action will be executed after the dependencies are extracted
	var postExtractAction = new Action(
		() =>
			{
				// Look for a known dependency stored within the self-extracting
				// executable. If it doesn't exist, expand the executable
				if (!File.Exists(Path.Combine(AssemblyDirectory, "SerialPorts.dll")))
				{
					var processStartInfo = new ProcessStartInfo()
											   {
												   FileName = expectedPath,
												   Arguments = "/S",
												   UseShellExecute = true,
												   CreateNoWindow = true
											   };

					var process = new Process() { StartInfo = processStartInfo };

					process.Start();

					Debug.WriteLine("Waiting for dependency extraction to complete...");
					process.WaitForExit(30000);
				}
			});
			
	var dependencyManager = new DependencyManager();
	dependencyManager.VerifyDependenciesAndExtractIfMissingThenPerformAction(dependencies, postExtractAction);
}
```

## Feedback

Is this crazy? Want to contribute? I'd love to hear from you.
