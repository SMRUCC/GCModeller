#Region "Microsoft.VisualBasic::beefdaddc387208c83432453db637e74, GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\KernelDriver\DataAdapter\DataAdapter.vb"

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


    ' Code Statistics:

    '   Total Lines: 74
    '    Code Lines: 48
    ' Comment Lines: 18
    '   Blank Lines: 8
    '     File Size: 3.36 KB


    ' Class DataAdapter
    ' 
    '     Function: __filterData, FetchData, ToString
    ' 
    '     Sub: Clear, DataAcquiring, SetFiltedHandles
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic

''' <summary>
''' 数据采集器
''' </summary>
''' <typeparam name="T"></typeparam>
''' <typeparam name="TDataSource"></typeparam>
''' <remarks></remarks>
Public Class DataAdapter(Of T, TDataSource As DataSourceHandler(Of T))

    ''' <summary>
    ''' 本列表之中的所有数据都不会被记录下来
    ''' </summary>
    ''' <remarks></remarks>
    Protected _filtedHandles As Long() = New Long() {}
    Protected ReadOnly _innerBuffer As List(Of TDataSource) = New List(Of TDataSource)

    Public Function FetchData(objectHandlers As Generic.IEnumerable(Of DataStorage.FileModel.ObjectHandle)) As DataStorage.FileModel.DataSerials(Of T)()
        Dim TimeSerials = (From data0Expr As TDataSource In _innerBuffer
                           Select data0Expr
                           Group data0Expr By data0Expr.Handle Into Group
                           Order By Handle Ascending).ToArray
        Dim [handles] As Long() = (From data0Expr In TimeSerials Select data0Expr.Handle).ToArray
        objectHandlers = (From hwnd As Long In [handles]
                          Let arraySel = (From objHwnd In objectHandlers Where objHwnd.Handle = hwnd Select objHwnd).FirstOrDefault
                          Select arraySel).ToArray
        Dim LQuery = (From objHwnd As DataStorage.FileModel.ObjectHandle
                      In objectHandlers.AsParallel
                      Let ChunkBuffer = (From data0Expr In TimeSerials Where data0Expr.Handle = objHwnd.Handle Select data0Expr).First
                      Let sampleSource As TDataSource() = (From DsGroup As TDataSource In ChunkBuffer.Group Select DsGroup Order By DsGroup.TimeStamp Ascending).ToArray
                      Let SampleValue As T() = (From data0Expr As TDataSource In sampleSource Select data0Expr.Value).ToArray
                      Let row = New DataStorage.FileModel.DataSerials(Of T) With {
                          .Handle = objHwnd.Handle,
                          .UniqueId = objHwnd.ID,
                          .Samples = SampleValue
                      }
                      Select row).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 清除数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Clear()
        Call _innerBuffer.Clear()
    End Sub

    ''' <summary>
    ''' 空值会自动清除过滤器的句柄列表
    ''' </summary>
    ''' <param name="lstHwnd"></param>
    Public Sub SetFiltedHandles(lstHwnd As Long())
        If lstHwnd Is Nothing Then
            lstHwnd = New Int64() {}
        End If
        _filtedHandles = lstHwnd
    End Sub

    Public Sub DataAcquiring(chunkBuffer As Generic.IEnumerable(Of TDataSource))
        Call _innerBuffer.AddRange(__filterData(chunkBuffer, Me._filtedHandles))
    End Sub

    Private Shared Function __filterData(chunkBuffer As Generic.IEnumerable(Of TDataSource), filters As Long()) As TDataSource()
        Return (From data0Expr As TDataSource
                In chunkBuffer.AsParallel
                Where Array.IndexOf(filters, data0Expr.Handle) = -1
                Select data0Expr).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return "CHUNK_SIZE:= " & _innerBuffer.Count
    End Function
End Class
