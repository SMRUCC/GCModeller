Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' The publication records of the BioCyc database (``pubs.dat``).
''' 
''' 这个模型仅仅保留用于引用索引的元数据（标题、作者、年分以及PubMed/DOI编号），
''' 不保留文献的摘要与正文内容，避免虚拟细胞模型文件体积过度膨胀。
''' </summary>
<Xref("pubs.dat")>
Public Class pubs : Inherits Model

    <AttributeField("TITLE")> Public Property title As String
    <AttributeField("AUTHORS")> Public Property authors As String()
    <AttributeField("YEAR")> Public Property year As Integer
    <AttributeField("SOURCE")> Public Property source As String
    <AttributeField("PUBMED-ID")> Public Property pubmedId As String
    <AttributeField("DOI-ID")> Public Property doiId As String
    <AttributeField("MESH-HEADINGS")> Public Property meshHeadings As String()
    <AttributeField("URL")> Public Property url As String

    Public Overrides Function ToString() As String
        Return $"{pubmedId} - {title}"
    End Function

End Class
