#Region "Microsoft.VisualBasic::e2e18320b1ab0f190a60ca65f280f8aa, modules\Knowledge_base\Knowledge_base\NLP\GeneName.vb"

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

    '   Total Lines: 73
    '    Code Lines: 58 (79.45%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 15 (20.55%)
    '     File Size: 2.97 KB


    ' Module GeneName
    ' 
    '     Function: (+2 Overloads) GroupBy
    '     Class TextSimilar
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetDPSimilarity, GetObject, GetSimilarity
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports std = System.Math

Public Module GeneName

    <Extension>
    Public Function GroupBy(Of EntityObject As {INamedValue, DynamicPropertyBase(Of String)})(genes As IEnumerable(Of EntityObject), field As String, Optional cutoff As Double = 0.3) As IEnumerable(Of NamedCollection(Of EntityObject))
        Return genes.GroupBy(Function(d) d(field), cutoff)
    End Function

    <Extension>
    Public Iterator Function GroupBy(Of T As INamedValue)(genes As IEnumerable(Of T), field As Func(Of T, String), Optional cutoff As Double = 0.3) As IEnumerable(Of NamedCollection(Of T))
        Dim tree As New AVLTree(Of String, String)(New TextSimilar(cutoff).GetComparer)
        Dim gene_id As New Dictionary(Of String, T)

        For Each gene As T In genes
            gene_id.Add(gene.Key, gene)
            tree.Add(field(gene), gene.Key)
        Next

        Dim text_clusters = tree.root.PopulateNodes.ToArray

        For Each cluster As BinaryTree(Of String, String) In text_clusters
            Yield New NamedCollection(Of T)(cluster.Key, cluster.Members.Select(Function(id) gene_id(id)))
        Next
    End Function

    Private Class TextSimilar : Inherits ComparisonProvider

        ReadOnly matrix As ScoreMatrix(Of Char)
        ReadOnly symbol As GenericSymbol(Of Char) = GetGeneralCharSymbol()

        Sub New(cutoff As Double)
            MyBase.New(cutoff, cutoff / 2)
            matrix = New ScoreMatrix(Of Char)(symbol)
        End Sub

        Private Function GetDPSimilarity(x As String, y As String) As Double
            Dim gnw As New NeedlemanWunsch(Of Char)(x, y, matrix, symbol)
            Dim best As GlobalAlign(Of Char) = gnw _
                .Compute() _
                .PopulateAlignments _
                .OrderByDescending(Function(a) a.score) _
                .FirstOrDefault

            Return best.score / best.Length
        End Function

        Public Overrides Function GetSimilarity(x As String, y As String) As Double
            Dim tx As String() = x.Split
            Dim ty As String() = y.Split
            Dim len As Integer = std.Min(tx.Length, ty.Length)

            For i As Integer = 0 To len - 1
                If Not tx(i).TextEquals(ty(i)) Then
                    Return i / len
                End If
            Next

            Return 1.0
        End Function

        Public Overrides Function GetObject(id As String) As Object
            Return id
        End Function
    End Class
End Module
