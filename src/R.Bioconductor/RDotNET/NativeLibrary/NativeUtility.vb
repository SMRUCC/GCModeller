#Region "Microsoft.VisualBasic::5d885645641e533c81f7964efd327058, RDotNET\NativeLibrary\NativeUtility.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class NativeUtility
    ' 
    '         Properties: IsUnix, Registry, SetEnvironmentVariablesLog
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ConstructRPath, CreateNew, ExecCommand, FindRHome, FindRPath
    '                   FindRPathFromRegistry, FindRPathMacOS, FindRPaths, FindRPathUnix, FindRPathWindows
    '                   GetPlatform, GetRCoreRegistryKey, GetRCoreRegistryKeyWin32, GetRCurrentVersionStringFromRegistry, GetRHomeEnvironmentVariable
    '                   GetRhomeWin32NT, GetRInstallPathFromRCoreKegKey, GetRLibraryFileName, GetRVersionFromRegistry, GetShortPath
    '                   GetShortPathName, PrependToEnv, RecurseFirstSubkey
    ' 
    '         Sub: CheckDirExists, CheckPlatformWin32, doFoundWinRegKey, doLogSetEnvVar, doLogSetEnvVarInfo
    '              doLogSetEnvVarWarn, FindRPaths, SetEnvironmentVariables, SetenvPrepend
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language.Default

Namespace NativeLibrary

    ''' <summary>
    ''' Collection of utility methods for operating systems.
    ''' </summary>
    Public Class NativeUtility
        ''' <summary> Gets or sets the registry.</summary>
        '''
        ''' <value> The registry.</value>
        Protected Property Registry() As IRegistry
            Get
                Return m_Registry
            End Get
            Set
                m_Registry = Value
            End Set
        End Property
        Private m_Registry As IRegistry

        ''' <summary> Constructor.</summary>
        '''
        ''' <param name="registry"> (Optional)
        '''                         The registry.</param>
        Public Sub New(Optional registry As IRegistry = Nothing)
            Me.Registry = (If(registry Is Nothing, New WindowsRegistry(), registry))
        End Sub

        Public Shared Function CreateNew() As NativeUtility
            Return New NativeUtility()
        End Function

        ''' <summary>
        ''' Gets the platform on which the current process runs.
        ''' </summary>
        ''' <remarks>
        ''' <see cref="Environment.OSVersion"/>'s platform is not <see cref="PlatformID.MacOSX"/> even on Mac OS X.
        ''' This method returns <see cref="PlatformID.MacOSX"/> when the current process runs on Mac OS X.
        ''' This method uses UNIX's uname command to check the operating system,
        ''' so this method cannot check the OS correctly if the PATH environment variable is changed (will returns <see cref="PlatformID.Unix"/>).
        ''' </remarks>
        ''' <returns>The current platform.</returns>
        Public Shared Function GetPlatform() As PlatformID
            Static curPlatform As PlatformID? = Nothing
            Static OSX As [Default](Of  PlatformID) = PlatformID.MacOSX

            If (Not curPlatform.HasValue) Then

                Dim platform = Environment.OSVersion.Platform

                If (platform <> PlatformID.Unix) Then

                    curPlatform = platform

                Else

                    Try
                        Dim kernelName$ = ExecCommand("uname", "-s")
                        curPlatform = platform Or OSX.When(kernelName = "Darwin")
                    Catch ex As Win32Exception
                        ' probably no PATH to uname.
                        curPlatform = platform
                    End Try
                End If
            End If

            Return curPlatform.Value
        End Function

        ''' <summary>
        ''' Execute a command in a new process
        ''' </summary>
        ''' <param name="processName">Process name e.g. "uname"</param>
        ''' <param name="arguments">Arguments e.g. "-s"</param>
        ''' <returns>The output of the command to the standard output stream</returns>
        Public Shared Function ExecCommand(processName As String, arguments As String) As String
            Using proc As New Process()
                proc.StartInfo.FileName = processName
                proc.StartInfo.Arguments = arguments
                proc.StartInfo.RedirectStandardOutput = True
                proc.StartInfo.UseShellExecute = False
                proc.StartInfo.CreateNoWindow = True
                proc.Start()

                Dim stdout$ = proc.StandardOutput.ReadLine()
                Call proc.WaitForExit()
                Return stdout
            End Using
        End Function

        Private Shared logSetEnvVar As New StringBuilder()

        ''' <summary>
        ''' Gets a log of the changes made to environment variables via the NativeUtility
        ''' </summary>
        Public Shared ReadOnly Property SetEnvironmentVariablesLog() As String
            Get
                Return logSetEnvVar.ToString()
            End Get
        End Property

        ''' <summary>
        ''' Sets the PATH to the R binaries and R_HOME environment variables if needed.
        ''' </summary>
        ''' <param name="rPath">The path of the directory containing the R native library.
        ''' If null (default), this function tries to locate the path via the Windows registry, or commonly used locations on MacOS and Linux</param>
        ''' <param name="rHome">The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set,
        ''' the function uses platform specific sensible default behaviors.</param>
        ''' <remarks>
        ''' This function has been designed to limit the tedium for users, while allowing custom settings for unusual installations.
        ''' </remarks>
        Public Sub SetEnvironmentVariables(Optional rPath As String = Nothing, Optional rHome As String = Nothing)
            '
            '             * Changing the behavior in Oct 2014, following the report of
            '             * https://rdotnet.codeplex.com/workitem/140
            '             * Use rHome, whether from the method parameter or from the environment variable,
            '             * to deduce the path to the binaries, in preference to the registry key.
            '             


            logSetEnvVar.Clear()

            Dim platform = GetPlatform()
            If rPath IsNot Nothing Then
                CheckDirExists(rPath)
            End If
            If rHome IsNot Nothing Then
                CheckDirExists(rHome)
            End If

            FindRPaths(rPath, rHome, logSetEnvVar)

            If String.IsNullOrEmpty(rHome) Then
                Throw New NotSupportedException("R_HOME was not provided and a suitable path could not be found by R.NET")
            End If
            SetenvPrepend(rPath)
            ' It is highly recommended to use the 8.3 short path format on windows.
            ' See the manual page of R.home function in R. Solves at least the issue R.NET 97.
            If platform = PlatformID.Win32NT Then
                rHome = GetShortPath(rHome)
            End If
            If Not Directory.Exists(rHome) Then
                Throw New DirectoryNotFoundException(String.Format("Directory '{0}' does not exist - cannot set the environment variable R_HOME to that value", rHome))
            End If
            Environment.SetEnvironmentVariable("R_HOME", rHome)
            ' Let's check that LD_LIBRARY_PATH is set if this is a custom installation of R.
            ' Normally in an R session from a custom build/install we get something typically like:
            ' > Sys.getenv('LD_LIBRARY_PATH')
            ' [1] "/usr/local/lib/R/lib:/usr/local/lib:/usr/lib/jvm/java-7-openjdk-amd64/jre/lib/amd64/server"
            ' The R script sets LD_LIBRARY_PATH before it starts the native executable under e.g. /usr/local/lib/R/bin/exec/R
            ' This would be useless to set LD_LIBRARY_PATH in the current function:
            ' it must be set as en env var BEFORE the process is started (see man page for dlopen)
            ' so all we can do is an intelligible error message for the user, explaining he needs to set the LD_LIBRARY_PATH env variable
            ' Let's delay the notification about a missing LD_LIBRARY_PATH till loading libR.so fails, if it does.
            If platform = PlatformID.Unix Then
            End If
        End Sub

        ''' <summary>
        ''' A method to help diagnose the environment variable setup process. 
        ''' This function does not change the environment, this is purely a "dry run"
        ''' </summary>
        ''' <param name="rPath">The path of the directory containing the R native library.
        ''' If null (default), this function tries to locate the path via the Windows registry, or commonly used locations on MacOS and Linux</param>
        ''' <param name="rHome">The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set,
        ''' the function uses platform specific sensible default behaviors.</param>
        ''' <returns>A console friendly output of the paths discovery process</returns>
        Public Function FindRPaths(ByRef rPath As String, ByRef rHome As String) As String
            Dim logger As New StringBuilder()
            FindRPaths(rPath, rHome, logger)
            Return logger.ToString()
        End Function

        Private Sub FindRPaths(ByRef rPath As String, ByRef rHome As String, logSetEnvVar As StringBuilder)
            doLogSetEnvVarInfo(String.Format("caller provided rPath={0}, rHome={1}", If(rPath Is Nothing, "null", rPath), If(rHome Is Nothing, "null", rHome)), logSetEnvVar)

            If String.IsNullOrEmpty(rHome) Then
                rHome = GetRHomeEnvironmentVariable()
                doLogSetEnvVarInfo(String.Format("R.NET looked for preset R_HOME env. var. Found {0}", If(rHome Is Nothing, "null", rHome)), logSetEnvVar)
            End If
            If String.IsNullOrEmpty(rHome) Then
                rHome = FindRHome(rPath:=Nothing, logger:=logSetEnvVar)
                doLogSetEnvVarInfo(String.Format("R.NET looked for platform-specific way (e.g. win registry). Found {0}", If(rHome Is Nothing, "null", rHome)), logSetEnvVar)
                If Not String.IsNullOrEmpty(rHome) Then
                    If rPath Is Nothing Then
                        rPath = FindRPath(rHome)
                        doLogSetEnvVarInfo(String.Format("R.NET trying to find rPath based on rHome; Deduced {0}", If(rPath Is Nothing, "null", rPath)), logSetEnvVar)
                    End If
                    If rPath Is Nothing Then
                        rPath = FindRPath()
                        doLogSetEnvVarInfo(String.Format("R.NET trying to find rPath, independently of rHome; Deduced {0}", If(rPath Is Nothing, "null", rPath)), logSetEnvVar)
                    End If
                Else
                    rHome = FindRHome(rPath)
                    doLogSetEnvVarInfo(String.Format("R.NET trying to find rHome based on rPath; Deduced {0}", If(rHome Is Nothing, "null", rHome)), logSetEnvVar)
                End If
            End If
            If String.IsNullOrEmpty(rHome) Then
                doLogSetEnvVar("Error", "R_HOME was not provided and a suitable path could not be found by R.NET", logSetEnvVar)
            End If
        End Sub

        Private Sub doLogSetEnvVar(level As String, msg As String, logSetEnvVar As StringBuilder)
            If logSetEnvVar IsNot Nothing Then
                logSetEnvVar.Append(level)
                logSetEnvVar.Append(": ")
                logSetEnvVar.AppendLine(msg)
            End If
        End Sub

        Private Sub doLogSetEnvVarWarn(msg As String, logger As StringBuilder)
            doLogSetEnvVar("Warn", msg, logger)
        End Sub

        Private Sub doLogSetEnvVarInfo(msg As String, logger As StringBuilder)
            doLogSetEnvVar("Info", msg, logger)
        End Sub

        Private Sub doFoundWinRegKey(rCore As IRegistryKey, logger As StringBuilder)
            doLogSetEnvVarInfo(String.Format("Found Windows registry key {0}", rCore.ToString()), logger)
        End Sub


        Private Shared Function GetShortPath(path As String) As String
            Dim shortPath = New StringBuilder(MaxPathLength)
            GetShortPathName(path, shortPath, MaxPathLength)
            Return shortPath.ToString()
        End Function

        Private Const MaxPathLength As Integer = 248
        'MaxPath is 248. MaxFileName is 260.
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto)>
        Private Shared Function GetShortPathName(<MarshalAs(UnmanagedType.LPTStr)> path As String, <MarshalAs(UnmanagedType.LPTStr)> shortPath As StringBuilder, shortPathLength As Integer) As Integer
        End Function

        ''' <summary>
        ''' Gets the value, if any, of the R_HOME environment variable of the current process
        ''' </summary>
        ''' <returns>The value, or null if not set</returns>
        Public Shared Function GetRHomeEnvironmentVariable() As String
            Return Environment.GetEnvironmentVariable("R_HOME")
        End Function

        ''' <summary>
        ''' Try to locate the directory path to use for the R_HOME environment variable. This is used by R.NET by default; users may want to use it to diagnose problematic behaviors.
        ''' </summary>
        ''' <param name="rPath">Optional path to the directory containing the R shared library. This is ignored unless on a Unix platform (i.e. ignored on Windows and MacOS)</param>
        ''' <param name="logger">Optional logger for diagnosis</param>
        ''' <returns>The path that R.NET found suitable as a candidate for the R_HOME environment</returns>
        Public Function FindRHome(Optional rPath As String = Nothing, Optional logger As StringBuilder = Nothing) As String
            Dim platform = GetPlatform()
            Dim rHome As String
            Select Case platform
                Case PlatformID.Win32NT
                    ' We need here to guess, process and set R_HOME
                    ' Rf_initialize_R for gnuwin calls get_R_HOME which scans the windows registry and figures out R_HOME; however for
                    ' unknown reasons in R.NET we end up with long path names, whereas R.exe ends up with the short, 8.3 path format.
                    ' Blanks in the R_HOME environment variable cause trouble (e.g. for Rcpp), so we really must make sure
                    ' that rHome is a short path format. Here we retrieve the path possibly in long format, and process to short format later on
                    ' to capture all possible sources of R_HOME specifications
                    ' Behavior added to fix issue
                    rHome = GetRhomeWin32NT(logger)
                    Exit Select

                Case PlatformID.MacOSX
                    rHome = "/Library/Frameworks/R.framework/Resources"
                    Exit Select

                Case PlatformID.Unix
                    If Not String.IsNullOrEmpty(rPath) Then
                        ' if rPath is e.g. /usr/local/lib/R/lib/ ,
                        rHome = Path.GetDirectoryName(rPath)
                    Else
                        rHome = "/usr/lib/R"
                    End If
                    If Not rHome.EndsWith("R") Then
                        ' if rPath is e.g. /usr/lib/ (symlink)  then default
                        rHome = "/usr/lib/R"
                    End If
                    Exit Select
                Case Else

                    Throw New NotSupportedException(platform.ToString())
            End Select
            Return rHome
        End Function

        Private Function GetRhomeWin32NT(logger As StringBuilder) As String
            Dim rCoreKey As IRegistryKey = GetRCoreRegistryKeyWin32(logger)
            Return GetRInstallPathFromRCoreKegKey(rCoreKey, logger)
        End Function

        Private Shared Sub CheckDirExists(rPath As String)
            If Not Directory.Exists(rPath) Then
                Throw New ArgumentException(String.Format("Specified directory not found: '{0}'", rPath))
            End If
        End Sub

        Private Function ConstructRPath(rHome As String) As String
            Dim shlibFilename = GetRLibraryFileName()
            Dim platform = GetPlatform()
            Select Case platform
                Case PlatformID.Win32NT
                    Dim rPath = Path.Combine(rHome, "bin")
                    Dim rVersion As Version = GetRVersionFromRegistry()
                    If rVersion.Major > 2 OrElse (rVersion.Major = 2 AndAlso rVersion.Minor >= 12) Then
                        Dim bitness = If(Environment.Is64BitProcess, "x64", "i386")
                        rPath = Path.Combine(rPath, bitness)
                    End If
                    Return rPath
                Case Else

                    Throw New PlatformNotSupportedException()
            End Select
        End Function

        Private Function GetRCoreRegistryKey(logger As StringBuilder) As IRegistryKey
            If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                Return Nothing
            End If
            Return GetRCoreRegistryKeyWin32(logger)
        End Function

        ''' <summary>
        ''' Gets the R version from the Windows R Registry (if available)
        ''' </summary>
        ''' <returns>a System.Version object</returns>
        Public Function GetRVersionFromRegistry(Optional logger As StringBuilder = Nothing) As Version
            Dim rCoreKey = GetRCoreRegistryKey(logger)
            Dim version = GetRCurrentVersionStringFromRegistry(rCoreKey)
            If String.IsNullOrEmpty(version) Then
                Dim subKeyNames As String() = rCoreKey.GetSubKeyNames()
                If subKeyNames.Length > 0 Then
                    version = subKeyNames(0)
                End If
            End If
            ' Version should normally be "3.2.4", but some releases had
            ' "3.2.4 Revised" in which case constructor for Version fails.
            ' The regex first extracts the numerical part of the string.
            Dim reg = New Regex("([0-9]*\.)*[0-9]*")
            Return New Version(reg.Match(version).Value)
        End Function

        Private Shared Function GetRCurrentVersionStringFromRegistry(rCoreKey As IRegistryKey) As String
            Return TryCast(rCoreKey.GetValue("Current Version"), String)
        End Function

        ''' <summary>
        ''' Attempt to find a suitable path to the R shared library. This is used by R.NET by default; users may want to use it to diagnose problematic behaviors.
        ''' </summary>
        ''' <returns>The path to the directory where the R shared library is expected to be</returns>
        Public Function FindRPath(Optional rHome As String = Nothing) As String
            Dim platform = GetPlatform()
            Select Case platform
                Case PlatformID.Win32NT
                    Return FindRPathWindows(rHome)

                Case PlatformID.MacOSX
                    Return FindRPathMacOS(rHome)

                Case PlatformID.Unix
                    Return FindRPathUnix(rHome)
                Case Else

                    Throw New PlatformNotSupportedException()
            End Select
        End Function

        Private Function FindRPathUnix(rHome As String) As String
            ' TODO: too many default strings here. R.NET should not try to overcome variance in Unix setups.
            Dim shlibFilename = GetRLibraryFileName()
            Dim rexepath = ExecCommand("which", "R")
            ' /usr/bin/R,  or /usr/local/bin/R
            If String.IsNullOrEmpty(rexepath) Then
                Return "/usr/lib"
            End If
            Dim bindir = Path.GetDirectoryName(rexepath)
            '   /usr/local/bin
            ' Trying to emulate the start of the R shell script
            ' /usr/local/lib/R/lib/libR.so
            Dim libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib", "R", "lib")
            If File.Exists(Path.Combine(libdir, shlibFilename)) Then
                Return libdir
            End If
            libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib64", "R", "lib")
            If File.Exists(Path.Combine(libdir, shlibFilename)) Then
                Return libdir
            End If
            Return "/usr/lib"
        End Function

        Private Function FindRPathMacOS(rHome As String) As String
            ' TODO: is there a way to detect installations on MacOS
            Return "/Library/Frameworks/R.framework/Libraries"
        End Function

        Private Function FindRPathWindows(rHome As String) As String
            If Not String.IsNullOrEmpty(rHome) Then
                Return ConstructRPath(rHome)
            Else
                Return FindRPathFromRegistry()
            End If
        End Function

        Private Shared Sub SetenvPrepend(rPath As String, Optional envVarName As String = "PATH")
            ' this function results from a merge of PR https://rdotnet.codeplex.com/SourceControl/network/forks/skyguy94/PRFork/contribution/7684
            '  Not sure of the intent, and why a SetDllDirectory was used, where we moved away from. May need discussion with skyguy94
            '  relying on this too platform-specific way to specify the search path where
            '  Environment.SetEnvironmentVariable is multi-platform.

            Environment.SetEnvironmentVariable(envVarName, PrependToEnv(rPath, envVarName))
            '
            '            var platform = GetPlatform();
            '            if (platform == PlatformID.Win32NT)
            '            {
            '               var result = WindowsLibraryLoader.SetDllDirectory(rPath);
            '               var buffer = new StringBuilder(100);
            '               WindowsLibraryLoader.GetDllDirectory(100, buffer);
            '               Console.WriteLine("DLLPath:" + buffer.ToString());
            '            }
            '            

        End Sub

        Private Shared Function PrependToEnv(rPath As String, Optional envVarName As String = "PATH") As String
            Dim currentPathEnv = Environment.GetEnvironmentVariable(envVarName)
            Dim paths = currentPathEnv.Split({Path.PathSeparator}, StringSplitOptions.RemoveEmptyEntries)
            If paths(0) = rPath Then
                Return currentPathEnv
            End If
            Return rPath & Path.PathSeparator & currentPathEnv
        End Function

        ''' <summary>
        ''' Windows-only function; finds in the Windows registry the path to the most recently installed R binaries.
        ''' </summary>
        ''' <returns>The path, such as</returns>
        Public Function FindRPathFromRegistry(Optional logger As StringBuilder = Nothing) As String
            CheckPlatformWin32()
            Dim is64Bit As Boolean = Environment.Is64BitProcess
            Dim rCoreKey As IRegistryKey = GetRCoreRegistryKeyWin32(logger)
            Dim installPath = GetRInstallPathFromRCoreKegKey(rCoreKey, logger)
            Dim currentVersion = GetRVersionFromRegistry()
            Dim bin = Path.Combine(installPath, "bin")
            ' Up to 2.11.x, DLLs are installed in R_HOME\bin.
            ' From 2.12.0, DLLs are installed in the one level deeper directory.
            Return If(currentVersion < New Version(2, 12), bin, Path.Combine(bin, If(is64Bit, "x64", "i386")))
        End Function

        Private Function GetRInstallPathFromRCoreKegKey(rCoreKey As IRegistryKey, logger As StringBuilder) As String
            Dim installPath As String = Nothing
            Dim subKeyNames As String() = rCoreKey.GetSubKeyNames()
            Dim valueNames As String() = rCoreKey.GetValueNames()
            If valueNames.Length = 0 Then
                doLogSetEnvVarWarn("Did not find any value names under " & Convert.ToString(rCoreKey), logger)
                Return RecurseFirstSubkey(rCoreKey, logger)
            Else
                Const installPathKey As String = "InstallPath"
                If valueNames.Contains(installPathKey) Then
                    doLogSetEnvVarInfo("Found sub-key InstallPath under " & Convert.ToString(rCoreKey), logger)
                    installPath = DirectCast(rCoreKey.GetValue(installPathKey), String)
                Else
                    doLogSetEnvVarInfo("Did not find sub-key InstallPath under " & Convert.ToString(rCoreKey), logger)
                    If valueNames.Contains("Current Version") Then
                        doLogSetEnvVarInfo("Found sub-key Current Version under " & Convert.ToString(rCoreKey), logger)
                        Dim currentVersion As String = GetRCurrentVersionStringFromRegistry(rCoreKey)
                        If subKeyNames.Contains(currentVersion) Then
                            Dim rVersionCoreKey As IRegistryKey = rCoreKey.OpenSubKey(currentVersion)
                            Return GetRInstallPathFromRCoreKegKey(rVersionCoreKey, logger)
                        Else
                            doLogSetEnvVarWarn("Sub key " & currentVersion & " not found in " & Convert.ToString(rCoreKey), logger)
                        End If
                    Else
                        doLogSetEnvVarInfo("Did not find sub-key Current Version under " & Convert.ToString(rCoreKey), logger)
                        Return RecurseFirstSubkey(rCoreKey, logger)
                    End If
                End If
            End If
            doLogSetEnvVarInfo(String.Format("InstallPath value of key " & rCoreKey.ToString() & ": {0}", If(installPath Is Nothing, "null", installPath)), logger)
            Return installPath
        End Function

        Private Function RecurseFirstSubkey(rCoreKey As IRegistryKey, logger As StringBuilder) As String
            Dim subKeyNames As String() = rCoreKey.GetSubKeyNames()
            If subKeyNames.Length > 0 Then
                Dim versionNum = subKeyNames.First()
                Dim rVersionCoreKey = rCoreKey.OpenSubKey(versionNum)
                doLogSetEnvVarInfo("As a last resort, trying to recurse into " & Convert.ToString(rVersionCoreKey), logger)
                Return GetRInstallPathFromRCoreKegKey(rVersionCoreKey, logger)
            Else
                doLogSetEnvVarWarn("No sub-key found under " & Convert.ToString(rCoreKey), logger)
                Return Nothing
            End If
        End Function

        Private Shared Sub CheckPlatformWin32()
            If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                Throw New NotSupportedException("This method is supported only on the Win32NT platform")
            End If
        End Sub

        Private Function GetRCoreRegistryKeyWin32(logger As StringBuilder) As IRegistryKey
            CheckPlatformWin32()
            Dim rCore As IRegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\R-core")
            If rCore Is Nothing Then
                doLogSetEnvVarInfo("Local machine SOFTWARE\R-core not found - trying current user", logger)
                rCore = Registry.CurrentUser.OpenSubKey("SOFTWARE\R-core")
                If rCore Is Nothing Then
                    Throw New ApplicationException("Windows Registry key 'SOFTWARE\R-core' not found in HKEY_LOCAL_MACHINE nor HKEY_CURRENT_USER")
                End If
            End If
            doFoundWinRegKey(rCore, logger)
            Dim is64Bit As Boolean = Environment.Is64BitProcess
            Dim subKey = If(is64Bit, "R64", "R")
            Dim r = rCore.OpenSubKey(subKey)
            If r Is Nothing Then
                Throw New ApplicationException(String.Format("Windows Registry sub-key '{0}' of key '{1}' was not found", subKey, rCore.ToString()))
            End If
            doFoundWinRegKey(rCore, logger)
            Return r
        End Function

        ''' <summary>
        ''' Gets the default file name of the R library on the supported platforms.
        ''' </summary>
        ''' <returns>R dll file name</returns>
        Public Shared Function GetRLibraryFileName() As String
            Dim p = GetPlatform()
            Select Case p
                Case PlatformID.Win32NT
                    Return "R.dll"

                Case PlatformID.MacOSX
                    Return "libR.dylib"

                Case PlatformID.Unix
                    Return "libR.so"
                Case Else

                    Throw New PlatformNotSupportedException()
            End Select
        End Function

        ''' <summary>
        ''' Is the platform a unix like (Unix or MacOX)
        ''' </summary>
        Public Shared ReadOnly Property IsUnix() As Boolean
            Get
                Dim p = GetPlatform()
                Return p = PlatformID.MacOSX OrElse p = PlatformID.Unix
            End Get
        End Property
    End Class
End Namespace
