Imports System.Security.Permissions
Imports Microsoft.Win32.SafeHandles

Namespace NativeLibrary.DynamicInterop
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
    Friend NotInheritable Class SafeHandleUnmanagedDll
        Inherits SafeHandleZeroOrMinusOneIsInvalid
        Public Sub New(dllName As String)
            MyBase.New(True)
            Dim libraryLoader As IDynamicLibraryLoader = Nothing
            If PlatformUtility.IsUnix Then
                libraryLoader = New UnixLibraryLoader()
            ElseIf Environment.OSVersion.Platform = PlatformID.Win32NT Then
                libraryLoader = New WindowsLibraryLoader()
            Else
                Throw New NotSupportedException(PlatformUtility.GetPlatformNotSupportedMsg())
            End If
            Me.libraryLoader = libraryLoader
            handle = libraryLoader.LoadLibrary(dllName)
        End Sub

        Private ReadOnly libraryLoader As IDynamicLibraryLoader

        ''' <summary>
        ''' Frees the native library this objects represents
        ''' </summary>
        ''' <returns>The result of the call to FreeLibrary</returns>
        Protected Overrides Function ReleaseHandle() As Boolean
            Return FreeLibrary()
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

        Public Function GetFunctionAddress(lpProcName As String) As IntPtr
            Return libraryLoader.GetFunctionAddress(handle, lpProcName)
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

        Public Function GetLastError() As String
            Return libraryLoader.GetLastError()
        End Function
    End Class
End Namespace
