﻿#Region "Microsoft.VisualBasic::00eb6d3b689de3daa843f03097733066, R#\visualkit\visualPlot.vb"

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

    '   Total Lines: 958
    '    Code Lines: 704 (73.49%)
    ' Comment Lines: 173 (18.06%)
    '    - Xml Docs: 92.49%
    ' 
    '   Blank Lines: 81 (8.46%)
    '     File Size: 44.63 KB


    ' Module visualPlot
    ' 
    '     Function: CategoryProfilePlots, class_heatmap, ClassChangePlot, colorBends, delete
    '               DEMBarPlot, GoEnrichBubbles, KEGGCategoryProfile, KEGGCategoryProfilePlots, KEGGEnrichBubbles
    '               keggEnrichmentBubble2, Plot, PlotCMeans3D, PlotExpressionPatterns, plotGSVA
    '               plotSingle, toBubbles, VolcanoPlot
    ' 
    '     Sub: DrawSampleColorBend, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Data
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.ColorBrewer
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.C.CLangStringFormatProvider
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports SMRUCC.genomics.Visualize.ExpressionPattern
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes.LinqPipeline
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Internal.Object.Linq
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting
Imports featureFrame = Microsoft.VisualBasic.Math.DataFrame.DataFrame
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports std = System.Math
Imports stdVec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' package module for biological analysis data visualization
''' </summary>
<Package("visualPlot", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module visualPlot

    Sub Main()
        Call Internal.generic.add("plot", GetType(ExpressionPattern), AddressOf Plot)
        Call Internal.generic.add("plot", GetType(CatalogProfiles), AddressOf CategoryProfilePlots)
        Call Internal.generic.add("plot", GetType(GSVADiff()), AddressOf plotGSVA)
    End Sub

    Public Function plotGSVA(diff As GSVADiff(), args As list, env As Environment) As Object
        Dim size As SizeF = graphicsPipeline.getSize(args.slots, env, [default]:=New SizeF(2100, 3700))
        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(
                padding:=args.getByName("padding"),
                [default]:="padding: 300px 300px 400px 200px"
            )
        }
        Dim bar As New GSVADiffBar(diff, theme)

        Return bar.Plot(size)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sample">
    ''' DAM data between sample group comparisions, this parameter 
    ''' has two data format:
    ''' 
    ''' 1. dataframe - should contains at least two data fields: up and down, 
    '''    field values of up and down is the DAM counts number. row data in
    '''    dataframe is the sample comparision groups
    ''' 2. list - elements in the list is the sample comparision groups and 
    '''    each element slot values should be a tuple list which contains the 
    '''    DAM id vector, and each id vector is named up and down.
    ''' </param>
    ''' <param name="upName"></param>
    ''' <param name="downName"></param>
    ''' <param name="size"></param>
    ''' <param name="padding"></param>
    ''' <param name="title"></param>
    ''' <param name="headN"></param>
    ''' <param name="ppi"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("dem.barplot")>
    Public Function DEMBarPlot(sample As Object,
                               Optional upName As String = "up",
                               Optional downName As String = "down",
                               <RRawVectorArgument>
                               Optional size As Object = "2700,2400",
                               <RRawVectorArgument>
                               Optional padding As Object = "padding:200px 350px 250px 200px;",
                               Optional title As String = "Statistic of Differently Expressed Metabolite",
                               Optional headN As Integer = 60,
                               Optional ppi As Integer = 300,
                               Optional env As Environment = Nothing) As Object

        Dim theme As New Theme With {
            .padding = InteropArgumentHelper.getPadding(padding, "padding:200px 100px 250px 200px;"),
            .axisLabelCSS = "font-style: normal; font-size: 13; font-family: " & FontFace.MicrosoftYaHei & ";",
            .XaxisTickFormat = "F0",
            .YaxisTickFormat = "F0"
        }

        Dim up As Double()
        Dim down As Double()
        Dim tags As String()

        If TypeOf sample Is RDataframe Then
            Dim sampleTable As RDataframe = DirectCast(sample, RDataframe)

            sampleTable = sampleTable.head(n:=headN)

            up = sampleTable.getVector(Of Double)(upName)
            down = sampleTable.getVector(Of Double)(downName)
            tags = sampleTable.rownames
        Else
            Throw New NotImplementedException
        End If

        Dim bar As New BiDirectionData With {
            .Factor1 = upName,
            .Factor2 = downName,
            .samples = tags _
                .Select(Function(label, i)
                            Return New BarDataSample With {
                                .data = {up(i), down(i)},
                                .tag = label
                            }
                        End Function) _
                .ToArray
        }
        Dim app As New BiDirectionBarPlot(bar, Color.Brown, Color.SteelBlue, theme) With {
            .main = title,
            .xlabel = "Number of Metabolites"
        }

        Return app.Plot(InteropArgumentHelper.getSize(size, env), ppi:=ppi)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="size"></param>
    ''' <param name="padding"></param>
    ''' <param name="bg"></param>
    ''' <param name="colorSet"></param>
    ''' <param name="showLabel"></param>
    ''' <param name="radius"></param>
    ''' <param name="xlab"></param>
    ''' <param name="orderByClass"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("classchange.plot")>
    Public Function ClassChangePlot(<RRawVectorArgument> genes As Object,
                                    <RRawVectorArgument> Optional size As Object = "3000,2400",
                                    <RRawVectorArgument> Optional padding As Object = g.DefaultUltraLargePadding,
                                    Optional bg As Object = "white",
                                    <RRawVectorArgument>
                                    Optional colorSet As Object = "Set1:c9",
                                    Optional showLabel As Boolean = False,
                                    <RRawVectorArgument>
                                    Optional radius As Object = "15,50",
                                    Optional xlab$ = "X",
                                    Optional orderByClass As orders = orders.none,
                                    Optional env As Environment = Nothing) As Object

        Dim geneList As pipeline = pipeline.TryCreatePipeline(Of DEGModel)(genes, env)
        Dim radiusRange As [Variant](Of DoubleRange, Message) = SMRUCC.Rsharp.GetDoubleRange(radius, env)

        If radiusRange Like GetType(Message) Then
            Return radiusRange.TryCast(Of Message)
        End If

        If geneList.isError Then
            Return geneList.getError
        Else
            genes = geneList.populates(Of DEGModel)(env).ToArray
        End If

        If Not showLabel Then
            genes = DirectCast(genes, DEGModel()) _
                .Select(Function(g)
                            Return New DEGModel With {
                                .[class] = g.class,
                                .label = Nothing,
                                .logFC = g.logFC,
                                .pvalue = g.pvalue
                            }
                        End Function) _
                .ToArray
        End If

        Return DirectCast(genes, DEGModel()) _
            .ClassChangePlot(
                size:=InteropArgumentHelper.getSize(size, env),
                padding:=InteropArgumentHelper.getPadding(padding),
                bg:=RColorPalette.getColor(bg, [default]:="white"),
                colorSet:=RColorPalette.getColorSet(colorSet),
                radius:=$"{radiusRange.TryCast(Of DoubleRange).Min},{radiusRange.TryCast(Of DoubleRange).Max}",
                xlab:=xlab,
                orderByClass:=orderByClass.ToString
            )
    End Function

    ''' <summary>
    ''' volcano plot of the different expression result
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="size"></param>
    ''' <param name="padding"></param>
    ''' <param name="bg"></param>
    ''' <param name="colors"></param>
    ''' <param name="pvalue"></param>
    ''' <param name="level"></param>
    ''' <param name="title$"></param>
    ''' <returns></returns>
    <ExportAPI("volcano")>
    Public Function VolcanoPlot(<RRawVectorArgument> genes As Object,
                                <RRawVectorArgument> Optional size As Object = "2400,2700",
                                <RRawVectorArgument> Optional padding As Object = g.DefaultUltraLargePadding,
                                Optional bg As Object = "white",
                                <RDefaultExpression>
                                Optional colors As Object = "~list(up='red',down='green',other='black')",
                                Optional pvalue As Double = 0.05,
                                Optional level As Double = 1.5,
                                Optional title$ = "volcano plot",
                                Optional env As Environment = Nothing) As Object

        Dim colorList As New Dictionary(Of Integer, Color)
        Dim geneSet As IDeg()
        Dim list As pipeline = pipeline.TryCreatePipeline(Of DEP_iTraq)(genes, env, suppress:=True)

        If list.isError Then
            list = pipeline.TryCreatePipeline(Of DEGModel)(genes, env, suppress:=False)

            If list.isError Then
                Return list.getError
            Else
                geneSet = list.populates(Of IDeg)(env).ToArray
            End If
        Else
            geneSet = list.populates(Of IDeg)(env).ToArray
        End If

        If colors Is Nothing Then
            colorList = New Dictionary(Of Integer, Color) From {
                {Types.Up, Color.Red},
                {Types.Down, Color.Green},
                {Types.None, Color.Gray}
            }
        Else
            With DirectCast(colors, list)
                Call colorList.Add(Types.Up, any.ToString(.slots("up")).TranslateColor)
                Call colorList.Add(Types.Down, any.ToString(.slots("down")).TranslateColor)
                Call colorList.Add(Types.None, any.ToString(.slots("other")).TranslateColor)
            End With
        End If

        pvalue = -Math.Log10(pvalue)
        level = Math.Log(level, 2)

        Dim toFactor = Function(x As DEGModel)
                           If x.pvalue < pvalue Then
                               Return 0
                           ElseIf Math.Abs(x.logFC) < level Then
                               Return 0
                           End If

                           If x.logFC > 0 Then
                               Return 1
                           Else
                               Return -1
                           End If
                       End Function

        Return Volcano.Plot(
                genes:=geneSet,
                colors:=colorList,
                factors:=toFactor,
                padding:="padding: 50 50 150 150",
                displayLabel:=LabelTypes.None,
                size:=size,
                log2Threshold:=level,
                pvalueThreshold:=pvalue,
                title:=title,
                displayCount:=True,
                labelP:=-1
            ) _
            .AsGDIImage _
            .CorpBlank(30, Color.White)
    End Function

    ''' <summary>
    ''' Create catalog profiles data for KEGG pathway 
    ''' enrichment result its data visualization.
    ''' </summary>
    ''' <param name="profiles">
    ''' kegg map id mapping to the profiles data
    ''' </param>
    ''' <param name="top"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("kegg.category_profile")>
    <RApiReturn(GetType(CatalogProfiles))>
    Public Function KEGGCategoryProfile(profiles As Object,
                                        Optional top% = 10,
                                        Optional sort As Boolean = True,
                                        Optional valueCut As Double = -1,
                                        Optional env As Environment = Nothing) As Object
        Dim profile As CatalogProfiles

        If TypeOf profiles Is Dictionary(Of String, Integer) Then
            profile = DirectCast(profiles, Dictionary(Of String, Integer)) _
                .Where(Function(v) v.Value > valueCut) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CDbl(a.Value)
                              End Function) _
                .DoKeggProfiles(top)
        ElseIf TypeOf profiles Is Dictionary(Of String, NamedValue(Of Double)()) Then
            profile = New CatalogProfiles(DirectCast(profiles, Dictionary(Of String, NamedValue(Of Double)()))).Take(top)
        ElseIf TypeOf profiles Is Dictionary(Of String, Double) Then
            profile = DirectCast(profiles, Dictionary(Of String, Double)).DoKeggProfiles(top)
        ElseIf TypeOf profiles Is list Then
            profile = DirectCast(profiles, list).slots _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CLRVector.asNumeric(a.Value).ElementAtOrDefault(Scan0)
                              End Function) _
                .DoKeggProfiles(top)
        ElseIf TypeOf profiles Is CatalogProfiles Then
            profile = DirectCast(profiles, CatalogProfiles).Take(top)
        Else
            Return Internal.debug.stop("invalid data type for plot kegg category profile plot!", env)
        End If

        Return profile
    End Function

    ''' <summary>
    ''' delete a set of the category data from the input profiles
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <param name="catalogs"></param>
    ''' <returns></returns>
    <ExportAPI("erase")>
    Public Function delete(profiles As CatalogProfiles, catalogs As String()) As CatalogProfiles
        For Each catName As String In catalogs
            Call profiles.delete(catName)
        Next

        Return profiles
    End Function

    <ExportAPI("kegg.enrichment.bubble2")>
    Public Function keggEnrichmentBubble2(terms As EnrichmentResult(),
                                          <RRawVectorArgument>
                                          Optional size As Object = "12000,10000",
                                          <RRawVectorArgument>
                                          Optional padding As Object = "padding:300px 2500px 500px 300px;",
                                          Optional colorset As String = SequentialSchemes.YlGnBu9,
                                          Optional top_n As Integer = 9,
                                          Optional mapLevels As Integer = 25,
                                          Optional label_font As String = "font-style: normal; font-size: 42; font-family: " & FontFace.Verdana & ";",
                                          Optional tick_font As String = "font-style: normal; font-size: 36; font-family: " & FontFace.Verdana & ";",
                                          Optional axis_stroke As String = "stroke: black; stroke-width: 10px; stroke-dash: solid;",
                                          Optional legend_title_font As String = "font-style: normal; font-size: 50; font-family: " & FontFace.Verdana & ";",
                                          Optional grid_stroke As String = "stroke: lightgray; stroke-width: 10px; stroke-dash: dash;",
                                          Optional dpi As Integer = 300,
                                          Optional env As Environment = Nothing) As Object

        Dim padding_val As String = InteropArgumentHelper.getPadding(padding, [default]:="padding:500px 2000px 500px 300px;", env)
        Dim size_val = InteropArgumentHelper.getSize(size, env, "6000,10000")
        Dim theme As New Theme With {
            .colorSet = colorset,
            .padding = padding_val,
            .axisLabelCSS = label_font,
            .axisTickCSS = tick_font,
            .axisStroke = axis_stroke,
            .legendTitleCSS = legend_title_font,
            .gridStrokeX = grid_stroke,
            .gridStrokeY = grid_stroke
        }
        Dim app As New EnrichmentCategoryBubble(terms, theme, top_n:=top_n) With {
            .mapLevels = mapLevels
        }

        Return app.Plot(size_val, dpi, driver:=env.getDriver)
    End Function

    ''' <summary>
    ''' plot the heatmap with kegg class information
    ''' </summary>
    ''' <param name="x">
    ''' the molecule expression dataframe object, should contains the data of:
    ''' 
    ''' the row names in the dataframe is the molecule name labels and 
    ''' all the column fields should be the expression value in different 
    ''' samples.
    ''' </param>
    ''' <param name="metadata">
    ''' the metadata for the molecules of given expression data <paramref name="x"/>, should contains the metadata fields of:
    ''' 
    ''' 1. class: a character vector of the kegg class labels, example as pathway names, module names, or orthology labels
    ''' 2. logp: a numeric vector of the multiple group ANOVA test pvalue its log transform result of the molecules
    ''' 3. VIP: a numeric vector of the multiple group pls-da VIP result value for the molecules
    ''' 
    ''' the data field name is case-sensitive.
    ''' </param>
    ''' <param name="sampleinfo"></param>
    ''' <param name="size">the image size of the plot</param>
    ''' <param name="padding">the padding of the plot region</param>
    ''' <param name="label_font"></param>
    ''' <param name="tick_font"></param>
    ''' <param name="axisStroke"></param>
    ''' <param name="dpi"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("class_heatmap")>
    Public Function class_heatmap(x As dataframe, metadata As dataframe, sampleinfo As SampleInfo(),
                                  <RRawVectorArgument>
                                  Optional size As Object = "10000,6500",
                                  <RRawVectorArgument>
                                  Optional padding As Object = "padding: 300px 1600px 1200px 300px;",
                                  Optional label_font As String = "font-style: normal; font-size: 18; font-family: " & FontFace.BookmanOldStyle & ";",
                                  Optional tick_font As String = "font-style: normal; font-size: 12; font-family: " & FontFace.BookmanOldStyle & ";",
                                  Optional axisStroke As String = "stroke: black; stroke-width: 5px; stroke-dash: solid;",
                                  Optional dpi As Integer = 300,
                                  Optional env As Environment = Nothing) As Object

        Dim matrix = MathDataSet.toFeatureSet(x, env)
        Dim metaset = MathDataSet.toFeatureSet(metadata, env)

        If TypeOf matrix Is Message Then
            Return matrix
        ElseIf TypeOf metaset Is Message Then
            Return metaset
        End If

        Dim theme As New Theme With {
            .axisTickCSS = tick_font,
            .colorSet = $"rev({ColorBrewer.DivergingSchemes.RdBu11})",
            .padding = InteropArgumentHelper.getPadding(padding, "padding: 300px 1200px 300px 100px;"),
            .tagCSS = label_font,
            .axisStroke = axisStroke
        }
        Dim heatmap As New EnrichmentCategoryHeatmap(
            data:=DirectCast(matrix, featureFrame),
            metadata:=DirectCast(metaset, featureFrame),
            groupd:=sampleinfo,
            theme:=theme) With {
            .mapLevels = 30
        }
        Dim size_str As String = InteropArgumentHelper.getSize(size, env, "10000,6500")

        Return heatmap.Plot(size_str.SizeParser, dpi, env.getDriver)
    End Function

    ''' <summary>
    ''' plot kegg enrichment result in bubble plot
    ''' </summary>
    ''' <param name="profiles">
    ''' A dataframe object(or a set of the dataframe object in tuple list) 
    ''' that contains the kegg pathway enrichment data, data fields is 
    ''' required for generates the bubble data:
    ''' 
    ''' + ``Raw p``: pvalue of the enrichment term
    ''' + ``Impact``: x axis value
    ''' + ``Hits``: the molecule hits number in current enrichment term
    ''' + ``pathway``: the kegg pathway id
    ''' 
    ''' </param>
    ''' <param name="size"></param>
    ''' <param name="padding"></param>
    ''' <param name="unenrichColor"></param>
    ''' <param name="ppi"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("kegg.enrichment.bubbles")>
    Public Function KEGGEnrichBubbles(<RRawVectorArgument> profiles As Object,
                                      <RRawVectorArgument>
                                      Optional size As Object = "3800,2600",
                                      Optional padding As Object = "padding:300px 1000px 300px 200px;",
                                      Optional unenrichColor As String = NameOf(Color.LightGray),
                                      Optional themeColors As String = "Set1:c8",
                                      Optional alpha As Double = 0.75,
                                      Optional displays As Integer = 5,
                                      Optional serialTopn As Boolean = False,
                                      <RRawVectorArgument(GetType(Double))>
                                      Optional bubbleRadius As Object = "7,40",
                                      Optional heatmap As Boolean = False,
                                      Optional bubbleStyle As Boolean = False,
                                      Optional top_samples As Integer = 16,
                                      Optional ppi As Integer = 300,
                                      Optional env As Environment = Nothing) As Object

        Dim bubbleSize = SMRUCC.Rsharp.GetDoubleRange(bubbleRadius, env, [default]:="7,40")
        Dim sizeStr As String = InteropArgumentHelper.getSize(size, env, "2700,2300")
        Dim isGeneric As Boolean = TypeOf profiles Is dataframe
        Dim theme As New Theme With {
            .gridFill = "white",
            .padding = padding,
            .legendLabelCSS = "font-style: normal; font-size: 12; font-family: " & FontFace.MicrosoftYaHei & ";",
            .axisLabelCSS = "font-style: normal; font-size: 16; font-family: " & FontFace.MicrosoftYaHei & ";",
            .axisTickCSS = "font-style: normal; font-size: 10; font-family: " & FontFace.BookmanOldStyle & ";",
            .colorSet = themeColors
        }

        If bubbleSize Like GetType(Message) Then
            Return bubbleSize.TryCast(Of Message)
        End If

        If isGeneric Then
            ' plot single group
            Dim enrichment As dataframe = DirectCast(profiles, dataframe)

            If enrichment.nrows = 0 Then
                With sizeStr.SizeParser
                    Return New Bitmap(.Width, .Height)
                End With
            End If

            Return enrichment.plotSingle(
                sizeStr:=sizeStr,
                theme:=theme,
                unenrichColor:=unenrichColor,
                padding:=InteropArgumentHelper.getPadding(padding),
                ppi:=ppi,
                bubbleRadius:=bubbleSize,
                displays:=displays,
                serialTopn:=serialTopn
            )
        ElseIf TypeOf profiles Is list Then
            ' multiple groups on multiple dataframe
            Dim multiples As New List(Of NamedValue(Of Dictionary(Of String, BubbleTerm())))
            Dim rawList = DirectCast(profiles, list)

            For Each name As String In rawList.getNames
                profiles = rawList.getValue(Of dataframe)(name, env, [default]:=Nothing)

                If Not profiles Is Nothing Then
                    Dim bubbleData As Dictionary(Of String, BubbleTerm()) = DirectCast(profiles, dataframe).toBubbles
                    Dim data As New NamedValue(Of Dictionary(Of String, BubbleTerm())) With {
                       .Name = name,
                       .Value = bubbleData
                    }

                    Call multiples.Add(data)
                End If
            Next

            Dim app As Plot

            If heatmap Then
                multiples = MultipleBubble.TopBubbles(multiples, displays, top_samples, Function(b) b.data).AsList

                If bubbleStyle Then
                    app = New CatalogBubbleHeatmap(multiples, mapLevels:=100, bubbleSize, theme) With {
                        .main = "Pathway enrichment analysis"
                    }
                Else
                    app = New CatalogHeatMap(multiples, 100, theme)
                End If
            Else
                multiples = MultipleBubble.TopBubbles(multiples, displays, top_samples, Function(b) b.PValue * b.Factor).AsList
                app = New MultipleBubble(
                    multiples:=multiples,
                    theme:=theme,
                    radius:=bubbleSize,
                    alpha:=alpha
                ) With {
                    .legendTitle = "Samples",
                    .main = "KEGG Enrichments",
                    .xlabel = "-log10(pvalue) * Pathway Impacts"
                }
            End If

            Return app.Plot(sizeStr, ppi:=ppi)
        Else
            Throw New NotImplementedException
        End If
    End Function

    ''' <summary>
    ''' the pvalue has been transform via -log10
    ''' </summary>
    ''' <param name="enrichment"></param>
    ''' <returns></returns>
    <Extension>
    Private Function toBubbles(enrichment As dataframe) As Dictionary(Of String, BubbleTerm())
        Dim rawP As stdVec = enrichment.getVector(Of Double)("Raw p").AsVector
        ' Y
        Dim logP As stdVec = -rawP.Log(base:=10)
        ' X
        Dim Impact As stdVec = enrichment.getVector(Of Double)("Impact")
        Dim values As stdVec = enrichment.getVector(Of Double)("Hits")
        Dim pathwayList As String() = enrichment.getVector(Of String)("pathway")
        Dim bubbleData As Dictionary(Of String, BubbleTerm()) = BubbleTerm.CreateBubbles(logP, Impact, values, pathwayList)

        Return bubbleData
    End Function

    <Extension>
    Private Function plotSingle(enrichment As dataframe,
                                sizeStr As String,
                                theme As Theme,
                                unenrichColor As String,
                                padding As String,
                                ppi As Integer,
                                displays As Integer,
                                serialTopn As Boolean,
                                bubbleRadius As DoubleRange) As Object

        Dim bubbleData As Dictionary(Of String, BubbleTerm()) = enrichment.toBubbles
        Dim enrichColors = BubbleTerm.CreateEnrichColors(
            bubbleData:=bubbleData,
            theme:=theme.colorSet
        )
        Dim baseColor As Color = unenrichColor.TranslateColor
        Dim bubble As New CatalogBubblePlot(
            data:=bubbleData,
            enrichColors:=enrichColors,
            showBubbleBorder:=False,
            displays:=New LabelDisplayStrategy With {.displays = displays, .serialTopn = serialTopn},
            pvalue:=-std.Log10(0.05),
            unenrich:=baseColor,
            theme:=theme,
            bubbleSize:=bubbleRadius
        ) With {
            .xlabel = "Topology Impact",
            .ylabel = "-log10(p-value)"
        }

        Try
            Return bubble.Plot(sizeStr, ppi:=ppi)
        Catch ex As Exception
            With sizeStr.SizeParser
                Return New Bitmap(.Width, .Height)
            End With
        End Try
    End Function

    ''' <summary>
    ''' plot of the Go enrichment in bubble plot style
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <param name="goDb"></param>
    ''' <param name="size"></param>
    ''' <param name="pvalue"></param>
    ''' <param name="topN"></param>
    ''' <param name="R"></param>
    ''' <returns></returns>
    <ExportAPI("go.enrichment.bubbles")>
    Public Function GoEnrichBubbles(profiles As EnrichmentTerm(), goDb As GO_OBO,
                                    Optional size$ = "2000,1600",
                                    Optional pvalue# = 0.05,
                                    Optional topN% = 10,
                                    Optional R$ = "10,50") As Object

        Dim terms As Dictionary(Of Term) = goDb.AsEnumerable.ToDictionary

        Return profiles.BubblePlot(
            GO_terms:=terms,
            pvalue:=pvalue,
            radius:=R,
            size:=size,
            displays:=topN
        )
    End Function

    Public Function CategoryProfilePlots(profiles As CatalogProfiles, args As list, env As Environment) As Object
        Return profiles.ProfilesPlot(
            title:=args.getValue("title", env, "Catalog Profiling"),
            size:=InteropArgumentHelper.getSize(args!size, env, "2300,2000"),
            tick:=args.getValue("tick", env, -1.0),
            axisTitle:=args.getValue("axis.title", env, "Number Of Terms"),
            labelRightAlignment:=False,
            valueFormat:=args.getValue("format", env, "F2"),
            colorSchema:=args.getValue("colors", env, "#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00"),
            dpi:=args.getValue("dpi", env, 300)
        )
    End Function

    ''' <summary>
    ''' Do plot of the given catalog profiles data
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <param name="title"></param>
    ''' <param name="axis_title"></param>
    ''' <param name="size">the size of the image</param>
    ''' <param name="tick">axis ticks, default value -1 for auto generated.</param>
    ''' <param name="colors">the color schema name</param>
    ''' <returns></returns>
    <ExportAPI("category_profiles.plot")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function KEGGCategoryProfilePlots(profiles As CatalogProfiles,
                                             Optional title$ = "KEGG Orthology Profiling",
                                             Optional axis_title$ = "Number Of Proteins",
                                             <RRawVectorArgument>
                                             Optional size As Object = "2300,2000",
                                             Optional tick# = -1,
                                             <RRawVectorArgument>
                                             Optional colors As Object = "#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00",
                                             Optional dpi As Integer = 300,
                                             Optional format$ = "F2",
                                             Optional env As Environment = Nothing) As Object
        Return profiles.ProfilesPlot(
            title:=title,
            size:=InteropArgumentHelper.getSize(size, env),
            tick:=tick,
            axisTitle:=axis_title,
            labelRightAlignment:=False,
            valueFormat:=format,
            colorSchema:=colors,
            dpi:=dpi
        )
    End Function

    <ExportAPI("sample.color_bend")>
    Public Sub DrawSampleColorBend(g As IGraphics, layout As RectangleF, geneExpression As Color(),
                                   Optional horizontal As Boolean = True,
                                   Optional sampleNames As String() = Nothing,
                                   Optional labelFontCSS$ = CSSFont.PlotSmallTitle）

        Call SampleColorBend.Draw(g, layout, geneExpression, horizontal, sampleNames, labelFontCSS)
    End Sub

    ''' <summary>
    ''' map gene expressin data to color bends
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="colorSet$"></param>
    ''' <param name="levels"></param>
    ''' <returns></returns>
    <ExportAPI("color_bends")>
    Public Function colorBends(matrix As Matrix,
                               Optional colorSet$ = "RdYlGn:c8",
                               Optional levels As Integer = 25,
                               Optional env As Environment = Nothing) As list

        Return New list With {
            .slots = SampleColorBend _
                .GetColors(matrix, colorSet, levels) _
                .ToDictionary(Function(a) a.name,
                              Function(a)
                                  Return CObj(New vector(matrix.sampleID, a.value, env))
                              End Function)
        }
    End Function

    Private Function Plot(matrix As ExpressionPattern, args As list, env As Environment) As Object
        Dim type As String = args.getValue(Of String)("type", env, "patterns")
        Dim size As String = InteropArgumentHelper.getSize(args!size, env, "2400,2700")
        Dim padding As String = InteropArgumentHelper.getPadding(args!padding, g.DefaultLargerPadding)
        Dim bg As String = RColorPalette.getColor(args!bg, "white")
        Dim colorSet As String = RColorPalette.getColorSet(args!colorSet, "PiYG:c8")
        Dim levels As Integer = args.getValue(Of Integer)("levels", env, 25)
        Dim clusterLabelStyle As String = InteropArgumentHelper.getFontCSS(args("cluster_labels.cex"), CSSFont.PlotSubTitle)
        Dim legendTitleStyle As String = InteropArgumentHelper.getFontCSS("legend_title.cex", CSSFont.Win7Small)
        Dim legendTickStyle As String = InteropArgumentHelper.getFontCSS("legend_tick.cex", CSSFont.Win7Small)
        Dim axisTickCSS As String = InteropArgumentHelper.getFontCSS("axis_tick.cex", CSSFont.Win10Normal)
        Dim axisLabelCSS As String = InteropArgumentHelper.getFontCSS("axis_label.cex", CSSFont.Win7Normal)
        Dim ppi As Integer = args.getValue("ppi", env, 300)
        Dim topMembers As Double = args.getValue("top_members", env, 500)

        Return matrix.DrawMatrix(
            size:=size,
            padding:=padding,
            bg:=bg,
            colorSet:=colorSet,
            levels:=levels,
            clusterLabelStyle:=clusterLabelStyle,
            legendTickStyle:=legendTickStyle,
            legendTitleStyle:=legendTitleStyle,
            axisLabelCSS:=axisLabelCSS,
            axisTickCSS:=axisTickCSS,
            ppi:=ppi,
            topMembers:=topMembers
        )
    End Function

    ''' <summary>
    ''' Visualize of the gene expression patterns across different sample groups. 
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="size"></param>
    ''' <param name="padding"></param>
    ''' <param name="bg"></param>
    ''' <param name="colorSet">color set for visualize the cmeans membership</param>
    ''' <returns></returns>
    <ExportAPI("plot.expression_patterns")>
    Public Function PlotExpressionPatterns(matrix As ExpressionPattern,
                                           <RRawVectorArgument>
                                           Optional size As Object = "2400,2700",
                                           <RRawVectorArgument>
                                           Optional padding As Object = g.DefaultUltraLargePadding,
                                           Optional bg As Object = "white",
                                           Optional colorSet$ = "PiYG:c8",
                                           Optional levels% = 25,
                                           Optional clusterLabelStyle As String = CSSFont.PlotSubTitle,
                                           Optional legendTitleStyle As String = CSSFont.Win7Small,
                                           Optional legendTickStyle As String = CSSFont.Win7Small,
                                           Optional axisTickCSS$ = CSSFont.Win10Normal,
                                           Optional axisLabelCSS$ = CSSFont.Win7Normal,
                                           Optional driver As Drivers = Drivers.Default,
                                           Optional ppi As Integer = 300,
                                           Optional env As Environment = Nothing) As Object

        Return matrix.DrawMatrix(
            size:=InteropArgumentHelper.getSize(size, env),
            padding:=InteropArgumentHelper.getPadding(padding),
            bg:=RColorPalette.getColor(bg, "white"),
            colorSet:=colorSet,
            levels:=levels,
            clusterLabelStyle:=clusterLabelStyle,
            legendTickStyle:=legendTickStyle,
            legendTitleStyle:=legendTitleStyle,
            axisLabelCSS:=axisLabelCSS,
            axisTickCSS:=axisTickCSS,
            driver:=driver,
            ppi:=ppi
        )
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="size"></param>
    ''' <param name="padding"></param>
    ''' <param name="bg"></param>
    ''' <param name="colorSet$"></param>
    ''' <param name="viewAngle"></param>
    ''' <param name="viewDistance#"></param>
    ''' <param name="qDisplay">quantile value for display the gene labels</param>
    ''' <returns></returns>
    <ExportAPI("plot.cmeans3D")>
    Public Function PlotCMeans3D(matrix As ExpressionPattern,
                                 Optional kmeans_n As Integer = 3,
                                 <RRawVectorArgument>
                                 Optional size As Object = "2400,2700",
                                 <RRawVectorArgument>
                                 Optional padding As Object = g.DefaultUltraLargePadding,
                                 Optional bg As Object = "white",
                                 Optional colorSet$ = "red,blue,green",
                                 <RRawVectorArgument(GetType(Double))>
                                 Optional viewAngle As Object = "30,60,-56.25",
                                 Optional viewDistance# = 2500,
                                 Optional qDisplay# = 0.9,
                                 Optional prefix$ = "Cluster: #",
                                 Optional axisFormat$ = "CMeans dimension #%s",
                                 Optional showHull As Boolean = True,
                                 Optional hullAlpha As Integer = 150,
                                 Optional hullBspline As Single = 3,
                                 Optional env As Environment = Nothing) As Object

        Dim clusterData As EntityClusterModel() = matrix.Patterns _
            .Select(Function(a)
                        Return New EntityClusterModel With {
                            .ID = a.uid,
                            .Cluster = a.cluster,
                            .Properties = a.memberships _
                                .ToDictionary(Function(t) (t.Key + 1).ToString,
                                              Function(t)
                                                  Return t.Value * 100
                                              End Function)
                        }
                    End Function) _
            .ToArray
        Dim normRange As DoubleRange = New Double() {0, 100}
        Dim clusterNMembers As Integer = clusterData.Length

        For Each project As String In clusterData _
            .Select(Function(a) a.Properties.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray

            Dim v As Double() = clusterData.Select(Function(a) a(project)).ToArray
            Dim range As DoubleRange = v.Range
            Dim map As Double

            For i As Integer = 0 To clusterNMembers - 1
                map = range.ScaleMapping(clusterData(i)(project), normRange)
                clusterData(i)(project) = map
            Next
        Next

        clusterData = clusterData _
            .Kmeans(
                expected:=kmeans_n,
                debug:=env.globalEnvironment.debugMode
            ) _
            .ToArray

        Dim camera As New Camera With {
            .fov = 500000,
            .screen = InteropArgumentHelper.getSize(size, env).SizeParser,
            .viewDistance = viewDistance,
            .angleX = DirectCast(viewAngle, Double())(0),
            .angleY = DirectCast(viewAngle, Double())(1),
            .angleZ = DirectCast(viewAngle, Double())(2)
        }
        Dim category As Dictionary(Of NamedCollection(Of String)) = clusterData _
            .GroupBy(Function(a) a.Cluster) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return New NamedCollection(Of String) With {
                                  .name = sprintf(axisFormat, a.Key),
                                  .value = {a.Key}
                              }
                          End Function)

        Dim arrowFactor$ = "1,1"

        If Not prefix.StringEmpty Then
            For Each protein As EntityClusterModel In clusterData
                protein.Cluster = prefix & protein.Cluster
            Next
        End If

        Return clusterData _
            .Scatter3D(
                catagory:=category,
                camera:=camera,
                bg:=bg,
                padding:=InteropArgumentHelper.getPadding(padding),
                size:=InteropArgumentHelper.getSize(size, env),
                schema:=colorSet,
                arrowFactor:=arrowFactor,
                labelsQuantile:=qDisplay,
                showLegend:=False,
                showHull:=showHull,
                hullAlpha:=hullAlpha,
                hullBspline:=hullBspline
            ) _
            .AsGDIImage _
            .CorpBlank(30, Color.White)
    End Function
End Module
