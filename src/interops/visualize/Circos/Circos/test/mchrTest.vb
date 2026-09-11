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

Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.NtProps

Module Module1

    Private Sub run()
        Dim fas As New FastaFile("H:\5.14.circos\6.22\Af293.fna")
        Dim maps As BlastnMapping() = "H:\5.14.circos\6.22\maps.MergeMappings-Trim.Full.Perfect.identities=0.9.Csv".LoadCsv(Of BlastnMapping)
        Dim genome As GenomeKaryotype = ChromosomeGenerator.FromNts(fas)
        Call genome.Save("x:/test.txt")

        Dim circos As New Circos
        circos.skeletonKaryotype = genome
        circos.includes.Add(New Configurations.IdeogramInclude(circos))
        circos.includes.Add(New Configurations.TicksInclude(circos))

        circos.Ideogram.Ideogram.show_label = yes
        circos.Ideogram.Ideogram.Spacing.default = "0.2u"
        circos.chromosomes_units = "1000000"

        Dim hhhh = Function() maps.Hits(circos.skeletonKaryotype)
        Dim inn = hhhh.BeginInvoke(Nothing, Nothing)

        circos.AddTrack(New Histogram(New GCSkew(fas, karyotype:=circos.skeletonKaryotype, winSize:=4096, steps:=2048, isCircular:=True)))
        circos.AddTrack(New Histogram(New GeneGCContent(genome:=fas, karyotype:=circos.skeletonKaryotype, winSize:=4096, steps:=2048, getValue:=Function(x) x.GC_AT)))
        circos.AddTrack(New Histogram(New GradientMappings(maps.IdentitiesTracks(circos.skeletonKaryotype), "Jet")))
        circos.AddTrack(New Histogram(New TrackDatas.TrackDataDocument(Of TrackDatas.ValueTrackData)(hhhh.EndInvoke(inn))))

        Call circos.Save("Z:\circos-test/")
    End Sub
End Module
