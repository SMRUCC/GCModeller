Imports System.Xml.Serialization

Namespace v2

    ''' <summary>
    ''' 基因表达转录调控
    ''' </summary>
    Public Class transcription

        <XmlAttribute> Public Property regulator As String

        <XmlAttribute> Public Property mode As String

        ''' <summary>
        ''' 这个是效应物物质编号列表
        ''' </summary>
        ''' <returns></returns>
        Public Property effector As String()
        ''' <summary>
        ''' 这个调控关系所影响到的中心法则的事件名称
        ''' </summary>
        ''' <returns></returns>
        Public Property centralDogma As String
        Public Property biological_process As String
        Public Property motif As Motif

        Public ReadOnly Property target As String
            Get
                Return centralDogma.Split.First
            End Get
        End Property

    End Class

    ''' <summary>
    ''' 调控的motif位点
    ''' </summary>
    Public Class Motif

        <XmlAttribute> Public Property family As String
        <XmlAttribute> Public Property left As Integer
        <XmlAttribute> Public Property right As Integer
        <XmlAttribute> Public Property strand As Char

        ''' <summary>
        ''' 这个motif位点到被调控的基因的ATG位点的最短距离
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("atg-distance")>
        Public Property distance As Integer

        <XmlText> Public Property sequence As String
    End Class
End Namespace