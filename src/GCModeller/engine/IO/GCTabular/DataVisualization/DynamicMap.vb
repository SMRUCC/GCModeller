#Region "Microsoft.VisualBasic::97efdee7db553694a33c7164f4dee13d, engine\IO\GCTabular\DataVisualization\DynamicMap.vb"

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

    '     Class DynamicMap
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetItemValue
    ' 
    '         Sub: AddCellSystemMapHandlers, AddSystemNodesHandles, TrimMap
    '         Class FileHandle
    ' 
    '             Function: CreateObject, GetValue, ReadBuffer, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.IO

Namespace DataVisualization

    Public Class DynamicMap

        Protected Friend Class FileHandle
            Dim _ChunkBuffer As KeyValuePairObject(Of String(), String())()
            Dim _FilePath As String

            Public Overrides Function ToString() As String
                Return _FilePath
            End Function

            Private Shared Function ReadBuffer(Path As String) As KeyValuePairObject(Of String(), String())
                Dim ChunkBuffer As String() = IO.File.ReadAllLines(Path)
                Dim Heads As String() = RowObject.TryParse(ChunkBuffer.First).ToArray
                Return New KeyValuePairObject(Of String(), String()) With {
                    .Key = Heads,
                    .Value = ChunkBuffer
                }
            End Function

            Public Shared Function CreateObject([Handles] As String()) As FileHandle
                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                For Each Path As String In [Handles]
                    Call sBuilder.Append(Path & "; ")
                Next
                Return New FileHandle With {
                    ._FilePath = sBuilder.ToString,
                    ._ChunkBuffer = (From Path As String In [Handles] Select ReadBuffer(Path)).ToArray
                }
            End Function

            Public Function GetValue(p As Integer) As KeyValuePair(Of String, Double)()
                Dim GetBuffer = Function(ChunkBuffer As String(), Heads As String()) As KeyValuePair(Of String, Double)()
                                    Dim DataRow = RowObject.TryParse(ChunkBuffer(p))
                                    Dim LQuery = (From i As Integer
                                                  In Heads.Sequence.Skip(1).AsParallel
                                                  Select New KeyValuePair(Of String, Double)(Heads(i), Val(DataRow(i)))).ToArray
                                    Return LQuery
                                End Function
                Dim List As List(Of KeyValuePair(Of String, Double)) = New List(Of KeyValuePair(Of String, Double))
                For Each Buffer In _ChunkBuffer
                    Call List.AddRange(GetBuffer(Buffer.Value, Buffer.Key))
                Next

                Return List.ToArray
            End Function
        End Class

        Protected Friend ReadOnly _FullCellSystemMap As Interactions(), _SystemNodes As NodeAttributes()
        Dim _FullCellSystemMapHandles As FileHandle, _SystemNodeHandlers As FileHandle

        Sub New(FullCellSystemMap As Interactions(), NodeAttributes As NodeAttributes())
            _SystemNodes = NodeAttributes
            _FullCellSystemMap = FullCellSystemMap
        End Sub

        ''' <summary>
        ''' 计算结果的输出Csv文件的文件名列表(MetabolismFlux类型)
        ''' </summary>
        ''' <param name="Handles"></param>
        ''' <remarks></remarks>
        Public Sub AddCellSystemMapHandlers([Handles] As String())
            _FullCellSystemMapHandles = FileHandle.CreateObject([Handles])
        End Sub

        ''' <summary>
        ''' 计算结果的输出Csv文件的文件名列表(EntityObject类型)
        ''' </summary>
        ''' <param name="Handles"></param>
        ''' <remarks></remarks>
        Public Sub AddSystemNodesHandles([Handles] As String())
            _SystemNodeHandlers = FileHandle.CreateObject([Handles])
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p">Time point</param>
        ''' <param name="Interactions"></param>
        ''' <param name="NodeAttributes"></param>
        ''' <remarks></remarks>
        Public Sub TrimMap(p As Integer, ByRef Interactions As Interactions(), ByRef NodeAttributes As NodeAttributes())
            Dim SystemMapData = _FullCellSystemMapHandles.GetValue(p), NodesQuantity = _SystemNodeHandlers.GetValue(p)
            Interactions = (From item In Me._FullCellSystemMap Let FluxValue = GetItemValue(SystemMapData, item.UniqueId) Where FluxValue > 1 Select item).ToArray

            '由于在NodeAttributes表之中也包含有Flux对象，故而查找NodesQuantity表的时候总是找不到，在这里使用小于-100的情况来表示目标对象为Flux对象
            NodeAttributes = (From item In Me._SystemNodes Let Quantity = GetItemValue(NodesQuantity, item.ID) Where Quantity > 1 OrElse Quantity < -1000 Select item).ToArray
            Interactions = (From item In Interactions Let FromNode = GetItemValue(NodesQuantity, item.FromNode) Where FromNode > 1 OrElse FromNode < -1000 Select item).ToArray
            Interactions = (From item In Interactions Let ToNode = GetItemValue(NodesQuantity, item.ToNode) Where ToNode > 1 OrElse ToNode < -1000 Select item).ToArray
        End Sub

        Private Shared Function GetItemValue(DataCollection As KeyValuePair(Of String, Double)(), UniqueId As String) As Double
            Dim LQuery = (From item In DataCollection.AsParallel Where String.Equals(item.Key, UniqueId) Select item.Value).ToArray
            If LQuery.IsNullOrEmpty Then
                Return -100000
            Else
                Return LQuery.First
            End If
        End Function
    End Class
End Namespace
