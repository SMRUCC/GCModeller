' /********************************************************************************/
'
'  Rockhopper —— 转录本视图模型（CSV 表格）
'
'  由原始 `API/TranscriptView.vb` 迁移而来，仅把 Column 特性迁移到
'  `Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection`。
'  该模型用于与 TSS 预测结果（如 sRNA / asRNA 注释表）做外部交换。
'
' /********************************************************************************/

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace AnalysisAPI

    ''' <summary>
    ''' 外部 TSS 注释表的一行。
    ''' </summary>
    Public Class TranscriptView

#Region "TSS INFORMATION"

        Public Property TSS_id As String

        ''' <summary>TSS 起始位置。</summary>
        Public Property pStart As Integer
        ''' <summary>TSS 终止位置。</summary>
        Public Property pStop As Integer

        Public Property Strand As String
        Public Property Replicon As String

        ''' <summary>TSS 分类（对应 <see cref="Transcripts.Categories"/>）。</summary>
        Public Property Category As String
        ''' <summary>asRNA 的子类别。</summary>
        <Column("Subcategory of asRNA")> Public Property Subcategory As String
        ''' <summary>前 3 个碱基。</summary>
        <Column("First 3 nt")> Public Property First As String
        ''' <summary>到起始密码子的距离。</summary>
        <Column("Distance to start codon")> Public Property ATGDistance As String
        Public Property Coverage As String

#End Region

#Region "ASSOCIATED GENE"

        <Column("Gene id")> Public Property GeneId As String
        ''' <summary>基因类型。</summary>
        <Column("Type of gene")> Public Property Type As String

        ''' <summary>关联基因的起始位置。</summary>
        Public Property gpStart As Integer
        ''' <summary>关联基因的终止位置。</summary>
        Public Property gpStop As Integer

        ''' <summary>基因产物（蛋白编码基因）。</summary>
        Public Property Product As String
        Public Property COG As String
        ''' <summary>asRNA 的靶标基因。</summary>
        <Column("Target gene of asRNA")> Public Property Target As String

        Public Property gStrand As String

#End Region

    End Class

End Namespace
