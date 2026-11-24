#Region "Microsoft.VisualBasic::d52f5c271d30c69faa0271cf31747dfa, R#\circoskit\Document.vb"

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

    ' Module Document
    ' 
    '     Function: AddTrack, ConfigCircosBackbone, CreateDataModel, GetCircosScript, GetIdeogram
    '               getMain, Save
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize.Circos
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots

Partial Module circosTool

    ''' <summary>
    ''' Creats a new <see cref="Circos"/> plots configuration document.
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="use_rules"></param>
    ''' <param name="chromosomes_units"></param>
    ''' <param name="chromosomes_display_default"></param>
    ''' <param name="chromosomes"></param>
    ''' <param name="chromosomes_reverse"></param>
    ''' <param name="chromosomes_radius"></param>
    ''' <param name="chromosomes_scale"></param>
    ''' <param name="chromosomes_color"></param>
    ''' <param name="chromosomes_order"></param>
    ''' <param name="chromosomes_breaks"></param>
    ''' <param name="show_scatter"></param>
    ''' <param name="show_line"></param>
    ''' <param name="show_histogram"></param>
    ''' <param name="show_heatmap"></param>
    ''' <param name="show_tile"></param>
    ''' <param name="show_highlight"></param>
    ''' <param name="show_links"></param>
    ''' <param name="show_highlights"></param>
    ''' <param name="show_text"></param>
    ''' <param name="show_heatmaps"></param>
    ''' <param name="track_width"></param>
    ''' <param name="track_start"></param>
    ''' <param name="track_step"></param>
    ''' <returns></returns>
    <ExportAPI("circos")>
    Public Function CreateDataModel(Optional genome$ = "",
                                    Optional use_rules$ = "yes",
                                    Optional chromosomes_units% = 5000,
                                    Optional chromosomes_display_default$ = "yes",
                                    Optional chromosomes$ = "",
                                    Optional chromosomes_reverse$ = "",
                                    Optional chromosomes_radius$ = "",
                                    Optional chromosomes_scale$ = "",
                                    Optional chromosomes_color$ = "",
                                    Optional chromosomes_order$ = "",
                                    Optional chromosomes_breaks$ = "",
                                    Optional show_scatter$ = "yes",
                                    Optional show_line$ = "yes",
                                    Optional show_histogram$ = "yes",
                                    Optional show_heatmap$ = "yes",
                                    Optional show_tile$ = "yes",
                                    Optional show_highlight$ = "yes",
                                    Optional show_links$ = "yes",
                                    Optional show_highlights$ = "yes",
                                    Optional show_text$ = "yes",
                                    Optional show_heatmaps$ = "yes",
                                    Optional track_width$ = "",
                                    Optional track_start$ = "",
                                    Optional track_step$ = "") As Circos

        Dim circos As Circos = Circos.CreateObject

        circos.genome = genome
        circos.use_rules = use_rules
        circos.chromosomes_units = chromosomes_units
        circos.chromosomes_display_default = chromosomes_display_default
        circos.chromosomes = chromosomes
        circos.chromosomes_reverse = chromosomes_reverse
        circos.chromosomes_radius = chromosomes_radius
        circos.chromosomes_scale = chromosomes_scale
        circos.chromosomes_color = chromosomes_color
        circos.chromosomes_order = chromosomes_order
        circos.chromosomes_breaks = chromosomes_breaks
        circos.show_scatter = show_scatter
        circos.show_line = show_line
        circos.show_histogram = show_histogram
        circos.show_heatmap = show_heatmap
        circos.show_tile = show_tile
        circos.show_highlight = show_highlight
        circos.show_links = show_links
        circos.show_highlights = show_highlights
        circos.show_text = show_text
        circos.show_heatmaps = show_heatmaps
        circos.track_width = track_width
        circos.track_start = track_start
        circos.track_step = track_step

        Return circos
    End Function

    <ExportAPI("backbone")>
    Public Function ConfigCircosBackbone(circos As Circos, source As FastaSeq, Optional loophole As Integer = 512) As Circos
        Call CircosAPI.SetBasicProperty(circos, nt:=source, loophole:=loophole)
        Return circos
    End Function

    ''' <summary>
    ''' Add track plot data model into circos doc object.
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="track"></param>
    ''' <returns></returns>
    <ExportAPI("add")>
    Public Function AddTrack(circos As Circos, track As ITrackPlot, Optional auto_layout As Boolean = True) As Circos
        circos.AddTrack(track, autoLayout:=auto_layout)
        Return circos
    End Function

    <ExportAPI("main")>
    Public Function getMain(circos As CircosConfig) As Circos
        Return circos.main
    End Function

    ''' <summary>
    ''' Gets the ideogram configuration node in the circos document object.
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' (还没有ideogram文档的时候，则会返回一个新的文档)
    ''' </remarks>
    <ExportAPI("ideogram")>
    <Extension>
    Public Function GetIdeogram(doc As Circos,
                                Optional thickness$ = "30p",
                                Optional stroke_thickness% = 0,
                                Optional stroke_color$ = "black",
                                Optional fill$ = "yes",
                                Optional fill_color$ = "black",
                                Optional radius$ = "0.85r",
                                Optional show_label$ = "no",
                                Optional label_font$ = "default",
                                Optional label_radius$ = "dims(ideogram,radius) + 0.05r",
                                Optional label_size% = 36,
                                Optional label_parallel$ = "yes",
                                Optional label_case$ = "upper",
                                Optional band_stroke_thickness% = 0,
                                Optional show_bands$ = "yes",
                                Optional fill_bands$ = "yes") As IdeogramInclude

        Dim LQuery As IEnumerable(Of IdeogramInclude) =
                                                       _
            From node As CircosConfig
            In doc.includes
            Where TypeOf node Is IdeogramInclude
            Select DirectCast(node, IdeogramInclude)

        Dim idNode As IdeogramInclude = LQuery.FirstOrDefault

        If idNode Is Nothing Then
            idNode = New IdeogramInclude(doc)
        End If

        With idNode.Ideogram
            .thickness = thickness
            .stroke_thickness = stroke_thickness
            .stroke_color = stroke_color
            .fill = fill
            .fill_color = fill_color
            .radius = radius
            .show_label = show_label
            .label_font = label_font
            .label_radius = label_radius
            .label_size = label_size
            .label_parallel = label_parallel
            .label_case = label_case
            .band_stroke_thickness = band_stroke_thickness
            .show_bands = show_bands
            .fill_bands = fill_bands
        End With

        Return idNode
    End Function

    ''' <summary>
    ''' Save the circos plots configuration object as the default 
    ''' configuration file: ``circos.conf``.
    ''' 
    ''' this function will always save the entire circos document into
    ''' the given directory location.
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="directory">
    ''' a required directory location for save the entire circos documents.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("save")>
    Public Function Save(circos As CircosConfig, directory As String) As String
        Return CircosAPI.WriteData(circos.main, directory, debug:=False)
    End Function
End Module
