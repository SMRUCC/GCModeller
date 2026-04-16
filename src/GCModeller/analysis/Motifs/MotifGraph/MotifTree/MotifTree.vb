#Region "Microsoft.VisualBasic::262cac28851500de5b587906c8c64955, analysis\Motifs\MotifGraph\MotifTree\MotifTree.vb"

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

    '   Total Lines: 20
    '    Code Lines: 14 (70.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (30.00%)
    '     File Size: 701 B


    ' Class MotifTree
    ' 
    '     Function: MakeTree
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MotifTree

    Dim clusters As BTreeCluster()

    Public Shared Function MakeTree(motifs As IEnumerable(Of Probability), equals As Double, gt As Double) As MotifTree
        Dim motifSet As New MotifComparison(motifs, equals, gt)
        Dim tree As BTreeCluster = motifSet.motifIDs.BTreeCluster(alignment:=motifs)
        Dim clusters As New List(Of BTreeCluster)

        Call BTreeCluster.PullAllClusterNodes(tree, pull:=clusters)

        Return New MotifTree With {
            .clusters = clusters.ToArray
        }
    End Function

End Class

