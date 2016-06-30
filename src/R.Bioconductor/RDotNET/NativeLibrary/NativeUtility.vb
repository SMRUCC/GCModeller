Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports Microsoft.Win32

Namespace NativeLibrary

    ''' <summary>
    ''' Collection of utility methods for operating systems.
    ''' </summary>
    Public NotInheritable Class NativeUtility
        Private Sub New()
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
            If Not curPlatform.HasValue Then
                Dim platform As System.PlatformID = Environment.OSVersion.Platform
                If platform <> PlatformID.Unix Then
                    curPlatform = platform
                Else
                    Try
                        Dim kernelName As String = ExecCommand("uname", "-s")
                        curPlatform = (If(kernelName = "Darwin", PlatformID.MacOSX, platform))
                    Catch generatedExceptionName As Win32Exception
                        ' probably no PATH to uname.
                        curPlatform = platform
                    End Try
                End If
            End If
            Return curPlatform.Value
        End Function

        Private Shared curPlatform As System.Nullable(Of PlatformID) = Nothing

        ''' <summary>
        ''' Execute a command in a new process
        ''' </summary>
        ''' <param name="processName">Process name e.g. "uname"</param>
        ''' <param name="arguments">Arguments e.g. "-s"</param>
        ''' <returns>The output of the command to the standard output stream</returns>
        Public Shared Function ExecCommand(processName As String, arguments As String) As String
            Using uname As Process = New Process()
                uname.StartInfo.FileName = processName
                uname.StartInfo.Arguments = arguments
                uname.StartInfo.RedirectStandardOutput = True
                uname.StartInfo.UseShellExecute = False
                uname.StartInfo.CreateNoWindow = True
                uname.Start()
                Dim kernelName As String = uname.StandardOutput.ReadLine()
                uname.WaitForExit()
                Return kernelName
            End Using
        End Function

        ''' <summary>
        ''' Sets the PATH and R_HOME environment variables if needed.
        ''' </summary>
        ''' <param name="rPath">The path of the directory containing the R native library. 
        ''' If null (default), this function tries to locate the path via the Windows registry, or commonly used locations on MacOS and Linux</param>
        ''' <param name="rHome">The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set, 
        ''' the function uses platform specific sensible default behaviors.</param>
        ''' <remarks>
        ''' This function has been designed to limit the tedium for users, while allowing custom settings for unusual installations.
        ''' </remarks>
        Public Shared Sub SetEnvironmentVariables(Optional rPath As String = Nothing, Optional rHome As String = Nothing)
            Dim platform As PlatformID = GetPlatform()
            If rPath IsNot Nothing Then
                CheckDirExists(rPath)
            End If
            If rHome IsNot Nothing Then
                CheckDirExists(rHome)
            End If

            If rPath Is Nothing Then
                rPath = FindRPath()
            End If
            SetenvPrependToPath(rPath)

            If String.IsNullOrEmpty(rHome) Then
                rHome = GetRHomeEnvironmentVariable()
            End If
            If String.IsNullOrEmpty(rHome) Then
                ' R_HOME is neither specified by the user nor as an environmental variable. Rely on default locations specific to platforms
                rHome = FindRHome(rPath)
            End If
            If String.IsNullOrEmpty(rHome) Then
                Throw New NotSupportedException("R_HOME was not provided and could not be found by R.NET")
            Else
                ' It is highly recommended to use the 8.3 short path format on windows. 
                ' See the manual page of R.home function in R. Solves at least the issue R.NET 97.
                If platform = PlatformID.Win32NT Then
                    rHome = WindowsLibraryLoader.GetShortPath(rHome)
                End If
                If Not Directory.Exists(rHome) Then
                    Throw New DirectoryNotFoundException("Directory for R_HOME does not exist")
                End If
                Environment.SetEnvironmentVariable("R_HOME", rHome)
            End If
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
        ''' <returns>The path that R.NET found suitable as a candidate for the R_HOME environment</returns>
        Public Shared Function FindRHome(Optional rPath As String = Nothing) As String
            Dim platform As PlatformID = GetPlatform()
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
                    rHome = GetRhomeWin32NT()
                    Exit Select
                Case PlatformID.MacOSX
                    rHome = "/Library/Frameworks/R.framework/Resources"
                    Exit Select
                Case PlatformID.Unix
                    ' if rPath is e.g. /usr/local/lib/R/lib/ , 
                    rHome = Path.GetDirectoryName(rPath)
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

        Private Shared Function GetRhomeWin32NT() As String
            Dim rCoreKey As RegistryKey = GetRCoreRegistryKeyWin32()
            Return GetRInstallPathFromRCoreKegKey(rCoreKey)
        End Function

        Private Shared Sub CheckDirExists(rPath As String)
            If Not Directory.Exists(rPath) Then
                Throw New ArgumentException(String.Format("Specified directory not found: '{0}'", rPath))
            End If
        End Sub

        ''' <summary>
        ''' Attempt to find a suitable path to the R shared library. This is used by R.NET by default; users may want to use it to diagnose problematic behaviors.
        ''' </summary>
        ''' <returns>The path to the directory where the R shared library is expected to be</returns>
        Public Shared Function FindRPath() As String
            Dim shlibFilename As String = GetRDllFileName()
            Dim platform As PlatformID = GetPlatform()
            Select Case platform
                Case PlatformID.Win32NT
                    Return FindRPathFromRegistry()
                Case PlatformID.MacOSX
                    ' TODO: is there a way to detect installations on MacOS
                    Return "/Library/Frameworks/R.framework/Libraries"
                Case PlatformID.Unix
                    Dim rexepath As String = ExecCommand("which", "R")
                    ' /usr/bin/R,  or /usr/local/bin/R
                    If Not String.IsNullOrEmpty(rexepath) Then
                        Dim bindir As String = Path.GetDirectoryName(rexepath)
                        '   /usr/local/bin
                        ' Trying to emulate the start of the R shell script
                        ' /usr/local/lib/R/lib/libR.so
                        Dim libdir As String = Path.Combine(Path.GetDirectoryName(bindir), "lib", "R", "lib")
                        If File.Exists(Path.Combine(libdir, shlibFilename)) Then
                            Return libdir
                        End If
                        libdir = Path.Combine(Path.GetDirectoryName(bindir), "lib64", "R", "lib")
                        If File.Exists(Path.Combine(libdir, shlibFilename)) Then
                            Return libdir
                        End If
                    End If
                    Return "/usr/lib"
                Case Else
                    Throw New NotSupportedException(platform.ToString())
            End Select
        End Function

        Private Shared Sub SetenvPrependToPath(rPath As String, Optional envVarName As String = "PATH")
            Environment.SetEnvironmentVariable(envVarName, PrependToPath(rPath, envVarName))
        End Sub

        Private Shared Function PrependToPath(rPath As String, Optional envVarName As String = "PATH") As String
            Dim currentPathEnv As String = Environment.GetEnvironmentVariable(envVarName)
            Dim paths As String() = currentPathEnv.Split(New String() {Path.PathSeparator}, StringSplitOptions.RemoveEmptyEntries)
            If paths(0) = rPath Then
                Return currentPathEnv
            End If
            Return rPath & Path.PathSeparator & currentPathEnv
        End Function

        ''' <summary>
        ''' Windows-only function; finds in the Windows registry the path to the most recently installed R binaries.
        ''' </summary>
        ''' <returns>The path, such as</returns>
        Public Shared Function FindRPathFromRegistry() As String
            CheckPlatformWin32()
            Dim is64Bit As Boolean = Environment.Is64BitProcess
            Dim rCoreKey As RegistryKey = GetRCoreRegistryKeyWin32()
            Dim installPath As String = GetRInstallPathFromRCoreKegKey(rCoreKey)
            Dim currentVersion As Version = New Version(DirectCast(rCoreKey.GetValue("Current Version"), String))
            Dim bin = Path.Combine(installPath, "bin")
            ' Up to 2.11.x, DLLs are installed in R_HOME\bin.
            ' From 2.12.0, DLLs are installed in the one level deeper directory.
            Return If(currentVersion < New Version(2, 12), bin, Path.Combine(bin, If(is64Bit, "x64", "i386")))
        End Function

        Private Shared Function GetRInstallPathFromRCoreKegKey(rCoreKey As RegistryKey) As String
            Dim installPath = DirectCast(rCoreKey.GetValue("InstallPath"), String)
            Return installPath
        End Function

        Private Shared Sub CheckPlatformWin32()
            If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
                Throw New NotSupportedException("This method is supported only on the Win32NT platform")
            End If
        End Sub

        Private Shared Function GetRCoreRegistryKeyWin32() As RegistryKey
            CheckPlatformWin32()
            Dim rCore = Registry.LocalMachine.OpenSubKey("SOFTWARE\R-core")
            If rCore Is Nothing Then
                rCore = Registry.CurrentUser.OpenSubKey("SOFTWARE\R-core")
                If rCore Is Nothing Then
                    Throw New ApplicationException("Windows Registry key 'SOFTWARE\R-core' not found in HKEY_LOCAL_MACHINE nor HKEY_CURRENT_USER")
                End If
            End If
            Dim is64Bit As Boolean = Environment.Is64BitProcess
            Dim subKey = If(is64Bit, "R64", "R")
            Dim r = rCore.OpenSubKey(subKey)
            If r Is Nothing Then
                Throw New ApplicationException(String.Format("Windows Registry sub-key '{0}' of key '{1}' was not found", subKey, rCore.ToString()))
            End If
            Return r
        End Function

        ''' <summary>
        ''' Gets the default file name of the R library on the supported platforms.
        ''' </summary>
        ''' <returns>R dll file name</returns>
        Public Shared Function GetRDllFileName() As String
            Dim p = GetPlatform()
            Select Case p
                Case PlatformID.Win32NT
                    Return "R.dll"

                Case PlatformID.MacOSX
                    Return "libR.dylib"

                Case PlatformID.Unix
                    Return "libR.so"
                Case Else

                    Throw New NotSupportedException("Platform is not supported: " & p.ToString())
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