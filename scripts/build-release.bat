@ECHO OFF

:: Location of msbuild - using .NET 4. Change if necessary.
SET msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

IF '%1'=='' (SET configuration=Release) ELSE (SET configuration=%1)
IF '%2'=='' (SET platform="Any CPU") ELSE (SET platform=%2)

%msbuild% "../source/DependencySharp.sln" /t:Rebuild /nologo /property:Platform=%platform% /property:Configuration=%configuration% /property:DebugType=None /property:AllowedReferenceRelatedFileExtensions=- /verbosity:minimal /flp:verbosity=normal;logfile=build-release.log 

PAUSE

:: IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%