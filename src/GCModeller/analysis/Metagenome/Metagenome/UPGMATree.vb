#Region "Microsoft.VisualBasic::249f85ca2c6093bf8fa3a944fee28182, GCModeller\analysis\Metagenome\Metagenome\UPGMATree.vb"

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
    '    Code Lines: 108
    ' Comment Lines: 5
    '   Blank Lines: 21
    '     File Size: 4.17 KB


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
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' https://github.com/graph1994/UPGMA-Tree-Building-Application/blob/master/UPGMATreeCreator.py
''' https://en.wikipedia.org/wiki/UPGMA
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
        Dim ids = 1

        For Each item In species
            Dim x As New Taxa(ids, {item}, 1, 0)
            taxas(x.ID) = x
            ids = ids + 1
        Next

        Return taxas
    End Function

    Private Function findMin(dic%(), array As List(Of Double())) As (i%, j%, lowest#)
        Dim lowest# = Integer.MaxValue
        Dim iMin = 0
        Dim jMin = 0

        For Each i In dic
            For Each j In dic
                If j > i Then
                    Dim tmp = array(j - 1)(i - 1)

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
        Dim n% = dicTaxas.Count

        Do While dicTaxas.Count <> 1
            Dim x As (i%, j%, dij#) = findMin(dicTaxas.Keys.ToArray, matrix)
            Dim i = x.i
            Dim j = x.j
            Dim dij = x.dij
            Dim icluster = dicTaxas(i)
            Dim jcluster = dicTaxas(j)

            Dim u As New Taxa(dicTaxas.Keys.Max + 1, {icluster, jcluster}, (icluster.Size + jcluster.Size), (dij))
            dicTaxas.Remove(i)
            dicTaxas.Remove(j)

            matrix.Add(New Vector(u.ID - 1))

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

    <Extension>
    Public Function BuildTree(data As IEnumerable(Of DataSet)) As Taxa
        Dim array = data.ToArray
        Dim inputs = array.Select(Function(x) New Taxa(0, x.ID, 0, 0)).ToArray
        Dim matrix = array.Matrix
        Dim table = form_taxas(inputs)
        Dim tree = combine(table, matrix.AsList)
        Return tree
    End Function
End Module
