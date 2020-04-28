#Region "Microsoft.VisualBasic::1a6bc1b5cb6762811593ef41144db203, PLAS.NET\SSystem\System\DataAcquisition.vb"

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

    '     Class DataSnapshot
    ' 
    ' 
    ' 
    '     Class MemoryCacheSnapshot
    ' 
    '         Sub: Cache, flush
    ' 
    '     Class SnapshotStream
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Cache, (+2 Overloads) Dispose
    ' 
    '     Class DataAcquisition
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: loadKernel, tagAs
    ' 
    '         Sub: Tick
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    Public MustInherit Class DataSnapshot

        Public MustOverride Sub Cache(data As DataSet)

    End Class

    Public Class MemoryCacheSnapshot : Inherits DataSnapshot

        Friend data As New List(Of DataSet)

        Public Overrides Sub Cache(data As DataSet)
            Me.data.Add(data)
        End Sub

        Public Sub flush(path As String)
            Call data.SaveTo(path, strict:=False)
        End Sub
    End Class

    Public Class SnapshotStream : Inherits DataSnapshot
        Implements IDisposable

        ReadOnly stream As WriteStream(Of DataSet)

        Sub New(fileOpen As String, symbols As String())
            stream = New WriteStream(Of DataSet)(fileOpen, strict:=False, metaKeys:=symbols, maps:=New Dictionary(Of String, String) From {{"ID", "#time"}})
        End Sub

        Public Overrides Sub Cache(data As DataSet)
            Call stream.Flush(data)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call stream.Flush()
                    Call stream.Dispose()
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

    ''' <summary>
    ''' Data service.(数据采集服务)
    ''' </summary>
    Public Class DataAcquisition

        ''' <summary>
        ''' 这个主要是调用外部接口的回调函数，这个回调函数会在一个内核循环之中采集完数据之后被触发调用
        ''' </summary>
        Dim __tickCallback As Action(Of DataSet)
        Dim kernel As Kernel

        Sub New(tick As Action(Of DataSet))
            __tickCallback = tick
        End Sub

        Public Function loadKernel(kernel As Kernel) As DataAcquisition
            Me.kernel = kernel
            Return Me
        End Function

        Public Sub Tick()
            Dim t As New DataSet With {
                .ID = kernel.RuntimeTicks * kernel.precision,
                .Properties = kernel.Vars _
                    .ToDictionary(AddressOf tagAs,
                                  Function(x)
                                      Return x.Value
                                  End Function)
            }

            ' 在一次内核循环之中才几万计算数据之后调用回调函数来触发外部事件
            Call __tickCallback(t)
        End Sub

        Private Shared Function tagAs(x As var) As String
            If Not String.IsNullOrEmpty(x.title) Then
                Return $"{x.Id}({x.title})"
            Else
                Return x.Id
            End If
        End Function
    End Class
End Namespace
