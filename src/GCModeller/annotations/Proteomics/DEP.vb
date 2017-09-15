Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Visualize

''' <summary>
''' iTraq的DEP分析结果输出的文件数据读取对象
''' 
''' > https://github.com/xieguigang/GCModeller.cli2R/blob/master/R/iTraq.log2_t-test.R
''' </summary>
Public Class DEP_iTraq : Inherits EntityObject
    Implements IDeg

    Public Overrides Property ID As String Implements IDeg.label

    <Column("FC.avg")> Public Property FCavg As Double
    <Column("p.value")> Public Property pvalue As Double Implements IDeg.pvalue
    <Column("is.DEP")> Public Property isDEP As Boolean
    <Column("log2FC")> Public Property log2FC As Double Implements IDeg.log2FC
    <Column("FDR")> Public Property FDR As Double

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
