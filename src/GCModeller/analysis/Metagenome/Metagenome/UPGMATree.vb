#Region "Microsoft.VisualBasic::cf64b12d04f4410ad04eb7741d242ab9, analysis\Metagenome\Metagenome\UPGMATree.vb"

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

    '   Total Lines: 134
    '    Code Lines: 108 (80.60%)
    ' Comment Lines: 5 (3.73%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 21 (15.67%)
    '     File Size: 4.31 KB


    ' Module UPGMATree
    ' 
    '     Function: BuildTree, combine, findMin, form_taxas
    '     Class Value
    ' 
    ' 
    ' 
    '     Class Taxa
    ' 
    '         Properties: Size
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Math
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' 基于矩阵数据结构（两两比较的得分/距离矩阵），最常用的从零构建进化树的算法是 UPGMA（Unweighted Pair Group Method with Arithmetic Mean）算法。
''' 
''' UPGMA 是一种自底向上的聚类算法，它假设分子钟假说（即所有物种的进化速率相同），最终生成一棵有根树。
''' > https://en.wikipedia.org/wiki/UPGMA
''' </summary>
Public Module UPGMATree

    Public Class Value
        Public size%
        Public distance#
    End Class

    Public Class Taxa : Inherits Tree(Of Value)

        Public ReadOnly Property Size As Double
            Get
                Return Data.size
            End Get
        End Property

        Sub New(id%, data As Taxa(), size%, distance#)
            Me.ID = id
            Me.Childs = data.Select(Function(x) CType(x, Tree(Of Value))).ToDictionary
            Me.Data = New Value With {
                .size = size,
                .distance = distance
            }
        End Sub

        Sub New(id%, data$, size%, distance#)
            Me.id = id
            Me.label = data
            Me.Data = New Value With {
                .size = size,
                .distance = distance
            }
        End Sub

        Public Overrides Function ToString() As String
            If Childs.IsNullOrEmpty Then
                Return Label
            Else
                With Childs
                    If .Count = 1 Then
                        Return .First.ToString
                    Else
                        Return $"({ .First.ToString}, { .Last.ToString}: {Data.size.ToString("F2")})"
                    End If
                End With
            End If
        End Function
    End Class

    Private Function form_taxas(species As Taxa()) As Dictionary(Of Integer, Taxa)
        Dim taxas As New Dictionary(Of Integer, Taxa)
        Dim ids As Integer = 1

        For Each taxa As Taxa In species
            Dim x As New Taxa(ids, {taxa}, 1, 0)
            taxas(x.ID) = x
            ids = ids + 1
        Next

        Return taxas
    End Function

    Private Function findMin(dic%(), array As List(Of Double())) As (i%, j%, lowest#)
        Dim lowest# = Integer.MaxValue
        Dim iMin As Integer = 0
        Dim jMin As Integer = 0

        For Each i As Integer In dic
            For Each j As Integer In dic
                If j > i Then
                    Dim tmp As Double = array(j - 1)(i - 1)

                    If tmp <= lowest Then
                        iMin = i
                        jMin = j
                        lowest = tmp
                    End If
                End If
            Next
        Next

        Return (iMin, jMin, lowest)
    End Function

    Private Function combine(dicTaxas As Dictionary(Of Integer, Taxa), matrix As List(Of Double())) As Taxa
        Dim n As Integer = dicTaxas.Count

        Do While dicTaxas.Count <> 1
            Dim x As (i%, j%, dij#) = findMin(dicTaxas.Keys.ToArray, matrix)
            Dim i = x.i
            Dim j = x.j
            Dim dij = x.dij
            Dim icluster As Taxa = dicTaxas(i)
            Dim jcluster As Taxa = dicTaxas(j)
            Dim u As New Taxa(dicTaxas.Keys.Max + 1, {icluster, jcluster}, icluster.Size + jcluster.Size, dij)

            Call dicTaxas.Remove(i)
            Call dicTaxas.Remove(j)
            Call matrix.Add(New Vector(u.ID - 1))

            For Each l In dicTaxas.Keys
                Dim dil = matrix(Max(i, l) - 1)(Min(i, l) - 1)
                Dim djl = matrix(Max(j, l) - 1)(Min(j, l) - 1)
                Dim dul = (dil * icluster.Size + djl * jcluster.Size) / (icluster.Size + jcluster.Size)

                matrix(u.ID - 1)(l - 1) = dul
            Next

            dicTaxas(u.ID) = u
        Loop

        ' 循环的退出条件为字典之中只有一个值
        Return dicTaxas.Values.First
    End Function

    ''' <summary>
    ''' Create taxonomy tree from a matrix
    ''' </summary>
    ''' <param name="data">rows in a numeric matrix</param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildTree(data As IEnumerable(Of DataSet)) As Taxa
        Dim rows As DataSet() = data.ToArray
        Dim inputs As Taxa() = rows.Select(Function(x) New Taxa(0, x.ID, 0, 0)).ToArray
        Dim matrix As IEnumerable(Of Double()) = rows.Matrix
        Dim table As Dictionary(Of Integer, Taxa) = form_taxas(inputs)
        Dim tree As Taxa = combine(table, matrix.AsList)
        Return tree
    End Function
End Module
