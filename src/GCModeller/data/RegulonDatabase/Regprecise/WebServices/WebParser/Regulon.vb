Imports System.Text.RegularExpressions
Imports System.Xml.Serialization

Namespace Regprecise

    Public Class Regulon

        <XmlElement> Public Property Regulators As Regulator()

    End Class

    Public Class RegulatedGene

        <XmlAttribute> Public Property vimssId As String
        <XmlAttribute> Public Property LocusId As String
        <XmlAttribute> Public Property Name As String
        <XmlAttribute> Public Property [Function] As String

        Public Overrides Function ToString() As String
            Return vimssId & vbTab & LocusId & vbTab & Name
        End Function

        ''' <summary>
        ''' 从文件系统上面的一个文本文件之中解析出基因的摘要数据
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function ParseDoc(path As String) As RegulatedGene()
            Dim lines As String() = IO.File.ReadAllLines(path)
            Return __parser(lines)
        End Function

        Private Shared Function __parser(lines As String()) As RegulatedGene()
            Dim LQuery = (From line As String In lines
                          Where Not String.IsNullOrEmpty(line)
                          Select __parser(line)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 解析文档文本
        ''' </summary>
        ''' <param name="doc"></param>
        ''' <returns></returns>
        Public Shared Function DocParser(doc As String) As RegulatedGene()
            Dim tokens As String() = doc.lTokens
            Return __parser(tokens)
        End Function

        Private Shared Function __parser(s As String) As RegulatedGene
            Dim Tokens As String() = Strings.Split(s, vbTab)
            Dim gene As New RegulatedGene With {
                .vimssId = Tokens.Get(Scan0),
                .LocusId = Tokens.Get(1),
                .Name = Tokens.Get(2)
            }
            Return gene
        End Function
    End Class
End Namespace