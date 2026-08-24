#Region "Microsoft.VisualBasic::3cad3194760eeae6b74db7f4156a362e, annotations\WGCNA\test\networkTest.vb"

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

    '   Total Lines: 15
    '    Code Lines: 12 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (20.00%)
    '     File Size: 678 B


    ' Module networkTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.HTS.WGCNA

Module networkTest

    Sub Main()
        Dim adj = CorrelationNetwork.LoadAdjacencyMatrix("C:\Users\Administrator\Downloads\WGCNA_output\adjacency_matrix.csv")
        Dim mods = ModuleMembershipResult.ReadModuleAssignment("C:\Users\Administrator\Downloads\WGCNA_output\gene_module_assignment.csv").ToArray
        Dim g As NetworkGraph = adj.ExportGraph(mods, adj_thres:=0.8)

        Call g.Tabular.Save("Z:/wgcna")
    End Sub
End Module

