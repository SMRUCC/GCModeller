Imports System.Xml.Serialization

Namespace v2

    Public Class Genome

        ''' <summary>
        ''' 基因列表
        ''' </summary>
        ''' <returns></returns>
        Public Property genes As Gene()
        ''' <summary>
        ''' 转录调控网络
        ''' </summary>
        ''' <returns></returns>
        Public Property regulations As TranscriptionRegulation()
    End Class

    Public Class Gene

        <XmlAttribute> Public Property locus_tag As String
        <XmlAttribute> Public Property protein_id As String

        <XmlText>
        Public Property product As String

        <XmlAttribute> Public Property left As Integer
        <XmlAttribute> Public Property right As Integer
        <XmlAttribute> Public Property strand As Char

    End Class

    Public Class TranscriptionRegulation

        <XmlAttribute> Public Property regulator As String
        <XmlAttribute> Public Property target As String

        Public Property effector As String
        Public Property mode As String
        Public Property distance As Integer
        Public Property biological_process As String

        Public Property motif As Motif

    End Class

    ''' <summary>
    ''' 调控的motif位点
    ''' </summary>
    Public Class Motif

        <XmlAttribute> Public Property family As String
        <XmlAttribute> Public Property left As Integer
        <XmlAttribute> Public Property right As Integer
        <XmlAttribute> Public Property strand As Char

        <XmlText> Public Property sequence As String
    End Class
End Namespace