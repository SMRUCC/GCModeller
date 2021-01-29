#Region "Microsoft.VisualBasic::9cf22408ae2efd2909794b590a6c7d8f, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixDatabase\WGCNA\WGCNA.vb"

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

    '     Class WGCNAWeight
    ' 
    '         Properties: PairItems
    ' 
    '         Function: __buildHashs, (+3 Overloads) Find, GetValue
    ' 
    '         Sub: Filtering
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Network

    ''' <summary>
    ''' 包含有结果数据的加载模块以及脚本的执行调用模块
    ''' </summary>
    Public Class WGCNAWeight

        Public Property PairItems As Weight()
            Get
                Return __pairItems
            End Get
            Set
                If Value Is Nothing Then
                    Value = New Weight() {}
                End If

                __pairItems = Value
                __innerHash = __buildHashs(Value)
            End Set
        End Property

        Dim __innerHash As Dictionary(Of String, Dictionary(Of String, Weight))
        Dim __pairItems As Weight()

        Private Shared Function __buildHashs(dataSet As Weight()) As Dictionary(Of String, Dictionary(Of String, Weight))
            If dataSet.IsNullOrEmpty Then
                Return New Dictionary(Of String, Dictionary(Of String, Weight))
            End If

            Dim p1Group = (From obj As Weight
                           In dataSet
                           Select obj
                           Group obj By obj.FromNode Into Group).ToArray
            Dim p2Group = (From obj In p1Group.AsParallel
                           Select obj.FromNode,
                               gr = (From go As Weight
                                     In obj.Group.ToArray
                                     Select go
                                     Group go By go.ToNode Into Group).ToArray) _
                                     .ToDictionary(Function(obj) obj.FromNode,
                                                   elementSelector:=Function(obj) _
                                                                        obj.gr.ToDictionary(
                                                                        Function(key) key.ToNode,
                                                                        elementSelector:=Function(value) value.Group.ToArray()(Scan0)))
            Return p2Group
        End Function

        ''' <summary>
        ''' 找不到会返回空值
        ''' </summary>
        ''' <param name="Id1"></param>
        ''' <param name="Id2"></param>
        ''' <param name="Parallel">可选参数，这个是为了控制并行计算的颗粒粒度而设置的参数，假若CPU利用率较低的话，可以尝试关闭本参数以增加颗粒粒度</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Find(Id1 As String, Id2 As String, Optional Parallel As Boolean = True) As Weight
            Dim weight As Weight = Nothing
            Dim hash As Dictionary(Of String, Weight)

            If __innerHash.ContainsKey(Id1) Then
                hash = __innerHash(Id1)

                If hash.ContainsKey(Id2) Then
                    weight = hash(Id2)
                End If

            ElseIf __innerHash.ContainsKey(Id2) Then
                hash = __innerHash(Id2)

                If hash.ContainsKey(Id1) Then
                    weight = hash(Id1)
                End If
            Else
                Return Nothing
            End If

            Return weight
        End Function

        Public Function Find(Id As String) As Weight()
            Dim LQuery = (From item In PairItems.AsParallel
                          Where String.Equals(item.FromNode, Id) OrElse
                              String.Equals(item.ToNode, Id)
                          Select item).ToArray
            Return LQuery
        End Function

        Public Function Find(Id As String, CutOff As Double) As Weight()
            Dim LQuery = (From item As Weight In PairItems.AsParallel
                          Where item.Weight >= CutOff AndAlso
                              (String.Equals(item.FromNode, Id) OrElse String.Equals(item.ToNode, Id))
                          Select item).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 将目标对象相关的WGCNA weight值过滤出来，作为计算数据，以减少计算开销
        ''' </summary>
        ''' <param name="IdList"></param>
        ''' <remarks></remarks>
        Public Sub Filtering(IdList As String())
            Dim pairList As New List(Of Weight)
            For Each Id As String In IdList
                Call pairList.AddRange(Find(Id))
            Next
            Call pairList.RemoveAll(Function(item As Weight) item Is Nothing)

            Me.PairItems = pairList.ToArray
        End Sub

        Public Function GetValue(Id1 As String, Id2 As String, Optional Parallel As Boolean = True) As Double
            Dim w = Find(Id1, Id2, Parallel)
            If w Is Nothing Then
                Return 0
            Else
                Return w.Weight
            End If
        End Function
    End Class
End Namespace
