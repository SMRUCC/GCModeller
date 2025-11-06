#Region "Microsoft.VisualBasic::c4b073ab2b4a5c7cf4c9c5852f3bc8d0, analysis\RNA-Seq\WGCNA\Network\WGCNA.vb"

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

'   Total Lines: 123
'    Code Lines: 84 (68.29%)
' Comment Lines: 21 (17.07%)
'    - Xml Docs: 95.24%
' 
'   Blank Lines: 18 (14.63%)
'     File Size: 4.58 KB


'     Class WGCNAWeight
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateMatrix, createMatrixInternal, (+3 Overloads) Find, GenericEnumerator, GetValue
'                   Subset
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace Network

    ''' <summary>
    ''' 包含有结果数据的加载模块以及脚本的执行调用模块
    ''' </summary>
    Public Class WGCNAWeight : Implements Enumeration(Of Weight)

        Dim matrix As Dictionary(Of String, Dictionary(Of String, Weight))
        Dim geneList As IReadOnlyCollection(Of String)

        Default Public ReadOnly Property Iteration(geneId1 As String, geneId2 As String) As Weight
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Find(geneId1, geneId2)
            End Get
        End Property

        Public ReadOnly Property geneSet As IEnumerable(Of String)
            Get
                Return geneList.AsEnumerable
            End Get
        End Property

        Private Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dataSet"></param>
        ''' <param name="allIds">
        ''' WGCNA共表达网络的导出结果为一个三角形矩阵，所有的基因ID都在这个三角形矩阵的上半部分，
        ''' 这里的allIds参数用于获取所有的基因ID列表，以便于后续的计算使用。
        ''' 
        ''' 如果不需要获取所有的基因ID列表，可以传入Nothing
        ''' </param>
        ''' <returns></returns>
        Private Shared Function buildMatrixInternal(dataSet As IEnumerable(Of Weight), ByRef allIds As IReadOnlyCollection(Of String)) As Dictionary(Of String, Dictionary(Of String, Weight))
            Dim matrix As New Dictionary(Of String, Dictionary(Of String, Weight))
            Dim pullAll As Weight() = dataSet.SafeQuery.ToArray
            Dim groupByFromNode = From itr As Weight
                                  In pullAll
                                  Select itr
                                  Group itr By itr.fromNode Into Group

            For Each fromGroup In groupByFromNode
                matrix(fromGroup.fromNode) = fromGroup.Group _
                    .ToDictionary(Function(itr)
                                      Return itr.toNode
                                  End Function)
            Next

            allIds = pullAll _
                .Select(Iterator Function(w) As IEnumerable(Of String)
                            Yield w.fromNode
                            Yield w.toNode
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray

            Return matrix
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateMatrix(dataSet As IEnumerable(Of Weight)) As WGCNAWeight
            Return New WGCNAWeight With {
                .matrix = buildMatrixInternal(dataSet, .geneList)
            }
        End Function

        ''' <summary>
        ''' 找不到会返回空值
        ''' </summary>
        ''' <param name="geneId1"></param>
        ''' <param name="geneId2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Find(geneId1 As String, geneId2 As String) As Weight
            Dim w As Weight
            Dim row As Dictionary(Of String, Weight)

            If matrix.ContainsKey(geneId1) Then
                row = matrix(geneId1)
                w = row.TryGetValue(geneId2)
            ElseIf matrix.ContainsKey(geneId2) Then
                row = matrix(geneId2)
                w = row.TryGetValue(geneId1)
            Else
                w = Nothing
            End If

            Return w
        End Function

        ''' <summary>
        ''' find all correlated genes
        ''' </summary>
        ''' <param name="geneId"></param>
        ''' <returns></returns>
        Public Function Find(geneId As String) As IEnumerable(Of Weight)
            If matrix.ContainsKey(geneId) Then
                Return matrix(geneId).Values
            Else
                Return New Weight() {}
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Find(geneId As String, cutOff As Double) As Weight()
            Return Find(geneId).Where(Function(itr) std.Abs(itr.weight) >= cutOff)
        End Function

        ''' <summary>
        ''' 将目标对象相关的WGCNA weight值过滤出来，作为计算数据，以减少计算开销
        ''' </summary>
        ''' <param name="geneList"></param>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Subset(geneList As IEnumerable(Of String)) As WGCNAWeight
            Return New WGCNAWeight With {
                .matrix = geneList.ToDictionary(Function(id) id, Function(id) matrix(id))
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetValue(geneId1 As String, geneId2 As String) As Double
            Dim iteration As Weight = Find(geneId1, geneId2)

            If iteration Is Nothing Then
                Return 0
            Else
                Return iteration.weight
            End If
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Weight) Implements Enumeration(Of Weight).GenericEnumerator
            For Each row As KeyValuePair(Of String, Dictionary(Of String, Weight)) In matrix
                For Each col As KeyValuePair(Of String, Weight) In row.Value
                    Yield col.Value
                Next
            Next
        End Function
    End Class
End Namespace
