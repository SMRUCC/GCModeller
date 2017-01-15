Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
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
                                  Optional size As Size = Nothing,
                                  Optional margin As Size = Nothing,
                                  Optional classFontStyle$ = CSSFont.Win7LargerBold,
                                  Optional catalogFontStyle$ = CSSFont.Win7Bold,
                                  Optional titleFontStyle$ = CSSFont.PlotTitle,
                                  Optional valueFontStyle$ = CSSFont.Win7Bold,
                                  Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                                  Optional tick% = 50) As Bitmap


        Dim data = annotations.CountStat(getGO, GO_terms) ' 返回来的是 Go_ID, label, count
        Dim profile As New Dictionary(Of String, NamedValue(Of Double)())

        For Each [class] In data.Keys
            Dim counts = data([class])
            Dim stat As New List(Of NamedValue(Of Double))

            For Each catalog In counts
                If catalog.Description.IsBlank Then
                    Continue For
                End If

                stat += New NamedValue(Of Double) With {
                    .Name = catalog.Description,
                    .Value = catalog.Value
                }
            Next

            Call profile.Add([class], stat)
        Next

        Return profile.ProfilesPlot(
            title, axisTitle, colorSchema,
            bg, size, margin,
            classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
            tickFontStyle, tick)
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
    ''' <param name="margin"></param>
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
                                  Optional size As Size = Nothing,
                                  Optional margin As Size = Nothing,
                                  Optional classFontStyle$ = CSSFont.Win7LargerBold,
                                  Optional catalogFontStyle$ = CSSFont.Win7Bold,
                                  Optional titleFontStyle$ = CSSFont.PlotTitle,
                                  Optional valueFontStyle$ = CSSFont.Win7Bold,
                                  Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                                  Optional tick% = 50) As Bitmap

        Dim data = annotations.CountStat(Function(g) {getGO(g)}, GO_terms) ' 返回来的是 Go_ID, label, count
        Dim profile As New Dictionary(Of String, NamedValue(Of Double)())

        For Each [class] In data.Keys
            Dim counts = data([class])
            Dim stat As New List(Of NamedValue(Of Double))

            For Each catalog In counts
                If catalog.Description.IsBlank Then
                    Continue For
                End If

                stat += New NamedValue(Of Double) With {
                    .Name = catalog.Description,
                    .Value = catalog.Value
                }
            Next

            Call profile.Add([class], stat)
        Next

        Return profile.ProfilesPlot(
            title, axisTitle, colorSchema,
            bg, size, margin,
            classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
            tickFontStyle, tick)
    End Function

    <Extension>
    Public Function Plot(profile As Dictionary(Of String, NamedValue(Of Double)()),
                         Optional title$ = "Gene Ontology Profiling",
                         Optional axisTitle$ = "Number Of Gene",
                         Optional colorSchema$ = "Set1:c6",
                         Optional bg$ = "white",
                         Optional size As Size = Nothing,
                         Optional margin As Size = Nothing,
                         Optional classFontStyle$ = CSSFont.Win7LargerBold,
                         Optional catalogFontStyle$ = CSSFont.Win7Bold,
                         Optional titleFontStyle$ = CSSFont.PlotTitle,
                         Optional valueFontStyle$ = CSSFont.Win7Bold,
                         Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                         Optional tick% = 50) As Bitmap

        Return profile.ProfilesPlot(
            title, axisTitle, colorSchema,
            bg, size, margin,
            classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
            tickFontStyle, tick)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="profile">原始的计数</param>
    ''' <param name="title$"></param>
    ''' <param name="axisTitle$"></param>
    ''' <param name="colorSchema$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="size"></param>
    ''' <param name="margin"></param>
    ''' <param name="classFontStyle$"></param>
    ''' <param name="catalogFontStyle$"></param>
    ''' <param name="titleFontStyle$"></param>
    ''' <param name="valueFontStyle$"></param>
    ''' <param name="tickFontStyle$"></param>
    ''' <param name="tick%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(profile As Dictionary(Of String, NamedValue(Of Integer)()),
                         Optional orderTakes% = -1,
                         Optional title$ = "Gene Ontology Profiling",
                         Optional axisTitle$ = "Number Of Gene",
                         Optional colorSchema$ = "Set1:c6",
                         Optional bg$ = "white",
                         Optional size As Size = Nothing,
                         Optional margin As Size = Nothing,
                         Optional classFontStyle$ = CSSFont.Win7LargerBold,
                         Optional catalogFontStyle$ = CSSFont.Win7Bold,
                         Optional titleFontStyle$ = CSSFont.PlotTitle,
                         Optional valueFontStyle$ = CSSFont.Win7Bold,
                         Optional tickFontStyle$ = CSSFont.Win7LargerBold,
                         Optional tick% = 50) As Bitmap
        Dim data As New Dictionary(Of String, NamedValue(Of Double)())

        For Each k In profile
            Dim x As NamedValue(Of Integer)() = k.Value

            If orderTakes > 0 Then
                Dim orders As NamedValue(Of Double)() = x _
                    .OrderByDescending(Function(o) o.Value) _
                    .Take(orderTakes) _
                    .ToArray(Function(o) New NamedValue(Of Double)(o.Description, o.Value))
                Call data.Add(k.Key, orders)
            Else
                Call data.Add(
                    k.Key,
                    x.ToArray(Function(o) New NamedValue(Of Double)(o.Description, o.Value)))
            End If
        Next

        Return data.ProfilesPlot(
            title, axisTitle, colorSchema,
            bg, size, margin,
            classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
            tickFontStyle, tick)
    End Function

    <Extension>
    Public Function EnrichmentPlot(data As IEnumerable(Of FunctionCluster), Optional size As Size = Nothing) As Bitmap
        Dim profile As New Dictionary(Of String, NamedValue(Of Double)())
        Dim g = From x As FunctionCluster
                In data
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
                .ToArray(Function(x) New NamedValue(Of Double) With {
                    .Name = x.Term,
                    .Value = -Math.Log10(x.PValue)
                })
        Next

        Return profile.ProfilesPlot(
            "GO enrichment",
            size:=size,
            axisTitle:="-Log10(p-value)",
            tick:=1)
    End Function
End Module
