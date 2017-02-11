Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.ComponentModel

Public Module COGCatalogProfiling

    <Extension>
    Public Function Plot(Of T As ICOGCatalog)(genes As IEnumerable(Of T),
                                              Optional size As Size = Nothing,
                                              Optional bg$ = "white",
                                              Optional title$ = "COG catalog profiling") As Bitmap

        Dim COGs As COG.Function = COG.Function.Default
        Dim array As T() = genes.ToArray
        Dim profiling = From c As Char
                        In array _
                            .Select(Function(g) g.Catalog) _
                            .Where(Function(s) Not s.IsBlank) _
                            .IteratesALL
                        Select c
                        Group c By c Into Count  ' 所有的元素经过分组操作之后都是唯一的
        Dim profiles As New Dictionary(Of String, NamedValue(Of Double)())
        Dim data As New Dictionary(Of String, List(Of NamedValue(Of Double)))
        Dim null% = array.Where(Function(g) g.Catalog.IsBlank).Count ' 空的分类的基因数目
        Dim profileData = profiling.ToArray
        Dim total% = array.Length

        For Each catalog In profileData
            Dim [class] = COGs.GetCatalog(CStr(catalog.c))

            If Not data.ContainsKey([class].Name) Then
                Call data.Add([class].Name, New List(Of NamedValue(Of Double)))
            End If

            Call data([class].Name).Add(
                New NamedValue(Of Double) With {
                    .Name = $"[{catalog.c}] {[class].Value}",
                    .Value = catalog.Count / total
                })
        Next

        For Each catalog In data
            Call profiles.Add(catalog.Key, catalog.Value)
        Next

        Return profiles.ProfilesPlot(
            title:=title,
            bg:=bg,
            size:=size)
    End Function
End Module
