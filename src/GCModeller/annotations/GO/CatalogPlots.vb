#Region "Microsoft.VisualBasic::1defc9ff5b185ee0edc83cc979896f1a, GCModeller\annotations\GO\CatalogPlots.vb"

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

    '   Total Lines: 396
    '    Code Lines: 304
    ' Comment Lines: 43
    '   Blank Lines: 49
    '     File Size: 16.99 KB


    ' Module CatalogPlots
    ' 
    '     Function: CreateEnrichmentProfiles, (+2 Overloads) EnrichmentPlot, (+4 Overloads) Plot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.GoStat
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Visualize.CatalogProfiling

Public Module CatalogPlots

    <Extension>
    Public Function Plot(Of gene)(annotations As IEnumerable(Of gene),
                                  getGO As Func(Of gene, String()),
                                  GO_terms As Dictionary(Of String, Term),
                                  Optional title$ = "Gene Ontology Profiling",
                                  Optional axisTitle$ = "Number Of Gene",
                                  Optional colorSchema$ = "Set1:c6",
                                  Optional bg$ = "white",
                                  Optional size$ = "2200,2000",
                                  Optional padding$ = g.DefaultPadding,
                                  Optional classFontStyle$ = CSSFont.Win7LargerBold,
                                  Optional catalogFontStyle$ = CSSFont.Win7Bold,
                                  Optional titleFontStyle$ = CSSFont.PlotTitle,
                                  Optional valueFontStyle$ = CSSFont.Win7Bold,
                                  Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                                  Optional tick% = 50) As GraphicsData


        Dim data = annotations.CountStat(getGO, GO_terms) ' 返回来的是 Go_ID, label, count
        Dim profile As New Dictionary(Of String, NamedValue(Of Double)())

        For Each [class] In data.Keys
            Dim counts = data([class])
            Dim stat As New List(Of NamedValue(Of Double))

            For Each catalog In counts
                If catalog.Description.StringEmpty Then
                    Continue For
                End If

                stat += New NamedValue(Of Double) With {
                    .Name = catalog.Description,
                    .Value = catalog.Value
                }
            Next

            Call profile.Add([class], stat)
        Next

        Return New CatalogProfiles(profile).ProfilesPlot(
            title, axisTitle, colorSchema,
            bg, size, padding,
            classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
            tickFontStyle, tick
        )
    End Function

    ''' <summary>
    ''' 这个函数适合绘制KOBAS的输出
    ''' </summary>
    ''' <typeparam name="gene"></typeparam>
    ''' <param name="annotations"></param>
    ''' <param name="getGO"></param>
    ''' <param name="GO_terms"><see cref="GO_OBO.Open(String)"/> for the dictionary data.</param>
    ''' <param name="title$"></param>
    ''' <param name="axisTitle$"></param>
    ''' <param name="colorSchema$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="size"></param>
    ''' <param name="classFontStyle$"></param>
    ''' <param name="catalogFontStyle$"></param>
    ''' <param name="titleFontStyle$"></param>
    ''' <param name="valueFontStyle$"></param>
    ''' <param name="tickFontStyle$"></param>
    ''' <param name="tick%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(Of gene)(annotations As IEnumerable(Of gene),
                                  getGO As Func(Of gene, (goID$, num As Integer)),
                                  GO_terms As Dictionary(Of String, Term),
                                  Optional title$ = "Gene Ontology Profiling",
                                  Optional axisTitle$ = "Number Of Gene",
                                  Optional colorSchema$ = "Set1:c6",
                                  Optional bg$ = "white",
                                  Optional size$ = "2200,2000",
                                  Optional padding$ = g.DefaultPadding,
                                  Optional classFontStyle$ = CSSFont.Win7LargerBold,
                                  Optional catalogFontStyle$ = CSSFont.Win7Bold,
                                  Optional titleFontStyle$ = CSSFont.PlotTitle,
                                  Optional valueFontStyle$ = CSSFont.Win7Bold,
                                  Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                                  Optional tick% = 50) As GraphicsData

        Dim data = annotations.CountStat(Function(g) {getGO(g)}, GO_terms) ' 返回来的是 Go_ID, label, count
        Dim profile As New Dictionary(Of String, NamedValue(Of Double)())

        For Each [class] In data.Keys
            Dim counts = data([class])
            Dim stat As New List(Of NamedValue(Of Double))

            For Each catalog In counts
                If catalog.Description.StringEmpty Then
                    Continue For
                End If

                stat += New NamedValue(Of Double) With {
                    .Name = catalog.Description,
                    .Value = catalog.Value
                }
            Next

            Call profile.Add([class], stat)
        Next

        Return New CatalogProfiles(profile).ProfilesPlot(
            title, axisTitle, colorSchema,
            bg, size, padding,
            classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
            tickFontStyle, tick
        )
    End Function

    <Extension>
    Public Function Plot(profile As Dictionary(Of String, NamedValue(Of Double)()),
                         Optional title$ = "Gene Ontology Profiling",
                         Optional axisTitle$ = "Number Of Gene",
                         Optional colorSchema$ = "Set1:c6",
                         Optional bg$ = "white",
                         Optional size$ = "2200,2000",
                         Optional padding$ = g.DefaultPadding,
                         Optional classFontStyle$ = CSSFont.Win7LargerBold,
                         Optional catalogFontStyle$ = CSSFont.Win7Bold,
                         Optional titleFontStyle$ = CSSFont.PlotTitle,
                         Optional valueFontStyle$ = CSSFont.Win7Bold,
                         Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                         Optional tick% = 50) As GraphicsData

        Return New CatalogProfiles(profile).ProfilesPlot(
            title, axisTitle, colorSchema,
            bg, size, padding,
            classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
            tickFontStyle, tick
        )
    End Function

    ''' <summary>
    ''' Catalog profiling plot for ``GO``.
    ''' </summary>
    ''' <param name="profile">原始的计数</param>
    ''' <param name="title$"></param>
    ''' <param name="axisTitle$"></param>
    ''' <param name="colorSchema$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="size"></param>
    ''' <param name="classFontStyle$"></param>
    ''' <param name="catalogFontStyle$"></param>
    ''' <param name="titleFontStyle$"></param>
    ''' <param name="valueFontStyle$"></param>
    ''' <param name="tickFontStyle$"></param>
    ''' <param name="tick%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(profile As Dictionary(Of String, NamedValue(Of Integer)()),
                         Optional selects$ = "Q3",
                         Optional title$ = "Gene Ontology Profiling",
                         Optional axisTitle$ = "Number Of Gene",
                         Optional colorSchema$ = "Set1:c6",
                         Optional bg$ = "white",
                         Optional size$ = "2200,2000",
                         Optional padding$ = g.DefaultPadding,
                         Optional classFontStyle$ = CSSFont.Win7LargerBold,
                         Optional catalogFontStyle$ = CSSFont.Win7Bold,
                         Optional titleFontStyle$ = CSSFont.PlotTitle,
                         Optional valueFontStyle$ = CSSFont.Win7Bold,
                         Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                         Optional tick% = 50,
                         Optional maxTermLength% = 72,
                         Optional labelAlignmentRight As Boolean = False,
                         Optional valueFormat$ = "F2") As GraphicsData

        Dim data As New Dictionary(Of String, NamedValue(Of Double)())
        Dim top As NamedValue(Of Double)()

        For Each k In profile
            Dim x As NamedValue(Of Integer)() = k.Value

            If Not selects.StringEmpty Then
                top = x _
                    .ApplySelector(Function(o) o.Value, selects) _
                    .Select(Function(o)
                                Return New NamedValue(Of Double) With {
                                    .Name = o.Description,
                                    .Value = o.Value
                                }
                            End Function) _
                    .ToArray

            Else

                top = x _
                    .Select(Function(o)
                                Return New NamedValue(Of Double) With {
                                    .Name = o.Description,
                                    .Value = o.Value
                                }
                            End Function) _
                    .ToArray
            End If

            Call data.Add(k.Key, top)
        Next

        Dim strip = Function(s$) If(
            Len(s) > maxTermLength,
            Mid(s, 1, maxTermLength) & "...",
            s)

        For Each k In data
            For i As Integer = 0 To k.Value.Length - 1
                Dim c As NamedValue(Of Double) = k.Value(i)
                k.Value(i) = New NamedValue(Of Double) With {
                    .Name = strip(c.Name),
                    .Value = c.Value,
                    .Description = strip(c.Description)
                }
            Next
        Next

        With New Dictionary(Of String, NamedValue(Of Double)())

            !biological_process = data!biological_process
            !cellular_component = data!cellular_component
            !molecular_function = data!molecular_function

            Return .DoCall(Function(profiles) New CatalogProfiles(profiles)) _
                   .ProfilesPlot(
                       title, axisTitle, colorSchema,
                       bg, size, padding,
                       classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
                       tickFontStyle, tick,
                       labelRightAlignment:=labelAlignmentRight,
                       valueFormat:=valueFormat
                   )
        End With
    End Function

    <Extension>
    Public Function EnrichmentPlot(data As IEnumerable(Of FunctionCluster),
                                   Optional size$ = "2200,2000",
                                   Optional tick# = 1,
                                   Optional pvalue# = 0.05,
                                   Optional colorSchema$ = "Set1:c6") As GraphicsData

        Dim profile As New Dictionary(Of String, NamedValue(Of Double)())
        Dim g = From x As FunctionCluster
                In data
                Where x.PValue <= pvalue
                Select x
                Group x By x.Category Into Group

        For Each cata In g
            Dim key$

            Select Case cata.Category
                Case "GOTERM_BP_DIRECT"
                    key = Ontologies.BiologicalProcess.Description
                Case "GOTERM_CC_DIRECT"
                    key = Ontologies.CellularComponent.Description
                Case "GOTERM_MF_DIRECT"
                    key = Ontologies.MolecularFunction.Description
                Case Else
                    Throw New Exception(cata.Category & " is an invalid catalog label!")
            End Select

            profile(key) = cata _
                .Group _
                .Select(Function(x) New NamedValue(Of Double) With {
                    .Name = x.Term,
                    .Value = -Math.Log10(x.PValue)
                }) _
                .ToArray
        Next

        Return New CatalogProfiles(profile).ProfilesPlot(
            "GO enrichment",
            size:=size,
            axisTitle:="-Log10(p-value)",
            tick:=tick,
            colorSchema:=colorSchema
        )
    End Function

    <Extension>
    Public Function CreateEnrichmentProfiles(Of EnrichmentTerm As IGoTermEnrichment)(
                    data As IEnumerable(Of EnrichmentTerm),
                    GO_terms As Dictionary(Of Term),
                    Optional usingCorrected As Boolean = False,
                    Optional top% = -1,
                    Optional pvalue# = 0.05) As CatalogProfiles

        Dim profile As New CatalogProfiles
        Dim testPvalue As Func(Of EnrichmentTerm, Boolean)
        Dim namespace$

        If usingCorrected Then
            testPvalue = Function(term) term.CorrectedPvalue <= pvalue
        Else
            testPvalue = Function(term) term.Pvalue <= pvalue
        End If

        For Each term As EnrichmentTerm In data _
            .Where(Function(x)
                       Return GO_terms.ContainsKey(x.Go_ID) AndAlso testPvalue(x)
                   End Function)

            With GO_terms(term.Go_ID)
                namespace$ = .namespace

                If Not profile.haveCategory([namespace]) Then
                    Call profile.catalogs.Add([namespace], New CatalogProfile)
                End If

                Call profile([namespace]).Add(.name, term.P)
            End With
        Next

        If top > 0 Then
            ' 已经转换为P值了，直接降序排序
            For Each GO_ns In profile.catalogs.ToArray
                Dim name$ = GO_ns.Key
                Dim terms = GO_ns.Value _
                    .AsEnumerable _
                    .OrderByDescending(Function(t) t.Value) _
                    .Take(top) _
                    .ToArray

                profile.catalogs(name) = terms
            Next
        End If

        Dim orders As New Dictionary(Of String, NamedValue(Of Double)())

        With orders
            !biological_process = profile!biological_process
            !cellular_component = profile!cellular_component
            !molecular_function = profile!molecular_function
        End With

        Return New CatalogProfiles(orders)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data">输出的结果都应该是不重复的</param>
    ''' <param name="GO_terms"></param>
    ''' <param name="size"></param>
    ''' <returns></returns>
    <Extension>
    Public Function EnrichmentPlot(Of EnrichmentTerm As IGoTermEnrichment)(data As IEnumerable(Of EnrichmentTerm),
                                                                           GO_terms As Dictionary(Of Term),
                                                                           Optional pvalue# = 0.05,
                                                                           Optional size$ = "2200,2000",
                                                                           Optional tick# = 1,
                                                                           Optional gray As Boolean = False,
                                                                           Optional labelRightAlignment As Boolean = False,
                                                                           Optional usingCorrected As Boolean = False,
                                                                           Optional top% = -1,
                                                                           Optional colorSchema$ = "Set1:c6",
                                                                           Optional disableLabelColor As Boolean = False,
                                                                           Optional labelMaxLen% = 64) As GraphicsData
        Return data _
            .CreateEnrichmentProfiles(GO_terms, usingCorrected, top, pvalue) _
            .ProfilesPlot(
                "GO enrichment",
                size:=size,
                axisTitle:="-Log10(p-value)",
                tick:=tick,
                gray:=gray,
                labelRightAlignment:=labelRightAlignment,
                colorSchema:=colorSchema,
                disableLabelColor:=disableLabelColor,
                labelTrimLength:=labelMaxLen
            )
    End Function
End Module
