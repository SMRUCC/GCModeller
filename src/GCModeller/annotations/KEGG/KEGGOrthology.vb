Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data

Public Module KEGGOrthology

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mappings">blast mapping数据</param>
    ''' <param name="KO">基因和KO之间的对应关系</param>
    ''' <param name="level$">统计的等级</param>
    ''' <returns></returns>
    <Extension>
    Public Function CatalogProfiling(mappings As IEnumerable(Of Map(Of String, String).IMap), KO As KO_gene(), Optional level$ = "A") As Dictionary(Of String, NamedValue(Of Integer)())
        Dim htext As htext = htext.ko00001
        Dim noMapping As Integer
        Dim out As New Dictionary(Of String, NamedValue(Of Integer)())
        Dim KO_genes As Dictionary(Of String, String()) = KO _
            .GroupBy(Function(x) $"{x.sp_code}:{x.gene}".ToLower) _
            .ToDictionary(Function(g) g.Key,
                          Function(k) k.Select(
                          Function(x) x.ko).Distinct.ToArray)


        Call out.Add(
            "NOT_ASSIGNED", {
                New NamedValue(Of Integer) With {
                    .Name = NameOf(noMapping),
                    .Value = noMapping
                }
            })

        Return out
    End Function
End Module
