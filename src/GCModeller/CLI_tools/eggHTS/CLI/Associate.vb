#Region "Microsoft.VisualBasic::9db82275fd7571ec147e0a627ee99f5e, CLI_tools\eggHTS\CLI\Associate.vb"

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
'     Function: KOBASKMeans, KOBASSimilarity, PccNetwork, SimHeatmap
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.Heatmap
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS

Partial Module CLI

    <ExportAPI("/Network.PCC")>
    <Usage("/Network.PCC /in <matrix.csv> [/cut <default=0.45> /out <out.DIR>]")>
    Public Function PccNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim cut# = args.GetValue("/cut", 0.45)
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".PCC/").AsDefault
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray

        With CorrelationNetwork.BuildNetwork(matrix, cut)
            Call .matrix.PopulateRowObjects(Of DataSet).SaveTo(out & "/matrix.csv")

            Return .net.Tabular() _
                   .Save(out) _
                   .CLICode
        End With
    End Function

    <ExportAPI("/KOBAS.similarity")>
    <Usage("/KOBAS.Similarity /group1 <DIR> /group2 <DIR> [/fileName <default=output_run-Gene Ontology.csv> /out <out.DIR>]")>
    Public Function KOBASSimilarity(args As CommandLine) As Integer
        Dim group1$ = args <= "/group1"
        Dim group2$ = args <= "/group2"
        Dim fileName$ = args.GetValue("/fileName", "output_run-Gene Ontology.csv")
        Dim out$ = args.GetValue("/out", group1.TrimDIR & "-" & group2.BaseName & $".{fileName}/")

        Dim files1 = (ls - l - r - fileName <= group1) _
            .Select(Function(file)
                        Return file.LoadTerms(file.ParentPath.ParentDirName)
                    End Function) _
            .ToArray
        Dim files2 = (ls - l - r - fileName <= group2) _
            .Select(Function(file)
                        Return file.LoadTerms(file.ParentPath.ParentDirName)
                    End Function) _
            .ToArray

        Dim matrixA As New Dictionary(Of DataSet)
        Dim matrixB As New Dictionary(Of DataSet)
        Dim Sim As New List(Of DataSet)

        For Each a In files1
            Dim vector As New DataSet With {
                .ID = $"[{group1.BaseName}] " & a.Name,
                .Properties = New Dictionary(Of String, Double)
            }

            For Each b In files2
                With Measure.Similarity(a, b)

                    .A.ID = $"[{group1.BaseName}] " & .A.ID
                    .B.ID = $"[{group2.BaseName}] " & .B.ID

                    If Not matrixA.ContainsKey(.A.ID) Then
                        matrixA += .A
                    End If
                    If Not matrixB.ContainsKey(.B.ID) Then
                        matrixB += .B
                    End If

                    vector.Properties.Add($"[{group2.BaseName}] " & b.Name, .similarity)
                End With
            Next

            Sim += vector
        Next

        Call matrixA.SaveTo(out & $"/{group1.BaseName}.csv")
        Call matrixB.SaveTo(out & $"/{group2.BaseName}.csv")
        Call Sim.SaveTo(out & $"/SimOf-{fileName}")

        Return 0
    End Function

    <ExportAPI("/KOBAS.Term.Kmeans")>
    <Usage("/KOBAS.Term.Kmeans /in <dir.input> [/n <default=3> /out <out.clusters.csv>]")>
    Public Function KOBASKMeans(args As CommandLine) As Integer
        Dim out$ = (args <= "/out") Or $"{(args <= "/in").TrimDIR}.clusters.csv".AsDefault
        Dim files As DataSet() = (ls - l - r - "*.csv" <= (args <= "/in")) _
            .Select(Function(file) DataSet.LoadDataSet(file)) _
            .IteratesALL _
            .ToArray
        Dim allTerms = files.PropertyNames.Distinct.Sort.ToArray
        Dim strip As New List(Of DataSet)
        Dim n% = args.GetValue("/n", 3)

        ' 补充零，将所有的向量的长度置位等长
        For Each vector As DataSet In files
            strip += New DataSet With {
                .ID = vector.ID,
                .Properties = allTerms.ToDictionary(
                    Function(key) key,
                    Function(term) vector(term))
            }
        Next

        Return strip _
            .ToKMeansModels _
            .Kmeans(expected:=n) _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/KOBAS.Sim.Heatmap")>
    <Usage("/KOBAS.Sim.Heatmap /in <sim.csv> [/size <1024,800> /colors <RdYlBu:8> /out <out.png>]")>
    Public Function SimHeatmap(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = (args <= "/out") Or $"{[in].TrimSuffix}.heatmap.png".AsDefault
        Dim size$ = (args <= "/size") Or "1024,800".AsDefault
        Dim colors$ = (args <= "/colors") Or "RdYlBu:8".AsDefault
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray

        Return Heatmap _
            .Plot(matrix, reverseClrSeq:=True, mapName:=colors, size:=size) _
            .Save(out) _
            .CLICode
    End Function
End Module
