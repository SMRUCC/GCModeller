#Region "Microsoft.VisualBasic::43d4d22a287ebf41ed5681c79ff22021, analysis\OperonMapper\NTTree.vb"

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

    '   Total Lines: 63
    '    Code Lines: 51 (80.95%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (19.05%)
    '     File Size: 1.88 KB


    ' Class NTTree
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetClusters, GetObject, GetSimilarity
    ' 
    '     Sub: MakeTtree
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class NTTree : Inherits ComparisonProvider

    ReadOnly tree As New ClusterTree
    ReadOnly map As New Dictionary(Of String, NTCluster)

    Default Public ReadOnly Property Item(i As Integer) As NTCluster
        Get
            Return map(CInt(i))
        End Get
    End Property

    Public Sub New(equals As Double, gt As Double)
        MyBase.New(equals, gt)
    End Sub

    Public Sub MakeTtree(seeds As IEnumerable(Of NTCluster))
        Dim args As New ClusterTree.Argument With {
            .threshold = equalsDbl,
            .diff = 0.1,
            .alignment = Me
        }
        Dim key As String
        Dim i As Integer = 0

        For Each seed As NTCluster In seeds
            key = i.ToString
            i += 1

            Call map.Add(key, seed)
            Call args.SetTargetKey(key)
            Call ClusterTree.Add(tree, args)
        Next
    End Sub

    Public Iterator Function GetClusters() As IEnumerable(Of NTCluster)
        Dim class_id As Integer = 1

        For Each node In ClusterTree.GetClusters(tree)
            For Each key As String In node.Members
                Dim gene As NTCluster = map(key)
                gene.cluster = class_id.ToString
                Yield gene
            Next

            class_id += 1
        Next
    End Function

    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Dim a = map(x)
        Dim b = map(y)
        Dim jac As Double = New Vector(a.fingerprint).Tanimoto(New Vector(b.fingerprint))
        Return jac
    End Function

    Public Overrides Function GetObject(id As String) As Object
        Return map(id)
    End Function
End Class

