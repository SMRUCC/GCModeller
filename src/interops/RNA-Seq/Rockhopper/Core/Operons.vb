' /********************************************************************************/
'
'  Rockhopper —— 操纵子模型
'
'  复刻自原始 Rockhopper `Java/ObjectModels/Operons.vb` 的公开模型：
'    * Operon           —— 一个多基因操纵子（链方向、起止、基因列表）
'    * OperonGenePair   —— 相邻基因对的共转录证据（基因间距 + 表达相似度 + 后验概率）
'
'  预测算法见 `Operons/OperonPrediction.vb`。
'  ToString 的输出格式被 FileIO.ResultWriter.WriteOperons 与 API 层回读逻辑依赖。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace Core

    ''' <summary>
    ''' 一个多基因操纵子。
    ''' </summary>
    Public Class Operon

        ''' <summary>操纵子在基因组上的起始坐标。</summary>
        Public Property Start As Long
        ''' <summary>操纵子在基因组上的终止坐标。</summary>
        Public Property [Stop] As Long
        ''' <summary>链方向："+" / "-"。</summary>
        Public Property Strand As String

        ''' <summary>组成该操纵子的基因号（按转录顺序）。</summary>
        <Collection("Genes", ", ")> Public Property Genes As String()

        ''' <summary>操纵子包含的基因数。</summary>
        Public ReadOnly Property NumberOfGenes As Integer
            Get
                Return If(Genes Is Nothing, 0, Genes.Length)
            End Get
        End Property

        ''' <summary>操纵子编号（对应 DOOR 的 OperonID 语义，可为空）。</summary>
        Public Property OperonID As String

        Public Overrides Function ToString() As String
            Return $"[{Strand}]{Start},{[Stop]};    {String.Join(", ", If(Genes, New String() {}))}"
        End Function

    End Class

    ''' <summary>
    ''' 相邻基因对的共转录证据。
    ''' </summary>
    Public Class OperonGenePair

        ''' <summary>上游基因号。</summary>
        Public Property Gene1 As String
        ''' <summary>下游基因号。</summary>
        Public Property Gene2 As String
        ''' <summary>两基因之间的基因间区长度（bp）。</summary>
        Public Property Distance As Integer
        ''' <summary>跨样品的表达谱相似度（皮尔逊相关系数，[-1,1]）。</summary>
        Public Property ExpressionSimilarity As Double
        ''' <summary>共转录后验概率 [0,1]。</summary>
        Public Property Probability As Double
        ''' <summary>是否判定为同一操纵子。</summary>
        Public Property IsOperon As Boolean

        Public Overrides Function ToString() As String
            Return $"{Gene1}{vbTab}{Gene2}{vbTab}{Distance}{vbTab}{ExpressionSimilarity:F4}{vbTab}{Probability:F4}{vbTab}{IsOperon}"
        End Function

    End Class

End Namespace
