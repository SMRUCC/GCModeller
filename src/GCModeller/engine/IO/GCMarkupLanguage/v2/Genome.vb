Imports System.Xml.Serialization

Namespace v2

    Public Class Genome

        ''' <summary>
        ''' 基因列表，在这个属性之中定义了该基因组之中的所有基因的数据
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
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 因为字符类型在进行XML序列化的时候会被转换为ASCII代码，影响阅读
        ''' 所以在这里使用字符串类型来解决这个问题
        ''' </remarks>
        <XmlAttribute> Public Property strand As String

    End Class

    Public Class TranscriptionRegulation

        <XmlAttribute> Public Property regulator As String
        <XmlAttribute> Public Property target As String
        <XmlAttribute> Public Property mode As String

        Public Property effector As String
        Public Property biological_process As String

        ''' <summary>
        ''' 这个调控关系所影响到的中心法则的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property centralDogma As String
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

        ''' <summary>
        ''' 这个motif位点到被调控的基因的ATG位点的最短距离
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("atg-distance")>
        Public Property distance As Integer

        <XmlText> Public Property sequence As String
    End Class
End Namespace