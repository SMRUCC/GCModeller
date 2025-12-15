
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Math.Statistics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' limma ``topTable`` output dataframe.
''' </summary>
Public Class LimmaTable : Implements IDeg, INamedValue, IReadOnlyId, IStatPvalue

    ''' <summary>
    ''' row names - gene id
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String Implements IDeg.label, INamedValue.Key, IReadOnlyId.Identity
    Public Property logFC As Double Implements IDeg.log2FC
    Public Property AveExpr As Double
    Public Property t As Double

    <Column("P.Value")>
    Public Property P_Value As Double Implements IDeg.pvalue, IStatPvalue.pValue
    <Column("adj.P.Val")>
    Public Property adj_P_Val As Double

    ''' <summary>
    ''' B (B-statistic)
    ''' 
    ''' B统计量或log-odds，表示基因是差异表达的概率的对数；计算公式：B = log10( (1 - p) / p ) 其中p是基因是差异表达的概率，基于经验贝叶斯方法估计；
    ''' B统计量的数学意义：表示"基因差异表达的可信度"（B > 0表示有差异表达证据，B > 1表示强证据）。
    ''' </summary>
    ''' <returns></returns>
    Public Property B As Double

    Public Property [class] As String

    Public Overrides Function ToString() As String
        Return $"{id} - logfc:{logFC}, p-value={P_Value}"
    End Function

    Public Shared Iterator Function LoadTable(filepath As String) As IEnumerable(Of LimmaTable)
        Dim lines As String() = filepath.LineIterators(strictFile:=True).ToArray
        Dim cols As Index(Of String) = Document.ParseHeaders(lines(Scan0)).Indexing(base:=0)
        Dim logFC As Integer = cols("logFC")
        Dim AveExpr As Integer = cols("AveExpr")
        Dim t As Integer = cols("t")
        Dim P_Value As Integer = cols("P.Value")
        Dim adj_P_Val As Integer = cols("adj.P.Val")
        Dim B As Integer = cols("B")

        For Each line As String In lines.Skip(1)
            Dim vec As NamedValue(Of Double()) = Reader.ParseGeneRowTokens(line)
            Dim data As Double() = CType(vec, Double())
            Dim expr As New LimmaTable With {
                .id = vec.Name,
                .logFC = If(logFC > -1, data(logFC), 0),
                .adj_P_Val = If(adj_P_Val > -1, data(adj_P_Val), 1),
                .AveExpr = If(AveExpr > -1, data(AveExpr), 0),
                .B = If(B > -1, data(B), 0),
                .P_Value = If(P_Value > -1, data(P_Value), 1),
                .t = If(t > -1, data(t), 0)
            }

            Yield expr
        Next
    End Function

End Class
