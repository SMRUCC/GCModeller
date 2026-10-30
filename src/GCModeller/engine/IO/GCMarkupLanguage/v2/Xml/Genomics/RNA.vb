Imports System.Xml.Serialization
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

Namespace v2

    ''' <summary>
    ''' 只记录tRNA，rRNA和其他RNA的数据，对于mRNA则不做记录
    ''' </summary>
    Public Class RNA

        <XmlAttribute> Public Property id As String

        ''' <summary>
        ''' the trranscription source template gene <see cref="v2.gene.locus_tag"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property gene As String
        ''' <summary>
        ''' the rna type
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property type As RNATypes
        ''' <summary>
        ''' usually be the:
        ''' 
        ''' 1. amino acid code for tRNA
        ''' 2. 16s,5s,23s for rRNA
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property val As String

        Public Property note As String

        ''' <summary>
        ''' 当前RNA分子的组成成分(对于复合物类型的RNA)
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("component")> Public Property components As String()

        ''' <summary>
        ''' 当前RNA分子所属的父对象编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property componentOf As String

        ''' <summary>
        ''' RNA分子在基因组上面的终止位置
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property endPosition As Integer

        ''' <summary>
        ''' RNA分子的交叉引用编号列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("db_xref")> Public Property dbLinks As String()

        Sub New()
        End Sub

        Sub New(gene_id As String, type As RNATypes, val As String)
            Me.gene = gene_id
            Me.type = type
            Me.val = val
        End Sub

        Public Overrides Function ToString() As String
            Return $"{gene} ({type}); ""{val}"""
        End Function

    End Class
End Namespace