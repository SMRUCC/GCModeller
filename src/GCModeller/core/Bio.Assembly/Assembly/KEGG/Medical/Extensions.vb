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
    End Module
End Namespace