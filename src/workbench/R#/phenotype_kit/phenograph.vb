#Region "Microsoft.VisualBasic::83e1032723f0b7a0bd2c47553b8d70c7, R#\phenotype_kit\phenograph.vb"

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

    ' Module phenograph
    ' 
    '     Function: phenograph
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SingleCell.PhenoGraph

''' <summary>
''' PhenoGraph algorithm
''' 
''' Jacob H. Levine and et.al. Data-Driven Phenotypic Dissection of AML Reveals Progenitor-like Cells that Correlate with Prognosis. Cell, 2015.
''' </summary>
<Package("phenograph")>
Module phenograph

    ''' <summary>
    ''' PhenoGraph algorithm
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="k"></param>
    ''' <param name="cutoff"></param>
    ''' <returns></returns>
    <ExportAPI("phenograph")>
    Public Function phenograph(matrix As Matrix, Optional k As Integer = 30, Optional cutoff As Double = 0) As NetworkGraph
        Dim sampleId = matrix.sampleID.SeqIterator.ToArray
        Dim dataset As DataSet() = matrix.expression _
            .Select(Function(gene)
                        Return New DataSet With {
                            .ID = gene.geneID,
                            .Properties = sampleId _
                                .ToDictionary(Function(id) id.value,
                                              Function(i)
                                                  Return gene.experiments(i)
                                              End Function)
                        }
                    End Function) _
            .Transpose
        Dim graph As NetworkGraph = CommunityGraph.CreatePhenoGraph(
            data:=dataset,
            k:=k,
            cutoff:=cutoff
        )

        Return graph
    End Function

End Module
