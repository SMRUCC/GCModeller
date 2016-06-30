Imports LANS.SystemsBiology.Toolkits.RNA_Seq.dataExprMAT
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace DESeq2

    ''' <summary>
    ''' 原始输出数据
    ''' </summary>
    Public Class ResultData : Inherits DESeq2Diff
        Implements IExprMAT
        Implements sIdEnumerable

        <Meta(GetType(Double))>
        Public Property dataExpr0 As Dictionary(Of String, Double) Implements IExprMAT.dataExpr0
            Get
                If _dataExpr0 Is Nothing Then
                    _dataExpr0 = New Dictionary(Of String, Double)
                End If
                Return _dataExpr0
            End Get
            Set(value As Dictionary(Of String, Double))
                _dataExpr0 = value
                If _dataExpr0.ContainsKey("") Then  ' 会有一个空格部分，不清楚为什么
                    Call _dataExpr0.Remove("")
                End If
            End Set
        End Property

        Dim _dataExpr0 As Dictionary(Of String, Double)

        Public Overrides Property locus_tag As String Implements IExprMAT.LocusId

        Public Overrides Function ToString() As String
            Return $"{locus_tag} ---> log2FoldChange  {log2FoldChange}"
        End Function

        Public Function ToSample() As ExprMAT
            Return New ExprMAT With {
                .LocusId = Me.locus_tag,
                .dataExpr0 = dataExpr0
            }
        End Function
    End Class
End Namespace