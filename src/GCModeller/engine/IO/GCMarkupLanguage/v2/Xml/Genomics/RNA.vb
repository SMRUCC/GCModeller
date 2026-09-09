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