Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language

Namespace v2

    Public Class Genome

        ''' <summary>
        ''' 一个完整的基因组是由若干个复制子所构成的，复制子主要是指基因组和细菌的质粒基因组
        ''' </summary>
        ''' <returns></returns>
        Public Property replicons As replicon()

    End Class

    ''' <summary>
    ''' 复制子
    ''' </summary>
    Public Class replicon

        ''' <summary>
        ''' 当前的这个复制子对象是否是质粒基因组？
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property isPlasmid As Boolean
        <XmlAttribute> Public Property genomeName As String

        ''' <summary>
        ''' 基因列表，在这个属性之中定义了该基因组之中的所有基因的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property genes As gene()
        ''' <summary>
        ''' 转录调控网络
        ''' </summary>
        ''' <returns></returns>
        Public Property regulations As TranscriptionRegulation()

        Public Overrides Function ToString() As String
            Dim type$ = "Genome" Or "Plasmid genome".When(isPlasmid)
            Return $"[{type}] {genomeName}"
        End Function

    End Class

    Public Class gene

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