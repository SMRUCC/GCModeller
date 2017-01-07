Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data

Public Module KEGGOrthology

    Public Const NOT_ASSIGN As String = NameOf(NOT_ASSIGN)

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

        For Each A As BriteHText In htext.Hierarchical.CategoryItems
            Call out.Add(A.ClassLabel, A.__profiles(KOcounts, level))
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
            For Each [sub] As BriteHText In htext.CategoryItems.SafeQuery
                Dim counts As Integer = [sub] _
                    .EnumerateEntries _
                    .Where(Function(k) Not k.EntryId Is Nothing) _
                    .Sum(Function(ko) If(
                        KOcounts.ContainsKey(ko.EntryId),
                        KOcounts(ko.EntryId), 0))

                out += New NamedValue(Of Integer) With {
                    .Name = [sub].ClassLabel,
                    .Value = counts
                }
            Next
        Else
            For Each [sub] As BriteHText In htext.CategoryItems
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
    ''' <param name="margin"></param>
    ''' <param name="classFontStyle$"></param>
    ''' <param name="catalogFontStyle$"></param>
    ''' <param name="titleFontStyle$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(profile As Dictionary(Of String, NamedValue(Of Integer)()),
                         Optional title$ = "KEGG Orthology Profiling",
                         Optional colorSchema$ = "Paired:c6",
                         Optional bg$ = "white",
                         Optional size As Size = Nothing,
                         Optional margin As Size = Nothing,
                         Optional classFontStyle$ = CSSFont.Win7Normal,
                         Optional catalogFontStyle$ = CSSFont.Win10Normal,
                         Optional titleFontStyle$ = CSSFont.Win7Normal) As Bitmap

        Static KO_class$() = {
            "Cellular Processes",
            "Environmental Information Processing",
            "Genetic Information Processing",
            "Human Diseases",
            "Metabolism",
            "Organismal Systems"
        }

        Dim colors As Color() = Designer.FromSchema(colorSchema, profile.Count - 1)
        Dim mapper As New Scaling(
            profile _
            .Values _
            .Select(Function(c) c.Select(Function(v) CDbl(v.Value))) _
            .IteratesALL, horizontal:=True)

        Return g.GraphicsPlots(
            size, margin,
            bg,
            Sub(ByRef g, regiong)

                Dim titleFont As Font = CSSFont.TryParse(titleFontStyle).GDIObject
                Dim catalogFont As Font = CSSFont.TryParse(catalogFontStyle).GDIObject
                Dim classFont As Font = CSSFont.TryParse(classFontStyle).GDIObject
                Dim left! = 5, y! = 100
                Dim maxLenSubKey$ = profile _
                    .Values _
                    .Select(Function(o) o.Select(Function(oo) oo.Name)) _
                    .IteratesALL _
                    .OrderByDescending(Function(s) s.Length) _
                    .First
                Dim maxLenClsKey$ = KO_class _
                    .OrderByDescending(Function(s) s.Length) _
                    .First
                Dim maxLenSubKeySize As SizeF = g.MeasureString(maxLenSubKey, catalogFont)
                Dim maxLenClsKeySize As SizeF = g.MeasureString(maxLenClsKey, classFont)

                For Each class$ In KO_class

                Next

            End Sub)
    End Function
End Module
