Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.DynamicInterop

Namespace NativeLibrary

    ''' <summary>
    ''' Collection of utility methods for operating systems.
    ''' </summary>
    Public Class NativeUtility

        ''' <summary> Gets or sets the registry.</summary>
        '''
        ''' <value> The registry.</value>
        Protected Property Registry As IRegistry

        ''' <summary> Constructor.</summary>
        '''
        ''' <param name="registry"> (Optional)
        '''                         The registry.</param>
        Public Sub New(ByVal Optional registry As IRegistry = Nothing)
            Me.Registry = (If(registry Is Nothing, New WindowsRegistry(), registry))
        End Sub

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
            Return PlatformUtility.GetPlatform()
        End Function

        ''' <summary>
        ''' Execute a command in a new process
        ''' </summary>
        ''' <param name="processName">Process name e.g. "uname"</param>
        ''' <param name="arguments">Arguments e.g. "-s"</param>
        ''' <returns>The output of the command to the standard output stream</returns>
        Public Shared Function ExecCommand(ByVal processName As String, ByVal arguments As String) As String
            Return PlatformUtility.ExecCommand(processName, arguments)
        End Function

        Private Shared logSetEnvVar As StringBuilder = New StringBuilder()

        ''' <summary>
        ''' Gets a log of the changes made to environment variables via the NativeUtility
        ''' </summary>
        Public Shared ReadOnly Property SetEnvironmentVariablesLog As String
            Get
                Return logSetEnvVar.ToString()
            End Get
        End Property

        ''' <summary>
        ''' Gets the path to the folder containing R.dll that this instance found when setting environment variables
        ''' </summary>
        Public Property RPath As String

        ''' <summary>
        ''' Gets the path to the R home directory that this instance found when setting environment variables
        ''' </summary>
        Public Property RHome As String

        Public Sub SetCachedEnvironmentVariables()
            If Equals(RPath, Nothing) OrElse Equals(RHome, Nothing) Then Throw New InvalidOperationException("SetCachedEnvironmentVariables requires R path and home directory to have been specified or detected")
            SetenvPrepend(RPath)
            Environment.SetEnvironmentVariable("R_HOME", RHome)
        End Sub

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
        Public Sub SetEnvironmentVariables(ByVal Optional rPath As String = Nothing, ByVal Optional rHome As String = Nothing)
            ' 
            '* Changing the behavior in Oct 2014, following the report of
            '* https://rdotnet.codeplex.com/workitem/140
            '* Use rHome, whether from the method parameter or from the environment variable,
            '* to deduce the path to the binaries, in preference to the registry key.


            logSetEnvVar.Clear()
            Dim platform = GetPlatform()
            If Not Equals(rPath, Nothing) Then CheckDirExists(rPath)
            If Not Equals(rHome, Nothing) Then CheckDirExists(rHome)
            FindRPaths(rPath, rHome, logSetEnvVar)
            If String.IsNullOrEmpty(rHome) Then Throw New NotSupportedException("R_HOME was not provided and a suitable path could not be found by R.NET")
            SetenvPrepend(rPath)
            ' It is highly recommended to use the 8.3 short path format on windows.
            ' See the manual page of R.home function in R. Solves at least the issue R.NET 97.
            If platform = PlatformID.Win32NT Then rHome = GetShortPath(rHome)
            If Not Directory.Exists(rHome) Then Throw New DirectoryNotFoundException(String.Format("Directory '{0}' does not exist - cannot set the environment variable R_HOME to that value", rHome))
            Environment.SetEnvironmentVariable("R_HOME", rHome)

            If platform = PlatformID.Unix Then
                ' Let's check that LD_LIBRARY_PATH is set if this is a custom installation of R.
                ' Normally in an R session from a custom build/install we get something typically like:
                ' > Sys.getenv('LD_LIBRARY_PATH')
                ' [1] "/usr/local/lib/R/lib:/usr/local/lib:/usr/lib/jvm/java-7-openjdk-amd64/jre/lib/amd64/server"
                ' The R script sets LD_LIBRARY_PATH before it starts the native executable under e.g. /usr/local/lib/R/bin/exec/R
                ' This would be useless to set LD_LIBRARY_PATH in the current function:
                ' it must be set as en env var BEFORE the process is started (see man page for dlopen)
                ' so all we can do is an intelligible error message for the user, explaining he needs to set the LD_LIBRARY_PATH env variable
                ' Let's delay the notification about a missing LD_LIBRARY_PATH till loading libR.so fails, if it does.
            End If

            Me.RPath = rPath
            Me.RHome = rHome
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
            Dim logger As StringBuilder = New StringBuilder()
            FindRPaths(rPath, rHome, logger)
            Return logger.ToString()
        End Function

        Private Sub FindRPaths(ByRef rPath As String, ByRef rHome As String, ByVal logSetEnvVar As StringBuilder)
            doLogSetEnvVarInfo(String.Format("caller provided rPath={0}, rHome={1}", If(Equals(rPath, Nothing), "null", rPath), If(Equals(rHome, Nothing), "null", rHome)), logSetEnvVar)

            If String.IsNullOrEmpty(rHome) Then
                rHome = GetRHomeEnvironmentVariable()
                doLogSetEnvVarInfo(String.Format("R.NET looked for preset R_HOME env. var. Found {0}", If(Equals(rHome, Nothing), "null", rHome)), logSetEnvVar)
            End If

            If String.IsNullOrEmpty(rHome) Then
                rHome = FindRHome(rPath:=Nothing, logger:=logSetEnvVar)
                doLogSetEnvVarInfo(String.Format("R.NET looked for platform-specific way (e.g. win registry). Found {0}", If(Equals(rHome, Nothing), "null", rHome)), logSetEnvVar)

                If Not String.IsNullOrEmpty(rHome) Then
                    If Equals(rPath, Nothing) Then
                        rPath = FindRPath(rHome)
                        doLogSetEnvVarInfo(String.Format("R.NET trying to find rPath based on rHome; Deduced {0}", If(Equals(rPath, Nothing), "null", rPath)), logSetEnvVar)
                    End If

                    If Equals(rPath, Nothing) Then
                        rPath = FindRPath()
                        doLogSetEnvVarInfo(String.Format("R.NET trying to find rPath, independently of rHome; Deduced {0}", If(Equals(rPath, Nothing), "null", rPath)), logSetEnvVar)
                    End If
                Else
                    rHome = FindRHome(rPath)
                    doLogSetEnvVarInfo(String.Format("R.NET trying to find rHome based on rPath; Deduced {0}", If(Equals(rHome, Nothing), "null", rHome)), logSetEnvVar)
                End If
            End If

            If String.IsNullOrEmpty(rHome) Then doLogSetEnvVar("Error", "R_HOME was not provided and a suitable path could not be found by R.NET", logSetEnvVar)
        End Sub

        Private Sub doLogSetEnvVar(ByVal level As String, ByVal msg As String, ByVal logSetEnvVar As StringBuilder)
            If logSetEnvVar IsNot Nothing Then
                logSetEnvVar.Append(level)
                logSetEnvVar.Append(": ")
                logSetEnvVar.AppendLine(msg)
            End If
        End Sub

        Private Sub doLogSetEnvVarWarn(ByVal msg As String, ByVal logger As StringBuilder)
            doLogSetEnvVar("Warn", msg, logger)
        End Sub

        Private Sub doLogSetEnvVarInfo(ByVal msg As String, ByVal logger As StringBuilder)
            doLogSetEnvVar("Info", msg, logger)
        End Sub

        Private Sub doFoundWinRegKey(ByVal rCore As IRegistryKey, ByVal logger As StringBuilder)
            doLogSetEnvVarInfo(String.Format("Found Windows registry key {0}", rCore.ToString()), logger)
        End Sub

        Private Shared Function GetShortPath(ByVal path As String) As String
            Dim shortPath = New StringBuilder(MaxPathLength)
            GetShortPathName(path, shortPath, MaxPathLength)
            Return shortPath.ToString()
        End Function

        Private Const MaxPathLength As Integer = 248 'MaxPath is 248. MaxFileName is 260.

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto)>
        Private Shared Function GetShortPathName(
        <MarshalAs(UnmanagedType.LPTStr)> ByVal path As String,
        <MarshalAs(UnmanagedType.LPTStr)> ByVal shortPath As StringBuilder, ByVal shortPathLength As Integer) As Integer
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
        Public Function FindRHome(ByVal Optional rPath As String = Nothing, ByVal Optional logger As StringBuilder = Nothing) As String
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
                Case PlatformID.MacOSX
                    rHome = "/Library/Frameworks/R.framework/Resources"
                Case PlatformID.Unix

                    If Not String.IsNullOrEmpty(rPath) Then
                        ' if rPath is e.g. /usr/local/lib/R/lib/ ,
                        rHome = Path.GetDirectoryName(rPath)
                    Else
                        rHome = "/usr/lib/R"
                    End If
                    ' if rPath is e.g. /usr/lib/ (symlink)  then default
                    If Not rHome.EndsWith("R") Then rHome = "/usr/lib/R"
                Case Else
                    Throw New NotSupportedException(platform.ToString())
            End Select

            Return rHome
        End Function

        Private Function GetRhomeWin32NT(ByVal logger As StringBuilder) As String
            Dim rCoreKey = GetRCoreRegistryKeyWin32(logger)
            Return GetRInstallPathFromRCoreKegKey(rCoreKey, logger)
        End Function

        Private Shared Sub CheckDirExists(ByVal rPath As String)
            If Not Directory.Exists(rPath) Then Throw New ArgumentException(String.Format("Specified directory not found: '{0}'", rPath))
        End Sub

        Private Function ConstructRPath(ByVal rHome As String) As String
            Dim shlibFilename = GetRLibraryFileName()
            Dim platform = GetPlatform()

            Select Case platform
                Case PlatformID.Win32NT
                    Dim rPath = Path.Combine(rHome, "bin")
                    Dim rVersion As Version = GetRVersionFromRegistry()

                    If rVersion.Major > 2 OrElse rVersion.Major = 2 AndAlso rVersion.Minor >= 12 Then
                        Dim bitness = If(Environment.Is64BitProcess, "x64", "i386")
                        rPath = Path.Combine(rPath, bitness)
                    End If

                    Return rPath
                Case Else
                    Throw New PlatformNotSupportedException()
            End Select
        End Function

        Private Function GetRCoreRegistryKey(ByVal logger As StringBuilder) As IRegistryKey
            If Environment.OSVersion.Platform <> PlatformID.Win32NT Then Return Nothing
            Return GetRCoreRegistryKeyWin32(logger)
        End Function

        ''' <summary>
        ''' Gets the R version from the Windows R Registry (if available)
        ''' </summary>
        ''' <returns>a System.Version object</returns>
        Public Function GetRVersionFromRegistry(ByVal Optional logger As StringBuilder = Nothing) As Version
            Dim rCoreKey = GetRCoreRegistryKey(logger)
            Dim version = GetRCurrentVersionStringFromRegistry(rCoreKey)

            If String.IsNullOrEmpty(version) Then
                Dim subKeyNames As String() = rCoreKey.GetSubKeyNames()
                If subKeyNames.Length > 0 Then version = subKeyNames(0)
            End If
            ' Version should normally be "3.2.4", but some releases had
            ' "3.2.4 Revised" in which case constructor for Version fails.
            ' The regex first extracts the numerical part of the string.
            Dim reg = New Regex("([0-9]*\.)*[0-9]*")
            Return New Version(reg.Match(version).Value)
        End Function

        Private Shared Function GetRCurrentVersionStringFromRegistry(ByVal rCoreKey As IRegistryKey) As String
            Return TryCast(rCoreKey.GetValue("Current Version"), String)
        End Function

        ''' <summary>
        ''' Attempt to find a suitable path to the R shared library. This is used by R.NET by default; users may want to use it to diagnose problematic behaviors.
        ''' </summary>
        ''' <returns>The path to the directory where the R shared library is expected to be</returns>
        Public Function FindRPath(ByVal Optional rHome As String = Nothing) As String
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

        Private Function FindRPathUnix(ByVal rHome As String) As String
            ' TODO: too many default strings here. R.NET should not try to overcome variance in Unix setups.
            Dim shlibFilename = GetRLibraryFileName()
            Dim rexepath = ExecCommand("which", "R") ' /usr/bin/R,  or /usr/local/bin/R
            If String.IsNullOrEmpty(rexepath) Then Return "/usr/lib"
            Dim bindir = Path.GetDirectoryName(rexepath) '   /usr/local/bin
            ' Trying to emulate the start of the R shell script
            ' /usr/local/lib/R/lib/libR.so
            Dim libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib", "R", "lib")
            If File.Exists(Path.Combine(libdir, shlibFilename)) Then Return libdir
            libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib64", "R", "lib")
            If File.Exists(Path.Combine(libdir, shlibFilename)) Then Return libdir
            Return "/usr/lib"
        End Function

        Private Function FindRPathMacOS(ByVal rHome As String) As String
            ' TODO: is there a way to detect installations on MacOS
            Return "/Library/Frameworks/R.framework/Libraries"
        End Function

        Private Function FindRPathWindows(ByVal rHome As String) As String
            If Not String.IsNullOrEmpty(rHome) Then
                Return ConstructRPath(rHome)
            Else
                Return FindRPathFromRegistry()
            End If
        End Function

        Private Shared Sub SetenvPrepend(ByVal rPath As String, ByVal Optional envVarName As String = "PATH")
            ' this function results from a merge of PR https://rdotnet.codeplex.com/SourceControl/network/forks/skyguy94/PRFork/contribution/7684
            '  Not sure of the intent, and why a SetDllDirectory was used, where we moved away from. May need discussion with skyguy94
            '  relying on this too platform-specific way to specify the search path where
            '  Environment.SetEnvironmentVariable is multi-platform.

            Environment.SetEnvironmentVariable(envVarName, PrependToEnv(rPath, envVarName))
            ' 
            'var platform = GetPlatform();
            'if (platform == PlatformID.Win32NT)
            '{
            '   var result = WindowsLibraryLoader.SetDllDirectory(rPath);
            '   var buffer = new StringBuilder(100);
            '   WindowsLibraryLoader.GetDllDirectory(100, buffer);
            '   Console.WriteLine("DLLPath:" + buffer.ToString());
            '}

        End Sub

        Private Shared Function PrependToEnv(ByVal rPath As String, ByVal Optional envVarName As String = "PATH") As String
            Dim currentPathEnv = Environment.GetEnvironmentVariable(envVarName)
            Dim paths = currentPathEnv.Split({Path.PathSeparator}, StringSplitOptions.RemoveEmptyEntries)
            If Equals(paths(0), rPath) Then Return currentPathEnv
            Return rPath & Path.PathSeparator & currentPathEnv
        End Function

        ''' <summary>
        ''' Windows-only function; finds in the Windows registry the path to the most recently installed R binaries.
        ''' </summary>
        ''' <returns>The path, such as</returns>
        Public Function FindRPathFromRegistry(ByVal Optional logger As StringBuilder = Nothing) As String
            CheckPlatformWin32()
            Dim is64Bit = Environment.Is64BitProcess
            Dim rCoreKey = GetRCoreRegistryKeyWin32(logger)
            Dim installPath = GetRInstallPathFromRCoreKegKey(rCoreKey, logger)
            Dim currentVersion = GetRVersionFromRegistry()
            Dim bin = Path.Combine(installPath, "bin")
            ' Up to 2.11.x, DLLs are installed in R_HOME\bin.
            ' From 2.12.0, DLLs are installed in the one level deeper directory.
            Return If(currentVersion < New Version(2, 12), bin, Path.Combine(bin, If(is64Bit, "x64", "i386")))
        End Function

        Private Function GetRInstallPathFromRCoreKegKey(ByVal rCoreKey As IRegistryKey, ByVal logger As StringBuilder) As String
            Dim installPath As String = Nothing
            Dim subKeyNames As String() = rCoreKey.GetSubKeyNames()
            Dim valueNames As String() = rCoreKey.GetValueNames()

            If valueNames.Length = 0 Then
                Me.doLogSetEnvVarWarn("Did not find any value names under " & rCoreKey.ToString, logger)
                Return RecurseFirstSubkey(rCoreKey, logger)
            Else
                Const installPathKey = "InstallPath"

                If valueNames.Contains(installPathKey) Then
                    Me.doLogSetEnvVarInfo("Found sub-key InstallPath under " & rCoreKey.ToString, logger)
                    installPath = CStr(rCoreKey.GetValue(installPathKey))
                Else
                    Me.doLogSetEnvVarInfo("Did not find sub-key InstallPath under " & rCoreKey.ToString, logger)

                    If valueNames.Contains("Current Version") Then
                        Me.doLogSetEnvVarInfo("Found sub-key Current Version under " & rCoreKey.ToString, logger)
                        Dim currentVersion = GetRCurrentVersionStringFromRegistry(rCoreKey)

                        If subKeyNames.Contains(currentVersion) Then
                            Dim rVersionCoreKey = rCoreKey.OpenSubKey(currentVersion)
                            Return GetRInstallPathFromRCoreKegKey(rVersionCoreKey, logger)
                        Else
                            Me.doLogSetEnvVarWarn("Sub key " & currentVersion & " not found in " & rCoreKey.ToString, logger)
                        End If
                    Else
                        Me.doLogSetEnvVarInfo("Did not find sub-key Current Version under " & rCoreKey.ToString, logger)
                        Return RecurseFirstSubkey(rCoreKey, logger)
                    End If
                End If
            End If

            doLogSetEnvVarInfo(String.Format("InstallPath value of key " & rCoreKey.ToString() & ": {0}", If(Equals(installPath, Nothing), "null", installPath)), logger)
            Return installPath
        End Function

        Private Function RecurseFirstSubkey(ByVal rCoreKey As IRegistryKey, ByVal logger As StringBuilder) As String
            Dim subKeyNames As String() = rCoreKey.GetSubKeyNames()

            If subKeyNames.Length > 0 Then
                Array.Sort(subKeyNames)
                Dim versionNum = subKeyNames.Last() ' gets the latest version of R installed and registered.
                'versionNum = subKeyNames.First(); // TEMP...
                Dim rVersionCoreKey = rCoreKey.OpenSubKey(versionNum)
                Me.doLogSetEnvVarInfo("As a last resort, trying to recurse into " & rVersionCoreKey.ToString, logger)
                Return GetRInstallPathFromRCoreKegKey(rVersionCoreKey, logger)
            Else
                Me.doLogSetEnvVarWarn("No sub-key found under " & rCoreKey.ToString, logger)
                Return Nothing
            End If
        End Function

        Private Shared Sub CheckPlatformWin32()
            If Environment.OSVersion.Platform <> PlatformID.Win32NT Then Throw New NotSupportedException("This method is supported only on the Win32NT platform")
        End Sub

        Private Function GetRCoreRegistryKeyWin32(ByVal logger As StringBuilder) As IRegistryKey
            CheckPlatformWin32()
            Dim rCore = Registry.LocalMachine.OpenSubKey("SOFTWARE\R-core")

            If rCore Is Nothing OrElse rCore.GetRealKey() Is Nothing Then
                doLogSetEnvVarInfo("Local machine SOFTWARE\R-core not found - trying current user", logger)
                rCore = Registry.CurrentUser.OpenSubKey("SOFTWARE\R-core")
                If rCore Is Nothing OrElse rCore.GetRealKey() Is Nothing Then Throw New ApplicationException("Windows Registry key 'SOFTWARE\R-core' not found in HKEY_LOCAL_MACHINE nor HKEY_CURRENT_USER")
            End If

            doFoundWinRegKey(rCore, logger)
            Dim is64Bit = Environment.Is64BitProcess
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
        Public Shared ReadOnly Property IsUnix As Boolean
            Get
                Dim p = GetPlatform()
                Return p = PlatformID.MacOSX OrElse p = PlatformID.Unix
            End Get
        End Property
    End Class
End Namespace
