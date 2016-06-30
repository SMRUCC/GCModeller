Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

Namespace NativeLibrary

    ''' <summary>
    ''' A proxy for unmanaged dynamic link library (DLL).
    ''' </summary>
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)> _
    Public Class UnmanagedDll
        Inherits SafeHandle
        ''' <summary>
        ''' Gets whether the current handle is equal to the invalid handle
        ''' </summary>
        Public Overrides ReadOnly Property IsInvalid() As Boolean
            Get
                Return handle = IntPtr.Zero
            End Get
        End Property

        Private libraryLoader As IDynamicLibraryLoader

        ''' <summary>
        ''' Creates a proxy for the specified dll.
        ''' </summary>
        ''' <param name="dllName">The DLL's name.</param>
        Public Sub New(dllName As String)
            MyBase.New(IntPtr.Zero, True)
            If dllName Is Nothing Then
                Throw New ArgumentNullException("dllName", "The name of the library to load is a null reference")
            End If
            If dllName = String.Empty Then
                Throw New ArgumentException("The name of the library to load is an empty string", "dllName")
            End If
            If IsUnix Then
                libraryLoader = New UnixLibraryLoader()
            Else
                libraryLoader = New WindowsLibraryLoader()
            End If

            Dim handle As IntPtr = libraryLoader.LoadLibrary(dllName)
            If handle = IntPtr.Zero Then
                ReportLoadLibError(dllName)
            End If
            SetHandle(handle)
            Me.DllFilename = dllName
        End Sub

        ''' <summary>
        ''' Gets the Dll file name used for this native Dll wrapper.
        ''' </summary>
        Public Property DllFilename() As String
            Get
                Return m_DllFilename
            End Get
            Private Set(value As String)
                m_DllFilename = Value
            End Set
        End Property
        Private m_DllFilename As String


        Private Sub ReportLoadLibError(dllName As String)
            Dim dllFullName As String = dllName
            If File.Exists(dllFullName) Then
                ThrowFailedLibraryLoad(dllFullName)
            Else
                ' This below assumes that the PATH environment variable is what is relied on
                ' TODO: check whether there is more to it: http://msdn.microsoft.com/en-us/library/ms682586.aspx

                ' Also some pointers to relevant information if we want to check whether the attempt to load 
                ' was made on a 32 or 64 bit library
                ' For Windows:
                ' http://stackoverflow.com/questions/1345632/determine-if-an-executable-or-library-is-32-or-64-bits-on-windows
                ' http://www.neowin.net/forum/topic/732648-check-if-exe-is-x64/?p=590544108#entry590544108
                ' Linux, and perhaps MacOS; the 'file' command seems the way to go.
                ' http://stackoverflow.com/questions/5665228/in-linux-determine-if-a-a-library-archive-32-bit-or-64-bit

                dllFullName = FindFullPath(dllName, throwIfNotFound:=True)
                ThrowFailedLibraryLoad(dllFullName)
            End If
        End Sub

        Private Shared Function FindFullPath(dllName As String, Optional throwIfNotFound As Boolean = False) As String
            Dim dllFullName As String
            If File.Exists(dllName) Then
                dllFullName = Path.GetFullPath(dllName)
                If File.Exists(dllFullName) Then
                    Return dllFullName
                End If
            End If
            Dim searchPaths As String() = (If(Environment.GetEnvironmentVariable("PATH"), "")).Split(Path.PathSeparator)
            dllFullName = searchPaths.[Select](Function(directory) Path.Combine(directory, dllName)).FirstOrDefault(AddressOf File.Exists)
            If throwIfNotFound Then
                If String.IsNullOrEmpty(dllFullName) OrElse Not File.Exists(dllFullName) Then
                    Throw New DllNotFoundException(String.Format("Could not find the library named {0} in the search paths", dllName))
                End If
            End If
            Return dllFullName
        End Function

        Private Function createLdLibPathMsg() As String
            If Not NativeUtility.IsUnix Then
                Return Nothing
            End If
            Dim sampleldLibPaths = "/usr/local/lib/R/lib:/usr/local/lib:/usr/lib/jvm/java-7-openjdk-amd64/jre/lib/amd64/server"
            Dim ldLibPathEnv = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH")
            Dim msg As String = Environment.NewLine & Environment.NewLine
            If String.IsNullOrEmpty(ldLibPathEnv) Then
                msg = msg & "The environment variable LD_LIBRARY_PATH is not set."
            Else
                msg = msg & String.Format("The environment variable LD_LIBRARY_PATH is set to {0}.", ldLibPathEnv)
            End If

            msg = msg & String.Format(" For some Unix-like operating systems you may need to set it BEFORE launching the application. For instance to {0}.", sampleldLibPaths)
            msg = msg & " You can get the value as set by the R console application for your system, with the statement Sys.getenv('LD_LIBRARY_PATH'). For instance from your shell prompt:"
            msg = msg & Environment.NewLine
            msg = msg & "Rscript -e ""Sys.getenv('LD_LIBRARY_PATH')"""
            msg = msg & Environment.NewLine
            msg = msg & "export LD_LIBRARY_PATH=/usr/the/paths/you/just/got/from/Rscript"
            msg = msg & Environment.NewLine & Environment.NewLine

            Return msg
        End Function

        Private Sub ThrowFailedLibraryLoad(dllFullName As String)
            Dim strMsg = String.Format("This {0}-bit process failed to load the library {1}", (If(Environment.Is64BitProcess, "64", "32")), dllFullName)
            Dim nativeError = libraryLoader.GetLastError()
            If Not String.IsNullOrEmpty(nativeError) Then
                strMsg = strMsg & String.Format(". Native error message is '{0}'", nativeError)
            End If
            Dim ldLibPathMsg = createLdLibPathMsg()
            If Not String.IsNullOrEmpty(ldLibPathMsg) Then
                strMsg = strMsg & String.Format(". {0}", ldLibPathMsg)
            End If
            Throw New Exception(strMsg)
        End Sub

        Private delegateFunctionPointers As New Dictionary(Of String, Object)()

        ''' <summary>
        ''' Creates the delegate function for the specified function defined in the DLL.
        ''' </summary>
        ''' <typeparam name="TDelegate">The type of delegate. The name of the native function is assumed to be the same as the delegate type name.</typeparam>
        ''' <returns>The delegate.</returns>
        Public Function GetFunction(Of TDelegate As Class)() As TDelegate
            Return GetFunction(Of TDelegate)(GetType(TDelegate).Name)
        End Function

        ''' <summary>
        ''' Creates the delegate function for the specified function defined in the DLL.
        ''' </summary>
        ''' <typeparam name="TDelegate">The type of delegate.</typeparam>
        ''' <param name="entryPoint">The name of the function exported by the DLL</param>
        ''' <returns>The delegate.</returns>
        Public Function GetFunction(Of TDelegate As Class)(entryPoint As String) As TDelegate
            If String.IsNullOrEmpty(entryPoint) Then
                Throw New ArgumentNullException("entryPoint", "Native function name cannot be null or empty")
            End If
            Dim delegateType As Type = GetType(TDelegate)
            If delegateFunctionPointers.ContainsKey(entryPoint) Then
                Return DirectCast(delegateFunctionPointers(entryPoint), TDelegate)
            End If
            If Not delegateType.IsSubclassOf(GetType([Delegate])) Then
                Throw New InvalidCastException()
            End If
            Dim [function] As IntPtr = GetFunctionAddress(entryPoint)
            If [function] = IntPtr.Zero Then
                throwEntryPointNotFound(entryPoint)
            End If
            Dim dFunc = TryCast(Marshal.GetDelegateForFunctionPointer([function], delegateType), TDelegate)
            delegateFunctionPointers.Add(entryPoint, dFunc)
            Return dFunc
        End Function

        Private Sub throwEntryPointNotFound(entryPoint As String)
            Throw New EntryPointNotFoundException(String.Format("Function {0} not found in native library {1}", entryPoint, Me.DllFilename))
        End Sub

        Private ReadOnly Property IsUnix() As Boolean
            Get
                Return NativeUtility.IsUnix
            End Get
        End Property

        Private Function GetFunctionAddress(lpProcName As String) As IntPtr
            Return libraryLoader.GetFunctionAddress(handle, lpProcName)
        End Function

        Private Function FreeLibrary() As Boolean
            Dim freed As Boolean = False
            If libraryLoader Is Nothing Then
                If Not Me.IsInvalid Then
                    Try
                        Throw New ApplicationException("Warning: unexpected condition of library loader and native handle - some native resources may not be properly disposed of")
                    Finally
                        freed = False
                    End Try
                Else
                    freed = True
                End If
                Return freed
            Else
                Return libraryLoader.FreeLibrary(handle)
            End If
        End Function

        ''' <summary>
        ''' Gets the handle of the specified entry.
        ''' </summary>
        ''' <param name="entryPoint">The name of function.</param>
        ''' <returns>The handle.</returns>
        Public Overloads Function DangerousGetHandle(entryPoint As String) As IntPtr
            If entryPoint Is Nothing Then
                Throw New ArgumentNullException("entryPoint")
            End If
            Return GetFunctionAddress(entryPoint)
        End Function

        ''' <summary>
        ''' Frees the native library this objects represents
        ''' </summary>
        ''' <returns>The result of the call to FreeLibrary</returns>
        Protected Overrides Function ReleaseHandle() As Boolean
            Return FreeLibrary()
        End Function

        ''' <summary>
        ''' Frees the native library this objects represents
        ''' </summary>
        ''' <param name="disposing"></param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If FreeLibrary() Then
                SetHandleAsInvalid()
            End If
            MyBase.Dispose(disposing)
        End Sub

    End Class
End Namespace