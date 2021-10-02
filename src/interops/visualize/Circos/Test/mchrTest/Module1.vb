#Region "Microsoft.VisualBasic::767eabbe71aadb82b63e7397e6e02314, visualize\Circos\Test\mchrTest\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: Main, run
    ' 
    ' /********************************************************************************/

#End Region

Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Circos
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Circos.Configurations.Nodes.Plots
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Circos.Karyotype
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Circos.TrackDatas
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Circos.TrackDatas.Highlights
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Circos.TrackDatas.NtProps
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Module Module1

    Sub Main()
        Try
            Call run()
        Catch ex As Exception
            Call ex.PrintException
        End Try

        Pause()
    End Sub

    Private Sub run()
        Dim fas As New FastaFile("H:\5.14.circos\6.22\Af293.fna")
        Dim maps As BlastnMapping() = "H:\5.14.circos\6.22\maps.MergeMappings-Trim.Full.Perfect.identities=0.9.Csv".LoadCsv(Of BlastnMapping)
        Dim genome As KaryotypeChromosomes = KaryotypeChromosomes.FromNts(fas)
        Call genome.GenerateDocument(0).SaveTo("x:/test.txt")

        Dim circos = CircosAPI.CreateDoc
        circos.SkeletonKaryotype = genome
        circos.Includes.Add(New Configurations.Ideogram(circos))
        circos.Includes.Add(New Configurations.Ticks(circos))

        circos.GetIdeogram.Ideogram.show_label = yes
        circos.GetIdeogram.Ideogram.Spacing.default = "0.2u"
        circos.chromosomes_units = "1000000"

        Dim hhhh = Function() maps.Hits(circos.SkeletonKaryotype)
        Dim inn = hhhh.BeginInvoke(Nothing, Nothing)

        circos.AddPlotElement(New Histogram(New GCSkew(genome:=fas, karyotype:=circos.SkeletonKaryotype, SlideWindowSize:=4096, Steps:=2048, Circular:=True)))
        circos.AddPlotElement(New Histogram(New GeneGCContent(genome:=fas, karyotype:=circos.SkeletonKaryotype, winSize:=4096, steps:=2048, getValue:=Function(x) x.GC_AT)))
        circos.AddPlotElement(New Histogram(New GradientMappings(maps.IdentitiesTracks(circos.SkeletonKaryotype), circos.SkeletonKaryotype, "Jet", 4096)))
        circos.AddPlotElement(New Histogram(New TrackDatas.data(Of TrackDatas.ValueTrackData)(hhhh.EndInvoke(inn))))

        Call circos.Save("x:\test/")
    End Sub
End Module
