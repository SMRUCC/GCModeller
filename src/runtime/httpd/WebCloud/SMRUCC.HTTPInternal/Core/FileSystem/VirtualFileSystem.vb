#Region "Microsoft.VisualBasic::095c9582aa132bbb9f9350ebe693d15b, WebCloud\SMRUCC.HTTPInternal\Core\FileSystem\VirtualFileSystem.vb"

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

    '     Class VirtualFileSystem
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetFileBuffer, ToString
    ' 
    '         Sub: Add, (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
