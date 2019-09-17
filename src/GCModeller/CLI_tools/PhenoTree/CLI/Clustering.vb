#Region "Microsoft.VisualBasic::ba8fe0bde897d946855b257b682d4cf0, CLI_tools\PhenoTree\CLI\Clustering.vb"

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

    ' Module CLI
    ' 
    '     Function: LociClustering
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.ClusterMatrix
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/locis.clustering",
               Usage:="/locis.clustering /in <locis.fasta> [/cut <0> /first.ID /method <NeedlemanWunsch> /colors <clusters> /clusters <20> /out <out.DIR>]")>
    Public Function LociClustering(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim method$ = args.GetValue("/method", "NeedlemanWunsch")
        Dim expected% = args.GetValue("/clusters", 20)
        Dim cut# = args.GetValue("/cut", 0R)
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & $"-{method}_expected={expected}_cut={cut}.clusters/")
        Dim fasta As FastaFile = FastaFile.LoadNucleotideData([in])
        Dim firstID As Boolean = args.GetBoolean("/first.ID")

        If firstID Then
            Call fasta.FirstTokenID
        End If

        Dim arguments As New NameValueCollection

        Call arguments.Add("cut", cut)

        Dim matrix As DataSet() = fasta.SimilarityMatrix(arguments)
        Dim clusters As EntityClusterModel() = matrix.KMeans(expected)
        Dim colors As Color() = Designer.GetColors(args <= "/colors", expected)
        Dim clusterColors As Dictionary(Of String, Color) = clusters _
            .Select(Function(x) x.Cluster) _
            .Distinct _
            .SeqIterator _
            .ToDictionary(Function(cluster) +cluster,
                          Function(color) colors(color))

        Call clusters.SaveTo(out & "/clusters.csv")
        Call clusters.ToNetwork(
            clusterColors,
            cut:=Function(score) score > cut).Save(out)

        Return 0
    End Function
End Module
