Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.GeneOntology.GoStat
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Visualize.CatalogProfiling

Public Module CatalogPlots

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

        Dim data = annotations.CountStat(getGO, GO_terms) ' 返回来的是 Go_ID, label, count
        Dim profile As New Dictionary(Of String, NamedValue(Of Integer)())

        For Each [class] In data.Keys
            Dim counts = data([class])
            Dim stat As New List(Of NamedValue(Of Integer))

            For Each catalog In counts
                stat += New NamedValue(Of Integer) With {
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
    Public Function Plot(profile As Dictionary(Of String, NamedValue(Of Integer)()),
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
End Module
