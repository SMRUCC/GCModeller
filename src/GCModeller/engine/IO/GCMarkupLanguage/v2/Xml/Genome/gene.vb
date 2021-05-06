Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    Public Class gene

        <XmlAttribute> Public Property locus_tag As String
        <XmlAttribute> Public Property protein_id As String

        <XmlElement>
        Public Property product As String

        <XmlAttribute> Public Property left As Integer
        <XmlAttribute> Public Property right As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 因为字符类型在进行XML序列化的时候会被转换为ASCII代码，影响阅读
        ''' 所以在这里使用字符串类型来解决这个问题
        ''' </remarks>
        <XmlAttribute> Public Property strand As String

        ''' <summary>
        ''' 对于rRNA和tRNA不存在
        ''' </summary>
        ''' <returns></returns>
        Public Property amino_acid As NumericVector
        ''' <summary>
        ''' mRNA, tRNA, rRNA, etc
        ''' </summary>
        ''' <returns></returns>
        Public Property nucleotide_base As NumericVector

    End Class
End Namespace