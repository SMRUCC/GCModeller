#Region "Microsoft.VisualBasic::39c43b50c26d3c8956e9ca2015824fde, visualize\Circos\Circos\CircosAPI.vb"

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
    '     Function: __includesRemoveCommon, AddGenbankData, (+3 Overloads) AddGradientMappings, AddMotifSites, AddPlotTrack
    '               AddScoredMotifs, AddSites, CircosOption, CreateDataModel, CreateGCContent
    '               CreateGCSkewPlots, CreateGenomeCircle, GenerateGeneCircle, GetCircosScript, GetIdeogram
    '               (+2 Overloads) IdentityColors, PlotsSeperatorLine, PTT2Dump, RemoveIdeogram, RemoveStroke
    '               RemoveTicks, RNAVisualize, (+3 Overloads) SetBasicProperty, (+2 Overloads) SetIdeogramRadius, SetIdeogramWidth
    '               SetPlotElementPosition, (+2 Overloads) SetRadius, SetTrackFillColor, SetTrackOrientation, Shell
    '               SitesFrequency, SkeletonFromDoor, VariantsHighlights, VariationAsDump, WriteData
    ' 
    '     Sub: __STDOUT_Threads, setProperty, ShowTicksLabel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.FileIO.Path
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.Karyotype.GeneObjects
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
Imports ColorPatterns = Microsoft.VisualBasic.Imaging.ColorMap

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
        Dim d = (rMax - rMin) / (circos.numberOfTracks + 1)
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
    ''' Mapping details:
    ''' 
    ''' ```
    ''' <see cref="IMotifSite.family"/> -> <see cref="Color"/>
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
             Where Not String.IsNullOrEmpty(gene.name)
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
                                    Optional mapName$ = ColorPatterns.PatternJet,
                                    Optional snuggle_refine As Boolean = True) As Configurations.Circos

        Dim sites As IMotifScoredSite() = motifs.ToArray
        Dim motifTrack As New MotifSites(sites, levels, mapName)
        Dim highlightLabel As New HighlightLabel(
            (From gene As IMotifSite
             In sites
             Where Not String.IsNullOrEmpty(gene.name)
             Select gene).ToArray)

        circos += New TextLabel(New HighlightLabel(highlightLabel)) With {
            .snuggle_refine = snuggle_refine.CircosOption,
            .label_snuggle = .snuggle_refine
        }
        circos += New HighLight(motifTrack)

        Return circos
    End Function

    <ExportAPI("Plots.add.Gradients")>
    Public Function AddGradientMappings(circos As Configurations.Circos,
                                        values As IEnumerable(Of Double),
                                        Optional mapName As String = "Jet",
                                        Optional winSize As Integer = 32,
                                        Optional replaceBase As Boolean = False,
                                        Optional extTails As Boolean = False) As Configurations.Circos
        Dim node As New GradientMappings(
            values, circos.size, mapName,
            winSize,
            replaceBase, extTails)
        Dim hTrack As New HighLight(node)
        Call circos.AddTrack(track:=hTrack)
        Return circos
    End Function

    <ExportAPI("Plots.add.Gradients")>
    Public Function AddGradientMappings(circos As Configurations.Circos, values As IEnumerable(Of ValueTrackData), Optional mapName As String = "Jet") As Configurations.Circos
        Dim node As New GradientMappings(values, circos.skeletonKaryotype, mapName)
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
            values, circos.size, mapName,
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
        Dim dump As GeneTable() = FeatureDumps(GBK, dumpAll:=dumpAll)
        Return GenerateGeneCircle(
            doc, dump,
            splitOverlaps:=splitOverlaps)
    End Function

    <ExportAPI("Plots.add.Sites")>
    Public Function AddSites(circos As Configurations.Circos, sites As IEnumerable(Of Contig)) As Configurations.Circos
        Dim genes As GeneTable() = LinqAPI.Exec(Of Contig, GeneTable) _
 _
            (sites) <= Function(site As Contig) New GeneTable With {
                .Location = site.MappingLocation,
                .locus_id = "",
                .commonName = "",
                .geneName = "",
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
    Public Function VariationAsDump(var As IEnumerable(Of Double)) As GeneTable()
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
            Function(r) New GeneTable With {
                .Location = New NucleotideLocation(r.start, r.end),
                .GC_Content = r.value
            }).ToArray
        Dim mapsSrc As Integer() = genesPretend.Select(Function(g) g.GC_Content).GenerateMapping(100)

        For i As Integer = 0 To genesPretend.Length - 1
            genesPretend(i).COG = CStr(mapsSrc(i))
        Next

        Return genesPretend
    End Function

    ''' <summary>
    ''' 使用Highlighs来显示RNA分子在基因组之上的位置
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("RNA.Visualize")>
    <Extension>
    Public Function RNAVisualize(doc As Configurations.Circos, anno As PTT) As Configurations.Circos
        Return TrackDatas.RNAVisualize(doc, anno)
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
                                           anno As IEnumerable(Of GeneTable),
                                           <Parameter("Gene.Name.Only")> Optional onlyGeneName As Boolean = True,
                                           <Parameter("ID.Regex", "Regular expression for parsing the number value in the gene's locus_tag")>
                                           Optional IDRegex As String = "",
                                           Optional DisplayName As Boolean = True,
                                           <Parameter("Snuggle.Refine?", "Enable the circos program layouts the lable of your gene in the best position? Please notices that,
                                       this option is set to False as default, if your genome have more than thousands number of gene to plots,
                                       then we recommends that not enable this option as the drawing plot will be easily go into a deadloop situation.")>
                                           Optional snuggleRefine As Boolean = False,
                                           Optional splitOverlaps As Boolean = False,
                                           Optional colorProfiles As Dictionary(Of String, String) = Nothing) As Configurations.Circos
        Return TrackDatas.FeatureAnnotations.GenerateGeneCircle(
            doc:=doc, anno:=anno, onlyGeneName:=onlyGeneName, IDregex:=IDRegex,
            displayName:=DisplayName,
            snuggleRefine:=snuggleRefine,
            splitOverlaps:=splitOverlaps,
            colorProfiles:=colorProfiles
        )
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
        track.orientation = DirectCast([Enum].Parse(GetType(orientations), Strings.LCase(orientation)), orientations)
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
    ''' Creates the circos outside gene circle from the export csv data of the genbank database file.
    ''' </summary>
    ''' <param name="anno"></param>
    ''' <param name="genome"></param>
    ''' <param name="defaultColor"></param>
    ''' <returns></returns>
    <ExportAPI("Plots.Genome_Circle.From.GenbankDump",
               Info:="Creates the circos outside gene circle from the export csv data of the genbank database file.")>
    Public Function CreateGenomeCircle(anno As IEnumerable(Of GeneTable), genome As FastaSeq, Optional defaultColor As String = "blue") As PTTMarks
        Dim track As New PTTMarks(anno.ToArray, genome, defaultColor)
        Return track
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
        doc.skeletonKaryotype = data

        Call doc.includes.Add(New Ticks(Circos:=doc))
        Call doc.includes.Add(New Ideogram(doc))

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

        Dim totalSize% = Len(NT.SequenceData) + loopHole

        Call circos.includes.Add(New Ticks(Circos:=circos))
        Call circos.includes.Add(New Ideogram(circos))

        circos.skeletonKaryotype = New KaryotypeChromosomes(totalSize, "white", bands)
        circos.skeletonKaryotype.loopHole = loopHole
        circos.karyotype = "./data/genome_skeleton.txt"

        Return True
    End Function

    <ExportAPI("Sites.Frequency")>
    Public Function SitesFrequency(locis As IEnumerable(Of ILoci), length As Integer) As Double()
        Dim d = (From site As ILoci
                 In locis
                 Select site
                 Group site By site.left Into Group).ToDictionary(
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
                      Let Loci = (From obj In Operon Select {obj.Value.Location.left, obj.Value.Location.right}).Unlist
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
    <Extension>
    Public Function WriteData(circos As Configurations.Circos,
                              Optional outDIR$ = "",
                              Optional debug As DebugGroups = DebugGroups.NULL) As String

        Dim perlRun$ = GetCircosScript().CLIPath.Replace("\", "/")
        Dim conf$ = circos.filePath.CLIPath.Replace("\", "/")

        Call circos.Save(outDIR)
        Call $"perl {perlRun} -conf {conf}{debug.GetOptions}".SaveTo(outDIR & "/run.bat")
        Call ("#! /bin/bash" & vbCrLf &
             $"perl {perlRun} -conf {conf}{debug.GetOptions}").SaveTo(outDIR & "/run.sh")

        Return circos.filePath
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
        If circos.includes.IsNullOrEmpty Then
            Return
        End If

        Dim ticks = LinqAPI.DefaultFirst(Of Ticks) <=
 _
            From include As CircosConfig
            In circos.includes
            Where InStr(include.RefPath, Configurations.Circos.TicksConf, CompareMethod.Text) > 0
            Select DirectCast(include, Ticks)

        If Not ticks Is Nothing Then
            Dim show As String = If(value, yes, no)

            ticks.show_tick_labels = show

            For Each tick As Nodes.Tick In ticks.Ticks.ticks
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
        If doc.includes.IsNullOrEmpty Then
            Return True
        End If

        Dim LQuery = (From include In doc.includes.AsParallel
                      Where InStr(include.RefPath, conf, CompareMethod.Text) > 0
                      Select include).ToArray
        If Not LQuery.IsNullOrEmpty Then
            Call doc.includes.Remove(LQuery(Scan0))
        End If

        Return True
    End Function

    <ExportAPI("Ideogram.Remove", Info:="Removes the ideogram plots element from the circos document node.")>
    Public Function RemoveIdeogram(doc As Configurations.Circos) As Boolean
        Dim Ideogram = (From include In doc.includes
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

        Using PI = New CBusyIndicator(start:=True)
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

    <ExportAPI("PTT2Dump")>
    Public Function PTT2Dump(PTT As PTT) As GeneTable()
        Return GenBank.ExportPTTAsDump(PTT)
    End Function
End Module
