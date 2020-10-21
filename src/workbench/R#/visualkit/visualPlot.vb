#Region "Microsoft.VisualBasic::0cc3d683f62f82e442ca87ec4b63e73b, visualkit\visualPlot.vb"

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

' Module visualPlot
' 
'     Function: colorBends, doKeggProfiles, GoEnrichBubbles, GOEnrichmentProfiles, KEGGCategoryProfile
'               KEGGCategoryProfilePlots, PlotExpressionPatterns
' 
'     Sub: DrawSampleColorBend
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports SMRUCC.genomics.Visualize.ExpressionPattern
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("visualkit.plots")>
Module visualPlot

    ''' <summary>
    ''' Create catalog profiles data for GO enrichment result its data visualization.
    ''' </summary>
    ''' <param name="enrichments"></param>
    ''' <param name="goDb"></param>
    ''' <param name="top">display the top n enriched GO terms.</param>
    ''' <returns></returns>
    <ExportAPI("GO.enrichment.profile")>
    Public Function GOEnrichmentProfiles(enrichments As EnrichmentTerm(), goDb As GO_OBO, Optional top% = 10) As Object
        Dim GO_terms = goDb.AsEnumerable.ToDictionary
        ' 在这里是不进行筛选的
        ' 筛选应该是发生在脚本之中
        Dim profiles = enrichments.CreateEnrichmentProfiles(GO_terms, False, top, 1)

        Return profiles
    End Function

    ''' <summary>
    ''' Create catalog profiles data for KEGG pathway enrichment result its data visualization.
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <param name="top"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("kegg.category_profile")>
    Public Function KEGGCategoryProfile(profiles As Object, Optional top% = 10, Optional env As Environment = Nothing) As Object
        Dim profile As Dictionary(Of String, NamedValue(Of Double)())

        If TypeOf profiles Is Dictionary(Of String, Integer) Then
            profile = DirectCast(profiles, Dictionary(Of String, Integer)) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CDbl(a.Value)
                              End Function) _
                .doKeggProfiles(top)
        ElseIf TypeOf profiles Is Dictionary(Of String, NamedValue(Of Double)()) Then
            profile = DirectCast(profiles, Dictionary(Of String, NamedValue(Of Double)()))
        ElseIf TypeOf profiles Is Dictionary(Of String, Double) Then
            profile = DirectCast(profiles, Dictionary(Of String, Double)).doKeggProfiles(top)
        ElseIf TypeOf profiles Is list Then
            profile = DirectCast(profiles, list).slots _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CDbl(REnv.asVector(Of Double)(a.Value).GetValue(Scan0))
                              End Function) _
                .doKeggProfiles(top)
        Else
            Return Internal.debug.stop("invalid data type for plot kegg category profile plot!", env)
        End If

        Return profile
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
                                    Optional R$ = "log(x,1.5)") As Object

        Dim terms As Dictionary(Of Term) = goDb.AsEnumerable.ToDictionary

        Return profiles.BubblePlot(
            GO_terms:=terms,
            pvalue:=pvalue,
            R:=R,
            size:=size,
            displays:=topN
        )
    End Function

    ''' <summary>
    ''' Do plot of the given catalog profiles data
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <param name="title"></param>
    ''' <param name="axisTitle"></param>
    ''' <param name="size">the size of the image</param>
    ''' <param name="tick">axis ticks, default value -1 for auto generated.</param>
    ''' <param name="colors">the color schema name</param>
    ''' <returns></returns>
    <ExportAPI("category_profiles.plot")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function KEGGCategoryProfilePlots(profiles As Object,
                                             Optional title$ = "KEGG Orthology Profiling",
                                             Optional axisTitle$ = "Number Of Proteins",
                                             <RRawVectorArgument>
                                             Optional size As Object = "2300,2000",
                                             Optional tick# = -1,
                                             <RRawVectorArgument>
                                             Optional colors As Object = "#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00",
                                             Optional env As Environment = Nothing) As Object

        Dim profile As Dictionary(Of String, NamedValue(Of Double)()) = profiles

        Return profile.ProfilesPlot(title,
            size:=InteropArgumentHelper.getSize(size),
            tick:=tick,
            axisTitle:=axisTitle,
            labelRightAlignment:=False,
            valueFormat:="F0",
            colorSchema:=colors
        )
    End Function

    <Extension>
    Private Function doKeggProfiles(profiles As Dictionary(Of String, Double), displays%) As Dictionary(Of String, NamedValue(Of Double)())
        Return profiles _
            .KEGGCategoryProfiles _
            .Where(Function(cls) Not cls.Value.IsNullOrEmpty) _
            .ToDictionary(Function(p) p.Key,
                          Function(group)
                              Return group.Value _
                                  .OrderByDescending(Function(t) t.Value) _
                                  .Take(displays) _
                                  .ToArray
                          End Function)
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
                                           Optional axisLabelCSS$ = CSSFont.Win7Small) As Object

        Return matrix.DrawMatrix(
            size:=InteropArgumentHelper.getSize(size),
            padding:=InteropArgumentHelper.getPadding(padding),
            bg:=InteropArgumentHelper.getColor(bg, "white"),
            colorSet:=colorSet,
            levels:=levels,
            clusterLabelStyle:=clusterLabelStyle,
            legendTickStyle:=legendTickStyle,
            legendTitleStyle:=legendTitleStyle,
            axisLabelCSS:=axisLabelCSS,
            axisTickCSS:=axisTickCSS
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
                                 <RRawVectorArgument>
                                 Optional size As Object = "2400,2700",
                                 <RRawVectorArgument>
                                 Optional padding As Object = g.DefaultUltraLargePadding,
                                 Optional bg As Object = "white",
                                 Optional colorSet$ = "red,blue,green",
                                 <RRawVectorArgument(GetType(Double))>
                                 Optional viewAngle As Object = "30,60,-56.25",
                                 Optional viewDistance# = 2500,
                                 Optional qDisplay# = -1.0,
                                 Optional prefix$ = "Cluster:  #") As Object

        Dim clusterData As EntityClusterModel() = matrix.Patterns _
            .Select(Function(a)
                        Return New EntityClusterModel With {
                            .ID = a.uid,
                            .Cluster = a.cluster,
                            .Properties = a.memberships _
                                .ToDictionary(Function(t) t.Key.ToString,
                                              Function(t)
                                                  Return t.Value
                                              End Function)
                        }
                    End Function) _
            .ToArray

        Dim camera As New Camera With {
            .fov = 500000,
            .screen = InteropArgumentHelper.getSize(size).SizeParser,
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
                                  .name = a.Key,
                                  .value = a _
                                      .Select(Function(g) g.ID) _
                                      .ToArray
                              }
                          End Function)

        Dim arrowFactor$ = "1,2"

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
                padding:=padding,
                size:=size,
                schema:=colorSet,
                arrowFactor:=arrowFactor,
                labelsQuantile:=qDisplay
            ) _
            .AsGDIImage _
            .CorpBlank(30, Color.White)
    End Function
End Module
