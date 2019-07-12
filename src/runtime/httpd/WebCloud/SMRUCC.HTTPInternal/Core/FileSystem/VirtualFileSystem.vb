Imports System.IO
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.Platform.Plugins
Imports SMRUCC.WebCloud.HTTPInternal.Core.Cache
Imports fs = Microsoft.VisualBasic.FileIO.FileSystem

Namespace Core.Cache

    ''' <summary>
    ''' 缓存在内存中的虚拟文件系统
    ''' </summary>
    Public Class VirtualFileSystem : Implements IDisposable

#Region "两种数据组织的方式"
        ReadOnly files As New Dictionary(Of String, CachedFile)
        ''' <summary>
        ''' The root of this in-memory virtual filesystem
        ''' </summary>
        ReadOnly fileTree As FileNode
#End Region

        ''' <summary>
        ''' 文件更新
        ''' </summary>
        ReadOnly _cacheUpdate As UpdateThread
        ReadOnly wwwroot As DirectoryInfo

        ''' <summary>
        ''' 可以使用这个时间戳来获取更新后的文件内容
        ''' </summary>
        ReadOnly lastUpdate As Date

        Sub New(updateMode As Boolean, wwwroot As DirectoryInfo)
            If updateMode Then
                _cacheUpdate = New UpdateThread(1000 * 60 * 30, AddressOf Me.RunCacheUpdate)
                _cacheUpdate.Start()
            End If

            Me.wwwroot = wwwroot

            Call "Running in file system cache mode!".__DEBUG_ECHO
        End Sub

        Public Sub Add(path As String, file As CachedFile)

        End Sub

        Public Function GetFileBuffer(file As String) As Byte()
            If files.ContainsKey(file) Then
                Return files(file).bufs
            Else
                Return {}
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"memory://{wwwroot.FullName}"
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call _cacheUpdate.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace