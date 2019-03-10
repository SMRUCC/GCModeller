﻿#Region "Microsoft.VisualBasic::f5906f81ceb2e3ceede689c8587a2ca4, visualize\Circos\Circos\CircosAPI.vb"

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

' Module CircosAPI
' 
'     Function: __createGenomeCircle, (+2 Overloads) __geneHighlights, __includesRemoveCommon, AddGenbankData, AddGeneInfoTrack
'               (+2 Overloads) AddGradientMappings, AddMotifSites, AddPlotTrack, AddScoredMotifs, AddSites
'               CircosOption, CreateDataModel, CreateGCContent, CreateGCSkewPlots, CreateGenomeCircle
'               DrawingImageAddLegend, GenerateBlastnAlignment, GenerateGeneCircle, GetCircosScript, GetGenomeCircle
'               GetIdeogram, (+2 Overloads) IdentityColors, PlotsSeperatorLine, PTT2Dump, RemoveIdeogram
'               RemoveStroke, RemoveTicks, RNAVisualize, (+3 Overloads) SetBasicProperty, (+2 Overloads) SetIdeogramRadius
'               SetIdeogramWidth, SetPlotElementPosition, (+2 Overloads) SetRadius, SetTrackFillColor, SetTrackOrientation
'               Shell, SitesFrequency, SkeletonFromDoor, VariantsHighlights, VariationAsDump
'               WriteData
' 
'     Sub: __addDisplayName, __STDOUT_Threads, setProperty, ShowTicksLabel
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.FileIO.Path
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal.Utility
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.Karyotype.GeneObjects
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

''' <summary>
''' Shoal shell interaction with circos perl program to draw a circle diagram of a bacteria genome.
''' </summary>
<Package("Circos", Description:="Shoal shell interaction with circos perl program to draw a circle diagram of a bacteria genome.
<br />
```
                                    ____ _
                                   / ___(_)_ __ ___ ___  ___
                                  | |   | | '__/ __/ _ \/ __|
                                  | |___| | | | (_| (_) \__ \
                                   \____|_|_|  \___\___/|___/

                                                round is good

circos - generate circularly composited information graphics
```
",
                    Cites:="Krzywinski, M., et al. (2009). ""Circos: an information aesthetic For comparative genomics."" Genome Res 19(9): 1639-1645.
<p> We created a visualization tool called Circos to facilitate the identification and analysis of similarities and differences arising from comparisons of genomes. Our tool is effective in displaying variation in genome structure and, generally, any other kind of positional relationships between genomic intervals. Such data are routinely produced by sequence alignments, hybridization arrays, genome mapping, and genotyping studies. Circos uses a circular ideogram layout to facilitate the display of relationships between pairs of positions by the use of ribbons, which encode the position, size, and orientation of related genomic elements. Circos is capable of displaying data as scatter, line, and histogram plots, heat maps, tiles, connectors, and text. Bitmap or vector images can be created from GFF-style data inputs and hierarchical configuration files, which can be easily generated by automated tools, making Circos suitable for rapid deployment in data analysis and reporting pipelines.

", Publisher:="martink@bcgsc.ca", Url:="http://mkweb.bcgsc.ca/circos", Category:=APICategories.ResearchTools)>
<Cite(Title:="Circos: an information aesthetic for comparative genomics", Pages:="1639-45", Issue:="9",
      Keywords:="Animals
Chromosome Mapping
Chromosomes, Artificial, Bacterial
Chromosomes, Human, Pair 17/genetics
Chromosomes, Human, Pair 6/genetics
Contig Mapping
Dogs
Gene Dosage/*genetics
Genome/*genetics
*Genomics
Humans
Lymphoma, Follicular/*genetics
*Software",
      Authors:="Krzywinski, M.
Schein, J.
Birol, I.
Connors, J.
Gascoyne, R.
Horsman, D.
Jones, S. J.
Marra, M. A.",
      DOI:="10.1101/gr.092759.109",
      Journal:="Genome Res",
      ISSN:="1549-5469 (Electronic)
1088-9051 (Linking)",
      Abstract:="We created a visualization tool called Circos to facilitate the identification and analysis of similarities and differences arising from comparisons of genomes.
Our tool is effective in displaying variation in genome structure and, generally, any other kind of positional relationships between genomic intervals.
Such data are routinely produced by sequence alignments, hybridization arrays, genome mapping, and genotyping studies. Circos uses a circular ideogram layout to facilitate the display of relationships between pairs of positions by the use of ribbons, which encode the position, size, and orientation of related genomic elements.
Circos is capable of displaying data as scatter, line, and histogram plots, heat maps, tiles, connectors, and text.
Bitmap or vector images can be created from GFF-style data inputs and hierarchical configuration files, which can be easily generated by automated tools, making Circos suitable for rapid deployment in data analysis and reporting pipelines.",
      AuthorAddress:="Canada's Michael Smith Genome Sciences Center, Vancouver, British Columbia V5Z 4S6, Canada. martink@bcgsc.ca",
      PubMed:=19541911,
      Volume:=19, Year:=2009,
      Notes:=
"                                    ____ _
                                   / ___(_)_ __ ___ ___  ___
                                  | |   | | '__/ __/ _ \/ __|
                                  | |___| | | | (_| (_) \__ \
                                   \____|_|_|  \___\___/|___/

                                                round is good

circos - generate circularly composited information graphics")>
Public Module CircosAPI



    <ExportAPI("IdentityColors")>
    Public Function IdentityColors([default] As String) As IdentityColors
        Return New IdentityLevels([default])
    End Function

    <ExportAPI("IdentityColors")>
    Public Function IdentityColors(min#, max#, Optional depth% = 10, Optional default$ = "Brown", Optional mapName$ = "Jet") As IdentityColors
        Return New IdentityGradients(min, max, depth, [default], mapName)
    End Function

    ''' <summary>
    ''' Invoke set the ideogram width in the circos plot drawing, if the width value is set to ZERO,
    ''' then the ideogram circle will be empty on the drawing but this is different with the ideogram
    ''' configuration document was not included in the circos main configuration.
    ''' </summary>
    ''' <param name="idg"></param>
    ''' <param name="width"></param>
    ''' <returns></returns>
    <ExportAPI("Set.Ideogram.Width", Info:="Invoke set the ideogram width in the circos plot drawing,
if the width value is set to ZERO, then the ideogram circle will be empty on the drawing but this is
different with the ideogram configuration document was not included in the circos main configuration.")>
    Public Function SetIdeogramWidth(idg As Ideogram, width As Integer) As Ideogram
        idg.Ideogram.thickness = width & "p"

        If width = 0 Then
            idg.Ideogram.stroke_thickness = "0"
            idg.Ideogram.band_stroke_thickness = "0"
        End If

        Return idg
    End Function

    ''' <summary>
    ''' Invoke set of the property value in the circos document object.
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="name">The property name in the circos document object, case insensitive.</param>
    ''' <param name="value">String value of the circos document object property.</param>
    <ExportAPI("SetValue", Info:="Invoke set of the property value in the circos document object.")>
    Public Sub setProperty(circos As Configurations.Circos,
                           <Parameter("Name", "The property name in the circos document object, case insensitive.")>
                           name As String,
                           <Parameter("value", "String value of the circos document object property.")>
                           value As String)

        Dim values As PropertyInfo() = circos.GetType().GetProperties(PublicProperty)
        Dim writer As PropertyInfo = LinqAPI.DefaultFirst(Of PropertyInfo) <=
 _
            From wp As PropertyInfo
            In values
            Where String.Equals(wp.Name, name, StringComparison.OrdinalIgnoreCase)
            Select wp

        If writer Is Nothing Then
            Return
        End If

        Call writer.SetValue(circos, value)
    End Sub

    ''' <summary>
    ''' Invoke set the radius value of the ideogram circle.
    ''' (其他的圆圈都会发生变化，则每一次修改之后都需要重新计算圆圈的位置)
    ''' </summary>
    ''' <param name="ideogram"></param>
    ''' <param name="r"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 圆圈的最大值只能够到达1.2了？？？
    ''' 相对的大小是和ideogram有关的
    ''' 1/ideogram.radius
    ''' </remarks>
    <ExportAPI("Set.Ideogram.Radius", Info:="Invoke set the radius value of the ideogram circle.")>
    Public Function SetIdeogramRadius(ideogram As Ideogram, r As Double) As Ideogram
        Dim PreviousRadius As Double = Val(ideogram.Ideogram.radius)

        ideogram.Ideogram.radius = r & "r"

        Dim IR As Double = r
        Dim Max As Double = (1 / r) * 0.825
        Dim getRadius = Function(rd As String) As String
                            r = Val(rd)
                            Dim f As Double = r / PreviousRadius
                            r = f * Max + IR
                            Return $"{r}r"
                        End Function

        For Each track As ITrackPlot In ideogram.main.Plots
            track.r0 = getRadius(track.r0)
            track.r1 = getRadius(track.r1)
        Next

        Return ideogram
    End Function

    ''' <summary>
    ''' Invoke set the radius value of the ideogram circle.
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="r"></param>
    ''' <returns></returns>
    <ExportAPI("Set.Ideogram.Radius", Info:="Invoke set the radius value of the ideogram circle.")>
    <Extension>
    Public Function SetIdeogramRadius(circos As Configurations.Circos, r As Double) As Configurations.Circos
        Dim idg As Ideogram = circos.GetIdeogram
        Call SetIdeogramRadius(idg, r)
        Return circos
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="r">从外圈到内圈的</param>
    ''' <returns></returns>
    '''
    <ExportAPI("Set.Radius")>
    <Extension>
    Public Function SetRadius(circos As Configurations.Circos, r As IEnumerable(Of Double())) As Configurations.Circos
        Dim idx As Integer

        For Each plot As ITrackPlot In circos.Plots
            Dim rD As Double() = r(idx)
            Dim r1 = rD(0)
            Dim r2 = rD(1)

            With plot
                .r1 = CStr(r1) & "r"
                .r0 = CStr(r2) & "r"
            End With

            idx += 1
        Next

        Return circos
    End Function

    <ExportAPI("Set.Radius")>
    <Extension>
    Public Function SetRadius(circos As Configurations.Circos, rMax#, rMin#) As Configurations.Circos
        Dim d = (rMax - rMin) / (circos.NumberOfTracks + 1)
        Dim dd = d * 0.2
        Dim r As Double = rMax - dd

        For Each plot As ITrackPlot In circos.Plots
            Dim r1 = r
            Dim r2 = r1 - d

            plot.r1 = $"{r1}r"
            plot.r0 = $"{r2}r"

            r = r2 - dd
        Next

        Return circos
    End Function

    ''' <summary>
    ''' Gets the ideogram configuration node in the circos document object.
    ''' (还没有ideogram文档的时候，则会返回一个新的文档)
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Get.Circos.Ideogram",
               Info:="Gets the ideogram configuration node in the circos document object.")>
    <Extension> Public Function GetIdeogram(doc As Configurations.Circos) As Ideogram
        Dim LQuery As Ideogram =
            LinqAPI.DefaultFirst(Of Ideogram) <=
                From node As CircosConfig
                In doc.Includes
                Where TypeOf node Is Ideogram
                Select DirectCast(node, Ideogram)

        If Not LQuery Is Nothing Then
            Return LQuery
        Else
            Return New Ideogram(doc)
        End If
    End Function

    ''' <summary>
    ''' The blast result alignment will be mapping on the circos plot circle individual as the 
    ''' highlights element in the circos plot.
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <param name="table">
    ''' The ncbi blast alignment result table object which can be achive from the NCBI website.
    ''' </param>
    ''' <param name="r1">The max radius of the alignment circles.</param>
    ''' <param name="rInner"></param>
    ''' <returns></returns>
    <ExportAPI("Plots.Add.Blast_alignment", Info:="The blast result alignment will be mapping on the circos plot circle individual as the highlights element in the circos plot.")>
    Public Function GenerateBlastnAlignment(doc As Configurations.Circos,
                                            <Parameter("Table", "The ncbi blast alignment result table object which can be achive from the NCBI website.")>
                                            table As AlignmentTable,
                                            <Parameter("r.Max", "The max radius of the alignment circles.")>
                                            r1 As Double,
                                            <Parameter("r.Inner")>
                                            rInner As Double,
                                            Optional Color As IdentityColors = Nothing) As Configurations.Circos

        Dim alignment = (From hit As HitRecord
                         In table.Hits
                         Select hit
                         Group By hit.SubjectIDs Into Group).ToArray
        Dim d As Double = Math.Abs(r1 - rInner) / alignment.Length
        Dim Colors As String() =
            CircosColor.AllCircosColors.Shuffles
        Dim i As Integer = 0

        For Each genome In alignment
            Dim Document As New BlastMaps(genome.Group.ToArray, Colors(i), Color)
            Dim PlotElement As New HighLight(Document)

            Call doc.AddTrack(PlotElement)

            PlotElement.r1 = $"{r1}r"
            r1 -= d
            PlotElement.r0 = $"{r1}r"
            i += 2
        Next

        Return doc
    End Function

    ''' <summary>
    ''' Creates a new seperator object in the circos plot with the specific width of the line, default is ZERO, not display.
    ''' </summary>
    ''' <param name="Length"></param>
    ''' <param name="width"></param>
    ''' <returns></returns>
    <ExportAPI("Plots.New.Seperator",
               Info:="Creates a new seperator object in the circos plot with the specific width of the line, default is ZERO, not display.")>
    Public Function PlotsSeperatorLine(Length As Integer, Optional width As Integer = 0) As Nodes.Plots.SeperatorCircle
        Return New SeperatorCircle(Length, width)
    End Function

    ''' <summary>
    ''' <see cref="yes"/>, <see cref="no"/>
    ''' </summary>
    ''' <param name="b"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CircosOption(b As Boolean) As String
        Return If(b, yes, no)
    End Function

    ''' <summary>
    ''' Mapping details:
    ''' 
    ''' ```
    ''' <see cref="IMotifSite.Type"/> -> <see cref="Color"/>
    ''' <see cref="IMotifSite.Name"/> -> display title label
    ''' ```
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="motifs"></param>
    ''' <param name="snuggle_refine"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AddMotifSites(circos As Configurations.Circos,
                                  motifs As IEnumerable(Of IMotifSite),
                                  Optional snuggle_refine As Boolean = True) As Configurations.Circos

        Dim sites As IMotifSite() = motifs.ToArray
        Dim motifTrack As New MotifSites(sites)
        Dim highlightLabel As New HighlightLabel(
            (From gene As IMotifSite
             In sites
             Where Not String.IsNullOrEmpty(gene.Name)
             Select gene).ToArray)

        Dim snuggle_refine_option As String =
            If(snuggle_refine, yes, no)

        circos += New TextLabel(New HighlightLabel(highlightLabel)) With {
            .snuggle_refine = snuggle_refine_option,
            .label_snuggle = .snuggle_refine
        }
        circos += New HighLight(motifTrack)

        Return circos
    End Function

    <Extension>
    Public Function AddScoredMotifs(circos As Configurations.Circos,
                                    motifs As IEnumerable(Of IMotifScoredSite),
                                    Optional levels% = 100,
                                    Optional mapName$ = ColorMap.PatternJet,
                                    Optional snuggle_refine As Boolean = True) As Configurations.Circos

        Dim sites As IMotifScoredSite() = motifs.ToArray
        Dim motifTrack As New MotifSites(sites, levels, mapName)
        Dim highlightLabel As New HighlightLabel(
            (From gene As IMotifSite
             In sites
             Where Not String.IsNullOrEmpty(gene.Name)
             Select gene).ToArray)

        circos += New TextLabel(New HighlightLabel(highlightLabel)) With {
            .snuggle_refine = snuggle_refine.CircosOption,
            .label_snuggle = .snuggle_refine
        }
        circos += New HighLight(motifTrack)

        Return circos
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <param name="anno"></param>
    ''' <param name="IDRegex">
    ''' Regular expression for parsing the number value in the gene's locus_tag.
    ''' (基因的名称的正则表达式解析字符串。如果为空字符串，则默认输出全部的名称)
    ''' </param>
    ''' <param name="onlyGeneName">当本参数为真的时候，<paramref name=" IDRegex "></paramref>参数失效</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Plots.add.Gene_Circle")>
    <Extension>
    Public Function GenerateGeneCircle(doc As Configurations.Circos,
                                       anno As IEnumerable(Of GeneDumpInfo),
                                       <Parameter("Gene.Name.Only")> Optional onlyGeneName As Boolean = True,
                                       <Parameter("ID.Regex", "Regular expression for parsing the number value in the gene's locus_tag")>
                                       Optional IDRegex As String = "",
                                       Optional DisplayName As Boolean = True,
                                       <Parameter("Snuggle.Refine?", "Enable the circos program layouts the lable of your gene in the best position? Please notices that,
                                       this option is set to False as default, if your genome have more than thousands number of gene to plots,
                                       then we recommends that not enable this option as the drawing plot will be easily go into a deadloop situation.")>
                                       Optional snuggleRefine As Boolean = False,
                                       Optional splitOverlaps As Boolean = False) As Configurations.Circos

        Dim COGVector$() = LinqAPI.Exec(Of String) <=
 _
            From gene As GeneDumpInfo
            In anno
            Select gene.COG
            Distinct

        Dim Colors As Dictionary(Of String, String) =
            CircosColor.ColorProfiles(COGVector)

        Call Colors.Remove("CDS")
        Call Colors.Add("CDS", "rdylbu-6-div-1")

        If DisplayName Then Call __addDisplayName(onlyGeneName, IDRegex, anno, doc, snuggleRefine)

        Dim highlightsTrack As HighLight() =
            __geneHighlights(anno, Colors, Strands.Forward, splitOverlaps)

        If Not highlightsTrack.IsNullOrEmpty Then
            If highlightsTrack.Length = 1 AndAlso
                Not highlightsTrack.First.Highlights.Count = 0 Then

                Dim htrack As HighLight = highlightsTrack(Scan0)
                htrack.r0 = "0.86r"
                htrack.r1 = "0.90r"

                Call doc.AddTrack(htrack)
            Else
                For Each circle As HighLight In highlightsTrack
                    If circle.Highlights.Count = 0 Then
                        Continue For
                    End If
                    Call doc.AddTrack(circle)
                Next
            End If
        End If

        highlightsTrack = __geneHighlights(anno, Colors, Strands.Reverse, splitOverlaps)

        If Not highlightsTrack.IsNullOrEmpty Then
            If highlightsTrack.Length = 1 AndAlso highlightsTrack.First.Highlights.Count > 0 Then

                Dim hTrack As HighLight = highlightsTrack(Scan0)
                hTrack.r0 = "0.82r"
                hTrack.r1 = "0.86r"
                hTrack.fill_color = "blue"
                hTrack.orientation = "out"

                Call doc.AddTrack(hTrack)
            Else
                For Each circle In highlightsTrack
                    If circle.Highlights.Count = 0 Then
                        Continue For
                    End If
                    Call doc.AddTrack(circle)
                Next
            End If
        End If

        Return doc
    End Function

    <ExportAPI("Plots.add.Gradients")>
    Public Function AddGradientMappings(circos As Configurations.Circos,
                                        values As IEnumerable(Of Double),
                                        Optional mapName As String = "Jet",
                                        Optional winSize As Integer = 32,
                                        Optional replaceBase As Boolean = False,
                                        Optional extTails As Boolean = False) As Configurations.Circos
        Dim node As New GradientMappings(
            values, circos.Size, mapName,
            winSize,
            replaceBase, extTails)
        Dim hTrack As New HighLight(node)
        Call circos.AddTrack(track:=hTrack)
        Return circos
    End Function

    <ExportAPI("Plots.add.Gradients")>
    Public Function AddGradientMappings(circos As Configurations.Circos,
                                        values As IEnumerable(Of ILoci),
                                        Optional mapName$ = "Jet",
                                        Optional winSize% = 32,
                                        Optional replaceBase As Boolean = False,
                                        Optional extTails As Boolean = False) As Configurations.Circos
        Dim node As New GradientMappings(
            values, circos.Size, mapName,
            winSize,
            replaceBase,
            extTails)
        Dim track As New HighLight(node)
        Call circos.AddTrack(track:=track)
        Return circos
    End Function

    <ExportAPI("Plots.add.Gene_Elements")>
    <Extension>
    Public Function AddGenbankData(doc As Configurations.Circos,
                                         GBK As GenBank.GBFF.File,
                                         Optional splitOverlaps As Boolean = False,
                                         Optional dumpAll As Boolean = False) As Configurations.Circos
        Dim dump As GeneDumpInfo() = FeatureDumps(GBK, dumpAll:=dumpAll)
        Return GenerateGeneCircle(
            doc, dump,
            splitOverlaps:=splitOverlaps)
    End Function

    <ExportAPI("Plots.add.genes_track")>
    <Extension>
    Public Function AddGeneInfoTrack(circos As Configurations.Circos,
                                        gbk As GenBank.GBFF.File,
                                       COGs As IEnumerable(Of MyvaCOG),
                     Optional splitOverlaps As Boolean = False,
                     Optional dumpAll As Boolean = False) As Configurations.Circos

        Dim dump As GeneDumpInfo() = FeatureDumps(gbk, dumpAll:=dumpAll)
        Dim hash = (From x As MyvaCOG
                    In COGs
                    Select x
                    Group x By x.QueryName Into Group) _
                        .ToDictionary(Function(x) x.QueryName,
                                      Function(x) x.Group.First)
        For Each x As GeneDumpInfo In dump
            If hash.ContainsKey(x.LocusID) Then
                x.COG = hash(x.LocusID).COG
            End If
        Next

        Return GenerateGeneCircle(circos, dump, splitOverlaps:=splitOverlaps)
    End Function

    <ExportAPI("Plots.add.Sites")>
    Public Function AddSites(circos As Configurations.Circos, sites As IEnumerable(Of Contig)) As Configurations.Circos
        Dim genes As GeneDumpInfo() = LinqAPI.Exec(Of Contig, GeneDumpInfo) _
 _
            (sites) <= Function(site As Contig) New GeneDumpInfo With {
                .Location = site.MappingLocation,
                .LocusID = "",
                .CommonName = "",
                .GeneName = "",
                .ProteinId = ""
        }

        Return circos.GenerateGeneCircle(genes, False)
    End Function

    <ExportAPI("Plots.Variants")>
    Public Function VariantsHighlights(Fasta As FastaFile, Optional index As Integer = Scan0) As NtProps.GCSkew
        Dim var As Double() = Patterns.NTVariations(Fasta, index)
        Dim node As New NtProps.GCSkew(var, 1)
        Return node
    End Function

    <ExportAPI("Variation.As.Dump")>
    Public Function VariationAsDump(var As IEnumerable(Of Double)) As GeneDumpInfo()
        Dim regions As New List(Of (value#, start%, end%))
        Dim pre As Double() = var.ToArray

        For i As Integer = 0 To pre.Length - 1
            Do While pre(i) = 0R
                i += 1
                If i > pre.Length - 1 Then
                    Exit Do
                End If
            Loop
            Dim start = i
            If i > pre.Length - 1 Then
                GoTo SET_END
            End If
            Do While pre(i) <> 0R
                i += 1
                If i > pre.Length - 1 Then
                    Exit Do
                End If
            Loop
SET_END:    Dim ends = i
            Dim chun As Double() = New Double(ends - start - 1) {}

            Call Array.ConstrainedCopy(pre, start, chun, Scan0, chun.Length)

            Dim aavg As Double

            If chun.IsNullOrEmpty Then
                aavg = pre.ElementAtOrDefault(start - 1, [default]:=0)  ' 一个点的？？？
            Else
                aavg = chun.Average
            End If

            regions += (aavg, start, ends)
        Next

        Dim genesPretend = regions.Select(
            Function(r) New GeneDumpInfo With {
                .Location = New NucleotideLocation(r.start, r.end),
                .GC_Content = r.value
            }).ToArray
        Dim mapsSrc As Integer() = genesPretend.Select(Function(g) g.GC_Content).GenerateMapping(100)

        For i As Integer = 0 To genesPretend.Length - 1
            genesPretend(i).COG = CStr(mapsSrc(i))
        Next

        Return genesPretend
    End Function

    Private Sub __addDisplayName(onlyGeneName As Boolean,
                                 IDRegex As String,
                                 ByRef anno As IEnumerable(Of GeneDumpInfo),
                                 ByRef doc As Configurations.Circos,
                                 snuggleRefine As Boolean)

        Dim setValue = New SetValue(Of GeneDumpInfo) <= NameOf(GeneDumpInfo.LocusID)

        If Not onlyGeneName Then
            Dim getID As Func(Of String, String) = If(
                Not String.IsNullOrEmpty(IDRegex),
                Function(ID As String) Regex.Match(ID, IDRegex).Value,
                Function(ID As String) ID.Split("_"c).Last)

            anno = LinqAPI.Exec(Of GeneDumpInfo) <= From gene As GeneDumpInfo
                                                    In anno
                                                    Let uid As String = If(
                                                        String.IsNullOrEmpty(gene.GeneName),
                                                        getID(gene.LocusID),
                                                        gene.GeneName)
                                                    Select setValue(gene, uid)
        Else  ' 仅仅显示基因名称
            anno = LinqAPI.Exec(Of GeneDumpInfo) <=
                From gene As GeneDumpInfo
                In anno
                Select setValue(gene, gene.GeneName)
        End If

        Dim LabelGenes As GeneDumpInfo() = LinqAPI.Exec(Of GeneDumpInfo) <=
 _
            From gene As GeneDumpInfo
            In anno
            Where Not String.IsNullOrEmpty(gene.LocusID)
            Select gene

        If LabelGenes.IsNullOrEmpty Then
            Return
        End If

        Dim labels As New TextLabel(New HighlightLabel(LabelGenes)) With {
            .r0 = "0.90r",
            .r1 = "0.995r"
        }

        Call doc.AddTrack(labels)

        labels.snuggle_refine = If(snuggleRefine, yes, no)
    End Sub

    ''' <summary>
    ''' 生成基因组的基因片段，因为有一个<paramref name="SplitOverlaps"/>选项，
    ''' 所以可能会将track圈分解成若干个互不重叠的圈返回，所以返回的类型是一个
    ''' 基因组位点高亮模型的数组
    ''' </summary>
    ''' <param name="anno"></param>
    ''' <param name="colors"></param>
    ''' <param name="strands"></param>
    ''' <param name="splitOverlaps">假若检测到基因有重叠的情况，是否分开为多个圆圈显示？</param>
    ''' <returns></returns>
    Private Function __geneHighlights(anno As IEnumerable(Of GeneDumpInfo),
                                      colors As Dictionary(Of String, String),
                                      strands As Strands,
                                      splitOverlaps As Boolean) As HighLight()
        If Not splitOverlaps Then
            Return {
                __geneHighlights(anno, colors, strands)
            }
        End If

        Dim list As List(Of GeneDumpInfo)

        If strands <> Strands.Unknown Then
            list = LinqAPI.MakeList(Of GeneDumpInfo) <=
                From gene As GeneDumpInfo
                In anno
                Where gene.Location.Strand = strands
                Select gene
        Else
            list = anno.AsList
        End If

        Dim circles As New List(Of HighLight)

        Do While Not list.IsNullOrEmpty
            Dim genes As New List(Of GeneDumpInfo)

            For Each gene As GeneDumpInfo In list.ToArray
                Dim lquery = (From gg In list
                              Let r = gene.Location.GetRelationship(gg.Location)
                              Where Not gg.Equals(gene) AndAlso (
                                  r = SegmentRelationships.Cover OrElse
                                  r = SegmentRelationships.Equals OrElse
                                  r = SegmentRelationships.InnerAntiSense OrElse
                                  r = SegmentRelationships.Inside)
                              Select gg,
                                  r).FirstOrDefault

                If lquery Is Nothing Then
                    Call genes.Add(gene)     ' 没有重叠，则进行添加
                    Call list.Remove(gene)
                Else
                    ' 跳过这个基因，留到下一个圆圈之上
                End If
            Next

            circles += New HighLight(New GeneMark(genes, colors))
        Loop

        Return circles.ToArray
    End Function


    ''' <summary>
    ''' 生成基因组的基因片段
    ''' </summary>
    ''' <param name="anno"></param>
    ''' <param name="colors"></param>
    ''' <param name="strands"></param>
    ''' <returns></returns>
    Private Function __geneHighlights(anno As IEnumerable(Of GeneDumpInfo),
                                      colors As Dictionary(Of String, String),
                                      strands As Strands) As HighLight
        Dim genes As GeneDumpInfo()

        If strands <> Strands.Unknown Then
            genes = LinqAPI.Exec(Of GeneDumpInfo) <=
 _
                From gene As GeneDumpInfo
                In anno
                Where gene.Location.Strand = strands
                Select gene
        Else
            genes = anno.ToArray
        End If

        Dim track As New HighLight(New GeneMark(genes, colors))
        Return track
    End Function


    ''' <summary>
    ''' 使用Highlighs来显示RNA分子在基因组之上的位置
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("RNA.Visualize")>
    <Extension>
    Public Function RNAVisualize(doc As Configurations.Circos, anno As PTT) As Configurations.Circos
        Dim COGVector As String() = anno.GeneObjects _
            .Select(Function(g) g.Product) _
            .Distinct _
            .ToArray ' RNA的数目很少，所以这里直接使用产物来替代COG来计算颜色了
        Dim Colors = CircosColor.ColorProfiles(COGVector)
        Dim setValue = New SetValue(Of GeneDumpInfo) <= NameOf(GeneDumpInfo.LocusID)
        Dim genes As GeneDumpInfo() = LinqAPI.Exec(Of GeneDumpInfo) <=
 _
            From gene As GeneDumpInfo
            In anno.ExportPTTAsDump
            Select setValue(gene, gene.Function)

        Dim highlightLabel As New HighlightLabel(
            (From gene As GeneDumpInfo
             In genes
             Where Not String.IsNullOrEmpty(gene.LocusID)
             Select gene).ToArray)
        Dim labels As New TextLabel(highlightLabel) With {
            .r0 = "0.8r",
            .r1 = "0.85r"
        }

        Call doc.AddTrack(labels)

        Dim highlights = __geneHighlights(genes, Colors, Strands.Unknown)
        highlights.r0 = "0.75r"
        highlights.r1 = "0.78r"

        Call doc.AddTrack(highlights)

        Return doc
    End Function

    ''' <summary>
    ''' Adds the GC% content on the circos plots.
    ''' </summary>
    ''' <param name="nt">
    ''' The original nt sequence in the fasta format for the calculation of the GC% content in each slidewindow
    ''' </param>
    ''' <param name="winSize%"></param>
    ''' <param name="steps%"></param>
    ''' <returns></returns>
    <ExportAPI("Plots.GC%", Info:="Adds the GC% content on the circos plots.")>
    Public Function CreateGCContent(<Parameter("NT.Fasta",
                                               "The original nt sequence in the fasta format for the calculation of the GC% content in each slidewindow")>
                                    nt As FastaSeq, winSize%, steps%) As NtProps.GenomeGCContent
        Return New NtProps.GenomeGCContent(nt, winSize, steps)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="track"></param>
    ''' <param name="rOutside">The radius value of the outside for this circle element.</param>
    ''' <param name="rInner">The radius value of the inner circle of this element.</param>
    ''' <returns></returns>
    <ExportAPI("Plots.Element.Set.Position")>
    Public Function SetPlotElementPosition(track As ITrackPlot,
                                           <Parameter("r.Outside", "The radius value of the outside for this circle element.")>
                                           rOutside As String,
                                           <Parameter("r.Inner", "The radius value of the inner circle of this element.")>
                                           rInner As String) As ITrackPlot
        track.r1 = rOutside
        track.r0 = rInner
        Return track
    End Function

    ''' <summary>
    ''' Invoke set the color of the circle element on the circos plots.
    ''' </summary>
    ''' <param name="track"></param>
    ''' <param name="Color">The name of the color in the circos program.</param>
    ''' <returns></returns>
    <ExportAPI("Plots.Element.Set.Fill_Color", Info:="Invoke set the color of the circle element on the circos plots.")>
    Public Function SetTrackFillColor(track As ITrackPlot,
                                      <Parameter("Color", "The name of the color in the circos program.")>
                                      Color As String) As ITrackPlot
        track.fill_color = Color
        Return track
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="track"></param>
    ''' <param name="orientation">ori = ""in"" or ""out""</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("Plots.Element.Set.Orientation", Info:="ori = ""in"" or ""out""")>
    Public Function SetTrackOrientation(track As ITrackPlot, orientation$) As ITrackPlot
        track.orientation = orientation
        Return track
    End Function

    '''' <summary>
    '''' Door之中的操纵子以heatmap的形式绘制
    '''' </summary>
    '''' <param name="DOOR">Door文件的文件路径</param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    '<ExportAPI("Plots.DOOR")>
    'Public Function DoorOperon(<Parameter("DOOR", "The file path of the DOOR database file.")> DOOR As String) As ITrackPlot
    '    Dim Data As New Karyotype.DoorOperon(DOOR)
    '    Dim HeatMap = New Nodes.Plots.HeatMap(Data)
    '    HeatMap.r0 = "1.3r"
    '    HeatMap.r1 = "1.1r"

    '    Return HeatMap
    'End Function

    ''' <summary>
    ''' The directory which contains the completed PTT data: ``*.ptt, *.rnt, *.fna``
    ''' and so on which you can download from the NCBI FTP website.
    ''' </summary>
    ''' <param name="PTT">
    ''' The directory which contains the completed PTT data: *.ptt, *.rnt, *.fna and so on which you can download from the NCBI FTP website.
    ''' </param>
    ''' <param name="myvaCog">
    ''' The csv file path of the myva cog value which was export from the alignment between
    ''' the bacteria genome And the myva cog database Using the NCBI blast package In the GCModeller.
    ''' </param>
    ''' <param name="defaultColor">
    ''' The default color of the gene which is not assigned to any COG will be have.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("Plots.Genome_Circle")>
    Public Function GetGenomeCircle(<Parameter("Dir.PTT",
                                               "The directory which contains the completed PTT data: *.ptt, *.rnt, *.fna and so on which you can download from the NCBI FTP website.")>
                                    PTT As String,
                                    <Parameter("COG.Myva", "The csv file path of the myva cog value which was export from the alignment between
                                    the bacteria genome and the myva cog database using the NCBI blast package in the GCModeller.")>
                                    Optional myvaCog As String = "",
                                    <Parameter("Color.Default", "The default color of the gene which is not assigned to any COG will be have.")>
                                    Optional defaultColor As String = "blue") As PTTMarks

        Dim pttDB As New PTTDbLoader(PTT)

        If String.IsNullOrEmpty(myvaCog) OrElse Not FileIO.FileSystem.FileExists(myvaCog) Then
            Return New PTTMarks(pttDB, Nothing, defaultColor)
        Else
            Dim Myva = myvaCog.LoadCsv(Of MyvaCOG)(False).ToArray
            Return __createGenomeCircle(pttDB, Myva, defaultColor)
        End If
    End Function

    ''' <summary>
    ''' Creates the circos outside gene circle from the export csv data of the genbank database file.
    ''' </summary>
    ''' <param name="anno"></param>
    ''' <param name="genome"></param>
    ''' <param name="defaultColor"></param>
    ''' <returns></returns>
    <ExportAPI("Plots.Genome_Circle.From.GenbankDump",
               Info:="Creates the circos outside gene circle from the export csv data of the genbank database file.")>
    Public Function CreateGenomeCircle(anno As IEnumerable(Of GeneDumpInfo), genome As FastaSeq, Optional defaultColor As String = "blue") As PTTMarks
        Dim track As New PTTMarks(anno.ToArray, genome, defaultColor)
        Return track
    End Function

    ''' <summary>
    ''' Creates the circos gene circle from the PTT database which is defined 
    ''' in the ``*.ptt/*.rnt`` file, and you can download this directory from 
    ''' the NCBI FTP website.
    ''' </summary>
    ''' <param name="PTT"></param>
    ''' <param name="COG"></param>
    ''' <param name="defaultColor"></param>
    ''' <returns></returns>
    <ExportAPI("Plots.Genome_Circle.From.Objects",
               Info:="Creates the circos gene circle from the PTT database which is defined in the *.ptt/*.rnt file, and you can download this directory from the NCBI FTP website.")>
    Public Function __createGenomeCircle(PTT As PTTDbLoader, COG As IEnumerable(Of MyvaCOG), Optional defaultColor As String = "blue") As PTTMarks
        Dim Data As New PTTMarks(PTT, COG.ToArray, defaultColor)
        Return Data
    End Function

    ''' <summary>
    ''' Creates the circos circle plots of the genome gcskew.
    ''' </summary>
    ''' <param name="SequenceModel"></param>
    ''' <param name="SlideWindowSize"></param>
    ''' <param name="steps"></param>
    ''' <returns></returns>
    <ExportAPI("Karyotype.doc.gcSkew", Info:="Creates the circos circle plots of the genome gcskew.")>
    Public Function CreateGCSkewPlots(SequenceModel As IPolymerSequenceModel,
                                      <Parameter("SlideWindow.Size")> SlideWindowSize As Integer,
                                      steps As Integer) As NtProps.GCSkew
        Return New NtProps.GCSkew(SequenceModel, SlideWindowSize, steps, True)
    End Function

    '<ExportAPI("Karyotype.As.Heatmap")>
    'Public Function KaryotypeAsHeatmap(doc As TrackDataDocument) As Nodes.Plots.HeatMap
    '    Return New Nodes.Plots.HeatMap(doc)
    'End Function

    '<ExportAPI("Karyotype.As.Histogram")>
    'Public Function KaryotypeAsHistogram(doc As TrackDataDocument) As Nodes.Plots.Histogram
    '    Return New Nodes.Plots.Histogram(doc)
    'End Function

    '<ExportAPI("Karyotype.As.Line")>
    'Public Function KaryotypeAsLine(doc As ITrackPlot) As Lines.Line
    '    Return New Lines.Line(doc)
    'End Function

    ''' <summary>
    ''' Adds a new circos plots element into the circos.conf object.
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="track"></param>
    ''' <returns></returns>
    <ExportAPI("Adds.Plots",
               Info:="Adds a new circos plots element into the circos.conf object.")>
    Public Function AddPlotTrack(ByRef circos As Configurations.Circos, track As ITrackPlot) As Integer
        Call circos.AddTrack(track)
        Return circos.Plots.Length
    End Function

    <ExportAPI("Set.Property.Basic")>
    Public Function SetBasicProperty(doc As Configurations.Circos, data As PTTMarks) As Boolean
        doc.SkeletonKaryotype = data

        Call doc.Includes.Add(New Ticks(Circos:=doc))
        Call doc.Includes.Add(New Ideogram(doc))

        Return True
    End Function

    <ExportAPI("Set.Property.Basic")>
    Public Function SetBasicProperty(doc As Configurations.Circos, nt As FASTA.FastaSeq, Optional loophole As Integer = 0) As Boolean
        Return SetBasicProperty(doc, nt, Nothing, loophole)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="NT"></param>
    ''' <param name="bands"></param>
    ''' <param name="loopHole">默认为0，没有缺口</param>
    ''' <returns></returns>
    <ExportAPI("Skeleton.With.Bands")>
    Public Function SetBasicProperty(circos As Configurations.Circos,
                                     NT As FastaSeq,
                                     bands As IEnumerable(Of NamedTuple(Of String)),
                                     Optional loopHole As Integer = 0) As Boolean
        Call circos.Includes.Add(New Ticks(Circos:=circos))
        Call circos.Includes.Add(New Ideogram(circos))

        circos.SkeletonKaryotype = New KaryotypeChromosomes(
            Len(NT.SequenceData) + loopHole,
            "white",
            bands.SafeQuery.ToArray)
        circos.SkeletonKaryotype.LoopHole.value = loopHole
        circos.karyotype = "./data/genome_skeleton.txt"

        Return True
    End Function

    <ExportAPI("Sites.Frequency")>
    Public Function SitesFrequency(locis As IEnumerable(Of ILoci), length As Integer) As Double()
        Dim d = (From site As ILoci
                 In locis
                 Select site
                 Group site By site.Left Into Group).ToDictionary(
                    Function(site) site.Left,
                    Function(site) CDbl(site.Group.ToArray.Length))

        VBDebugger.Mute = True

        Dim values = length.Sequence.Select(Function(idx) d.TryGetValue(idx, [default]:=0))

        VBDebugger.Mute = False

        Dim slids = values.CreateSlideWindows(10)
        Dim avgs = slids.Select(Function(win) win.Average).ToArray
        Return avgs
    End Function

    <ExportAPI("Skeleton.From.Door", Info:="Creates the basic Karyotype document for the circos plot.")>
    Public Function SkeletonFromDoor(doc As Configurations.Circos,
                                     NT As FastaSeq,
                                     <Parameter("Door.File", "The file path of the door operon prediction data.")> DOOR As String,
                                     Optional loophole As Integer = 0) As Boolean
        Dim LQuery = (From Operon As Operon In DOOR_API.Load(DOOR)
                      Let Loci = (From obj In Operon Select {obj.Value.Location.Left, obj.Value.Location.Right}).Unlist
                      Let COG As String = New String((From c In (From obj As KeyValuePair(Of String, OperonGene)
                                                                 In Operon
                                                                 Select obj.Value.COG_number.GetCOGCategory.ToArray).Unlist
                                                      Select c
                                                      Distinct
                                                      Order By c Ascending).ToArray)
                      Select COG,
                          band = New NamedTuple(Of String)(CStr(Loci.Min), CStr(Loci.Max))).ToArray
        Dim Color As Dictionary(Of String, String) =
            CircosColor.ColorProfiles((From obj In LQuery
                                       Select obj.COG
                                       Distinct).ToArray)
        Dim setValue = New SetValue(Of NamedTuple(Of String)) <= NameOf(NamedTuple(Of String).Name)
        Dim BandsData = LinqAPI.Exec(Of NamedTuple(Of String)) _
 _
            () <= From obj
                  In LQuery
                  Select setValue(obj.band, Color(obj.COG))

        Return SetBasicProperty(doc, NT, BandsData, loophole)
    End Function

    ''' <summary>
    ''' Creats a new <see cref="Configurations.Circos"/> plots configuration document.
    ''' </summary>
    ''' <returns><see cref="Configurations.Circos.CreateObject"/></returns>
    <ExportAPI("Circos.Document.Create", Info:="Creats a new circos plots configuration document.")>
    Public Function CreateDataModel() As Configurations.Circos
        Return Configurations.Circos.CreateObject
    End Function

    ''' <summary>
    ''' Save the circos plots configuration object as the default configuration file: circos.conf
    ''' </summary>
    ''' <param name="circos"></param>
    ''' <param name="outDIR"></param>
    ''' <param name="debug"></param>
    ''' <returns></returns>
    <ExportAPI("Write.Txt.Circos", Info:="Save the circos plots configuration object as the default configuration file: circos.conf")>
    <Extension>
    Public Function WriteData(circos As Configurations.Circos,
                              Optional outDIR$ = "",
                              Optional debug As DebugGroups = DebugGroups.NULL) As String

        Dim perlRun$ = GetCircosScript().CLIPath.Replace("\", "/")
        Dim conf$ = circos.FilePath.CLIPath.Replace("\", "/")

        Call circos.Save(outDIR)
        Call $"perl {perlRun} -conf {conf}{debug.GetOptions}".SaveTo(outDIR & "/run.bat")
        Call ("#! /bin/bash" & vbCrLf &
             $"perl {perlRun} -conf {conf}{debug.GetOptions}").SaveTo(outDIR & "/run.sh")

        Return circos.FilePath
    End Function

    ''' <summary>
    ''' Gets the circos Perl script file location automatically by search on the file system.
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("Circos.pl", Info:="Gets the circos Perl script file location automatically by search on the file system.")>
    Public Function GetCircosScript() As String
        Dim libs = ProgramPathSearchTool.SearchDirectory("circos")

        For Each DIR As String In libs
            Dim circos$() = ProgramPathSearchTool.SearchScriptFile(DIR, "circos").ToArray

            If Not circos.IsNullOrEmpty Then
                Return circos.First
            End If
        Next

        Return ""
    End Function

    <ExportAPI("Ticks.ShowLabel")>
    Public Sub ShowTicksLabel(circos As Configurations.Circos, value As Boolean)
        If circos.Includes.IsNullOrEmpty Then
            Return
        End If

        Dim ticks = LinqAPI.DefaultFirst(Of Ticks) <=
 _
            From include As CircosConfig
            In circos.Includes
            Where InStr(include.RefPath, Configurations.Circos.TicksConf, CompareMethod.Text) > 0
            Select DirectCast(include, Ticks)

        If Not ticks Is Nothing Then
            Dim show As String = If(value, yes, no)

            ticks.show_tick_labels = show

            For Each tick As Nodes.Tick In ticks.Ticks.Ticks
                tick.show_label = show
            Next
        End If
    End Sub

    <ExportAPI("Ticks.Remove", Info:="Removes the ticks label from the circos docuemnt node.")>
    Public Function RemoveTicks(doc As Configurations.Circos) As Boolean
        Return __includesRemoveCommon(Configurations.Circos.TicksConf, doc)
    End Function

    <Extension>
    Public Function RemoveStroke(Of Track As ITrackPlot)(t As Track) As Track
        t.thickness = "0p"
        t.stroke_color = t.fill_color
        t.stroke_thickness = "0"

        Return t
    End Function

    Private Function __includesRemoveCommon(conf As String, doc As Configurations.Circos) As Boolean
        If doc.Includes.IsNullOrEmpty Then
            Return True
        End If

        Dim LQuery = (From include In doc.Includes.AsParallel
                      Where InStr(include.RefPath, conf, CompareMethod.Text) > 0
                      Select include).ToArray
        If Not LQuery.IsNullOrEmpty Then
            Call doc.Includes.Remove(LQuery(Scan0))
        End If

        Return True
    End Function

    <ExportAPI("Ideogram.Remove", Info:="Removes the ideogram plots element from the circos document node.")>
    Public Function RemoveIdeogram(doc As Configurations.Circos) As Boolean
        Dim Ideogram = (From include In doc.Includes
                        Where InStr(include.RefPath, Configurations.Ideogram.IdeogramConf, CompareMethod.Text) > 0
                        Select DirectCast(include, Configurations.Ideogram)).FirstOrDefault
        If Ideogram Is Nothing Then
            Call $"Circos configuration file have no ideogram data".__DEBUG_ECHO
        Else
            Ideogram.Ideogram.thickness = "0p"

        End If
        Return True
    End Function

    <ExportAPI("Circos.Draw",
               Info:="Invoke the Perl program to drawing the circos plots. before you can using this method, you should switch the terminal
               work directory to the directory which contains the circos.conf plots configuration file.")>
    Public Function Shell(Optional conf As String = "") As Boolean
        Dim Directories = ProgramPathSearchTool.SearchDirectory("perl", "")
        Dim Perl As String = ""
        Dim Circos As String = GetCircosScript()

        For Each Dir As String In Directories
            Dim Files = ProgramPathSearchTool.SearchProgram(Dir, "perl").ToArray

            If Not Files.IsNullOrEmpty Then
                Perl = Files.First
                Call $"Perl program find at ""{Perl.ToFileURL}""".__DEBUG_ECHO
                Exit For
            End If
        Next

        If String.IsNullOrEmpty(Circos) Then
            Call Console.WriteLine("System could not found the circos script!")
            Return False
        End If
        If String.IsNullOrEmpty(Perl) Then
            Call Console.WriteLine("System could not found the perl location!")
            Return False
        End If

        Call $"Circos script file found at ""{Circos.ToFileURL}""".__DEBUG_ECHO

        If String.IsNullOrEmpty(conf) Then
            conf = "./circos.conf"
        End If

        Call $"Circos drawing configuration script found at ""{conf.ToFileURL}""".__DEBUG_ECHO

        Dim cmdl_argvs As String = String.Format("""{0}"" ""{1}""", Circos, conf)
        Dim Process As Process = New Process
        Process.StartInfo = New ProcessStartInfo(Perl, cmdl_argvs)
        Process.StartInfo.RedirectStandardOutput = True
        Process.StartInfo.CreateNoWindow = True
        Process.StartInfo.UseShellExecute = False

        Call Process.Start()
        Call $" system(""""{Perl}"" {cmdl_argvs}"")".__DEBUG_ECHO

        Dim Reader As System.IO.StreamReader = Process.StandardOutput

        Using PI = New CBusyIndicator(_start:=True)
            Call (Sub() Call __STDOUT_Threads(Reader)).BeginInvoke(Nothing, Nothing)
            Call Process.WaitForExit()
        End Using

        Return True
    End Function

    Private Sub __STDOUT_Threads(Reader As System.IO.StreamReader)
        Do While True

            Dim STD As String = Reader.ReadLine
            If Not String.IsNullOrEmpty(STD) Then
                Call Console.WriteLine(STD)
            End If
            Call Threading.Thread.Sleep(1)
        Loop
    End Sub

    Public Const yes As String = "yes"
    Public Const no As String = "no"
    ''' <summary>
    ''' This property have no data value
    ''' </summary>
    Public Const null As String = ""

    Dim Margin As Integer = 200

    <ExportAPI("Circos.Add.Legends",
               Info:="If the NCBI alignment result plots was includes in your circos plots,
then you can using this method to adding the legends on your circos plots image when you have finish invoke drawing by the circos script program.")>
    Public Function DrawingImageAddLegend(doc As Configurations.Circos) As Image
        Dim ImagePath As String = FileIO.FileSystem.GetParentPath(doc.FilePath) & "/Circos.png"
        Dim CircosImage = Image.FromFile(ImagePath)

        Dim AlignmentData = doc.GetBlastAlignmentData
        Dim Font = New Font(FontFace.Ubuntu, 20)
        Dim sz As SizeF

        If Not AlignmentData.IsNullOrEmpty Then
            sz = AlignmentData.Keys.MaxLengthString.MeasureSize(New Size(1, 1).CreateGDIDevice, Font)
        Else
            sz = New SizeF(1, 20)
        End If

        Dim device = (New SizeF(CircosImage.Width + 3 * Margin + sz.Width * 2, CInt(CircosImage.Height + Margin * 4))).CreateGDIDevice
        Call device.Graphics.DrawImage(CircosImage, New Point(Margin, Margin))

        Dim refPt As Point = New Point(100, 100)

        If Not doc.Plots.IsNullOrEmpty Then

            For Each PlotElement As ITrackPlot In doc.Plots
                '   refPt = PlotElement.KaryotypeDocumentData.LegendsDrawing(refPt, Device)
            Next
        End If

        If Not AlignmentData.IsNullOrEmpty Then

            Font = New Font(FontFace.Ubuntu, 28)
            sz = AlignmentData.Keys.MaxLengthString.MeasureSize(device, Font)

            Dim dh = CInt(sz.Height)
            Dim Y As Integer = Margin * 3
            Dim X As Integer = CInt(device.Width - sz.Width - 2 * Margin)
            Dim ColorBlockSize As New SizeF(200, sz.Height)

            Call device.Graphics.DrawString("Localblast Alignment Order:", Font, Brushes.Black, New Point(X, Y))
            Y += 2 * dh

            For Each ID As NamedValue(Of String) In AlignmentData
                Call device.Graphics.DrawString(ID.Name, Font, Brushes.Black, New Point(X, Y))
                Call device.Graphics.FillRectangle(New SolidBrush(CircosColor.FromKnownColorName(ID.Value)), New RectangleF(New PointF(X - ColorBlockSize.Width - 10, Y), ColorBlockSize))

                Y += dh + 3
            Next
        End If

        Return device.ImageResource
    End Function

    <ExportAPI("PTT2Dump")>
    Public Function PTT2Dump(PTT As PTT) As GeneDumpInfo()
        Return GenBank.ExportPTTAsDump(PTT)
    End Function
End Module
