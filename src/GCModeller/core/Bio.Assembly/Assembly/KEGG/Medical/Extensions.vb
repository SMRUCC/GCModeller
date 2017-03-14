Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.Medical

    Public Module Extensions

        Const hsa$ = "HSA[:]\d+(\s\d+)*"

        ''' <summary>
        ''' 从<see cref="Drug.Targets"/>属性之中解析出``geneID``列表
        ''' </summary>
        ''' <param name="drug"></param>
        ''' <returns></returns>
        <Extension>
        Public Function DrugTargetGenes(drug As Drug) As String()
            Dim geneTargets$() = drug.Targets _
                .SafeQuery _
                .Select(Function(gene) Regex.Match(gene, hsa, RegexICSng).Value) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray
            Dim geneIDs$() = geneTargets _
                .Select(Function(id) id.GetTagValue(":").Value.Split) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Return geneIDs
        End Function

        ''' <summary>
        ''' Using remarks the same as map drug to compounds
        ''' </summary>
        ''' <param name="drugs"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CompoundsDrugs(drugs As IEnumerable(Of Drug)) As Dictionary(Of String, Drug())
            Dim compoundDrugs = drugs _
                .Select(Function(dr) New Tuple(Of String, Drug)(dr.TheSameAs, dr)) _
                .Where(Function(dr) Not dr.Item1.StringEmpty) _
                .GroupBy(Function(k) k.Item1) _
                .ToDictionary(Function(k) k.Key,
                              Function(v) v.Select(Function(g) g.Item2) _
                                           .GroupBy(Function(d) d.Entry) _
                                           .Select(Function(dr) dr.First) _
                                           .ToArray)
            Return compoundDrugs
        End Function
    End Module
End Namespace