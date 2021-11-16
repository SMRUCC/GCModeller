#Region "Microsoft.VisualBasic::bf9bc5aea538792386326cffff59ace5, analysis\PhenoGraph\test\Program.vb"

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

    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.Microarray.PhenoGraph

Module Program

    Const demoDir As String = "D:\GCModeller\src\GCModeller\analysis\PhenoGraph\demo"

    Sub Main()
        Dim data As DataSet() = DataSet.LoadDataSet($"{demoDir}\HR2MSI mouse urinary bladder S096_top3.csv").ToArray
        Dim graph As NetworkGraph = CommunityGraph.CreatePhenoGraph(data, k:=300, cutoff:=0)

        Call graph.Tabular.Save($"{demoDir}\HR2MSI mouse urinary bladder S096_graph/")

        Pause()
    End Sub

End Module
