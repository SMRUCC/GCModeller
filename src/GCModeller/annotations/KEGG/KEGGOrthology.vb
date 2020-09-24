#Region "Microsoft.VisualBasic::aaa5a487b7eb6302a1b198d1833128f8, annotations\KEGG\KEGGOrthology.vb"

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

    ' Module KEGGOrthology
    ' 
    '     Function: __profiles, CatalogProfiling, (+2 Overloads) KEGGEnrichmentPlot, KEGGPathwayEnrichmentProfile, Plot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Visualize.CatalogProfiling

Public Module KEGGOrthology

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mappings">blast mapping数据</param>
    ''' <param name="KO">基因和KO之间的对应关系</param>
    ''' <param name="level$">统计的等级</param>
    ''' <returns></returns>
    <Extension>
    Public Function CatalogProfiling(Of T As Map(Of String, String).IMap)(mappings As IEnumerable(Of T), KO As KO_gene(), Optional level$ = "A") As Dictionary(Of String, NamedValue(Of Integer)())
        Dim htext As htext = htext.ko00001
        Dim noMapping As Integer
        Dim out As New Dictionary(Of String, NamedValue(Of Integer)())
        Dim KO_genes As Dictionary(Of String, String()) = KO _
            .GroupBy(Function(x) $"{x.sp_code}:{x.gene}".ToLower) _
            .ToDictionary(Function(g) g.Key,
                          Function(k) k.Select(
                          Function(x) x.ko).Distinct.ToArray)
        Dim mappingGenes As Dictionary(Of String, Integer) =
            mappings _
            .GroupBy(Function(x) x.Maps.ToLower) _
            .ToDictionary(Function(k) k.Key,
                          Function(n) n.Count)
        Dim l As Char = level.First
        Dim KOcounts As New Dictionary(Of String, Integer)

        For Each hit In mappingGenes
            If KO_genes.ContainsKey(hit.Key) Then
                Dim k As String() = KO_genes(hit.Key)

                For Each ko_num$ In k
                    If Not KOcounts.ContainsKey(ko_num) Then
                        KOcounts.Add(ko_num, 0)
                    End If
                    KOcounts(ko_num) += hit.Value
                Next
            Else
                noMapping += hit.Value
            End If
        Next

        For Each A As BriteHText In htext.Hierarchical.categoryItems
            Call out.Add(A.classLabel, A.__profiles(KOcounts, level))
        Next

        Call out.Add(
            NOT_ASSIGN, {
                New NamedValue(Of Integer) With {
                    .Name = NOT_ASSIGN,
                    .Value = noMapping
                }
            })

        Return out
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="htext"></param>
    ''' <param name="KOcounts"></param>
    ''' <param name="level"></param>
    <Extension>
    Private Function __profiles(htext As BriteHText,
                                KOcounts As Dictionary(Of String, Integer),
                                level As Char) As NamedValue(Of Integer)()

        Dim out As New List(Of NamedValue(Of Integer))

        If htext.CategoryLevel = level Then  ' 将本对象之中的所有sub都进行求和
            For Each [sub] As BriteHText In htext.categoryItems.SafeQuery
                Dim counts As Integer = [sub] _
                    .EnumerateEntries _
                    .Where(Function(k) Not k.entryID Is Nothing) _
                    .Sum(Function(ko) If(
                        KOcounts.ContainsKey(ko.entryID),
                        KOcounts(ko.entryID), 0))

                out += New NamedValue(Of Integer) With {
                    .Name = [sub].classLabel,
                    .Value = counts
                }
            Next
        Else
            For Each [sub] As BriteHText In htext.categoryItems
                out += [sub].__profiles(KOcounts, level)
            Next
        End If

        Return out
    End Function

    ''' <summary>
    ''' KEGG Orthology Profiling Bar Plot
    ''' </summary>
    ''' <param name="profile"></param>
    ''' <param name="title$"></param>
    ''' <param name="colorSchema$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="size"></param>
    ''' <param name="classFontStyle$"></param>
    ''' <param name="catalogFontStyle$"></param>
    ''' <param name="titleFontStyle$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(profile As Dictionary(Of String, NamedValue(Of Double)()),
                         Optional title$ = "KEGG Orthology Profiling",
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

        Static KO_class$() = {
            "Cellular Processes",
            "Environmental Information Processing",
            "Genetic Information Processing",
            "Human Diseases",
            "Metabolism",
            "Organismal Systems"
        }

        Return profile.ProfilesPlot(
            title, axisTitle,
            colorSchema, bg,
            size, padding,
            classFontStyle, catalogFontStyle, titleFontStyle, valueFontStyle,
            tickFontStyle, tick)
    End Function

    Const Other$ = NameOf(Other)

    ''' <summary>
    ''' 进行DAVID分析结果的KEGG代谢途径富集的计算绘图
    ''' </summary>
    ''' <param name="result"></param>
    ''' <param name="size"></param>
    ''' <returns></returns>
    <Extension>
    Public Function KEGGEnrichmentPlot(result As IEnumerable(Of FunctionCluster),
                                       Optional KEGG As Dictionary(Of String, BriteHText) = Nothing,
                                       Optional size$ = "2200,2000",
                                       Optional tick# = 1,
                                       Optional pvalue# = 0.05,
                                       Optional colorSchema$ = "Set1:c6") As GraphicsData

        Dim data As New Dictionary(Of String, List(Of NamedValue(Of Double)))

        For Each term As FunctionCluster In result.Where(Function(x) x.PValue <= pvalue)
            Dim P# = -Math.Log10(term.PValue)

            With term.Term.GetTagValue(":", trim:=True)
                If Not KEGG Is Nothing AndAlso KEGG.ContainsKey(.Name) Then
                    Dim class$ = KEGG(.Name).parent.parent.description

                    If Not data.ContainsKey([class]) Then
                        data.Add([class], New List(Of NamedValue(Of Double)))
                    End If

                    data([class]) +=
                        New NamedValue(Of Double)(.Value, P#)
                Else
                    If Not data.ContainsKey(Other) Then
                        data.Add(Other, New List(Of NamedValue(Of Double)))
                    End If

                    data!Other += New NamedValue(Of Double)(term.Term, P#)
                End If
            End With
        Next

        If data.Count = 1 AndAlso data.Keys.First = Other Then
            data.Add("KEGG pathway", data!Other)
            data.Remove(Other)
        Else
            If data.Count > 6 Then
                colorSchema = "Set1:c8"
            End If
        End If

        Dim profile = data.ToDictionary(
            Function([class]) [class].Key,
            Function(terms)
                Return terms.Value _
                    .OrderByDescending(Function(t) t.Value) _
                    .ToArray
            End Function)

        Return profile.ProfilesPlot(
            title:="KEGG Pathway enrichment",
            size:=size,
            axisTitle:="-Log10(p-value)",
            tick:=tick,
            colorSchema:=colorSchema
        )
    End Function

    <Extension>
    Public Iterator Function KEGGPathwayEnrichmentProfile(result As IEnumerable(Of EnrichmentTerm)) As IEnumerable(Of EntityObject)
        Dim pathwayBrite = Pathway.LoadDictionary
        Dim catalog As Pathway

        For Each term As EnrichmentTerm In result
            catalog = pathwayBrite.GetPathwayBrite(term.ID)

            If catalog Is Nothing Then
                catalog = New Pathway With {
                    .category = "Unclassified",
                    .class = .category
                }
            End If

            Yield New EntityObject With {
                .ID = term.ID,
                .Properties = New Dictionary(Of String, String) From {
                    {"name", term.Term},
                    {"class", catalog.class},
                    {"category", catalog.category},
                    {"pvalue", term.Pvalue}，
                    {"num_inputs", term.number},
                    {"input_genes", term.Input}
                }
            }
        Next
    End Function

    <Extension>
    Public Function KEGGEnrichmentPlot(result As IEnumerable(Of EnrichmentTerm),
                                       Optional size$ = "2200,2000",
                                       Optional pvalue# = 0.05,
                                       Optional tick# = 1,
                                       Optional gray As Boolean = False,
                                       Optional labelRightAlignment As Boolean = False,
                                       Optional topN As Integer = 13,
                                       Optional colorSchema$ = "Set1:c6") As GraphicsData

        Dim pathwayBrite = Pathway.LoadDictionary
        Dim profiles As New Dictionary(Of String, List(Of NamedValue(Of Double)))
        Dim catalog As Pathway

        For Each term As EnrichmentTerm In result.Where(Function(x) x.Pvalue <= pvalue)
            catalog = pathwayBrite.GetPathwayBrite(term.ID)

            If catalog Is Nothing Then
                catalog = New Pathway With {
                    .category = "Unclassified",
                    .class = .category
                }
            End If

            If Not profiles.ContainsKey(catalog.class) Then
                Call profiles.Add(catalog.class, New List(Of NamedValue(Of Double)))
            End If

            Call profiles(catalog.class).Add(
                New NamedValue(Of Double) With {
                    .Name = term.Term,
                    .Value = -Math.Log10(term.Pvalue)
                })
        Next

        If profiles.Count = 0 Then
            Throw New DataException($"No enrichment terms for data plot after pvalue(<={pvalue}) cutoff filtering!")
        End If

        Dim profileData = profiles _
            .ToDictionary(Function(k) k.Key,
                          Function(terms) As NamedValue(Of Double)()
                              Return terms _
                                  .Value _
                                  .OrderByDescending(Function(t) t.Value) _
                                  .Take(topN) _
                                  .ToArray
                          End Function)
        Return profileData _
            .ProfilesPlot(title:="KEGG Pathway enrichment",
                          size:=size,
                          axisTitle:="-Log10(p-value)",
                          tick:=tick,
                          gray:=gray,
                          labelRightAlignment:=labelRightAlignment,
                          colorSchema:=colorSchema
            )
    End Function
End Module
