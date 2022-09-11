#Region "Microsoft.VisualBasic::3e15bf5493c7463ffd1d49e7a46cb83b, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\Heatmap\TreeOrder.vb"

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

    '   Total Lines: 84
    '    Code Lines: 69
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 3.23 KB


    '     Module TreeOrder
    ' 
    '         Function: GetTreeOrder, OrderByTree
    ' 
    '         Sub: ReOrder
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace CatalogProfiling

    Module TreeOrder

        <Extension>
        Public Function OrderByTree(samples As MultipleCatalogHeatmap) As String()
            Dim allSamples = samples.multiples.ToDictionary(Function(a) a.Name, Function(a) a.Value)
            Dim matrix As New Dictionary(Of String, Vector)
            Dim allPathways As String() = allSamples.Values _
                .Select(Function(a) a.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.termId) _
                .Distinct _
                .ToArray

            For Each sample In allSamples
                Dim bubbles = sample.Value.Values.IteratesALL.ToDictionary(Function(b) b.termId)
                Dim [if] As Double() = allPathways _
                    .Select(Function(name)
                                Dim b As BubbleTerm = bubbles.TryGetValue(name)

                                If b Is Nothing Then
                                    Return 0.0
                                Else
                                    Return b.Factor * b.PValue * b.data
                                End If
                            End Function) _
                    .ToArray

                matrix(sample.Key) = [if].AsVector
            Next

            Dim compareSample As Comparison(Of String) =
                Function(sample1, sample2)
                    Dim v1 As Vector = matrix(sample1)
                    Dim v2 As Vector = matrix(sample2)
                    Dim cos As Double = v1.SSM(v2)

                    If cos > 0.9 Then
                        Return 0
                    ElseIf cos > 0.8 Then
                        Return 1
                    Else
                        Return -1
                    End If
                End Function
            Dim tree As New AVLTree(Of String, String)(compareSample)

            For Each sampleName As String In allSamples.Keys
                Call tree.Add(sampleName, sampleName, valueReplace:=False)
            Next

            Return tree.GetTreeOrder.ToArray
        End Function

        <Extension>
        Private Iterator Function GetTreeOrder(tree As AVLTree(Of String, String)) As IEnumerable(Of String)
            Dim orders = tree.root.PopulateNodes.ToArray

            For Each node As BinaryTree(Of String, String) In orders
                Dim members As IEnumerable(Of String) = node.Members

                For Each name As String In members
                    Yield name
                Next
            Next
        End Function

        <Extension>
        Public Sub ReOrder(samples As MultipleCatalogHeatmap, orders As String())
            Dim index = samples.multiples.ToDictionary(Function(a) a.Name)

            Call samples.multiples.Clear()
            Call orders.ForEach(Sub(i, null) samples.multiples.Add(index(i)))
        End Sub
    End Module
End Namespace
