Imports System.Xml.Serialization
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.ComponentModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DocumentFormat.MEME.LDM

    ''' <summary>
    ''' Motif data from the text format output file of the meme program.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Motif

        ''' <summary>
        ''' 进行一些绘图操作的时候可能需要使用到这个属性，一般情况之下这个属性只是用作于一些用户的自定义数据，不太重要
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property uid As String
        <XmlAttribute> Public Property Id As String
        <XmlAttribute> Public Property Width As Integer
        <XmlAttribute> Public Property llr As Integer
        <XmlAttribute> Public Property Evalue As Double

        ''' <summary>
        ''' Simplified pos.-specific probability matrix
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PspMatrix As MotifPM()
        '<XmlAttribute> Public Property RelativeEntropy As Double

        ''' <summary>
        ''' Multilevel consensus sequence
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Mcs As String()

        ''' <summary>
        ''' 产生这个Motif的序列的集合
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("Motif.Sites", Namespace:="http://code.google.com/p/genome-in-code/meme/motif.matrix")>
        Public Property Sites As Site()
        ''' <summary>
        ''' 使用这个正则表达式来描述当前的这个Motif的特征
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("Motif.Signature")> Public Property Signature As String

        ''' <summary>
        ''' 是否为核酸类型的Motif位点，TRUE为是，FALSE为蛋白质序列的Motif
        ''' </summary>
        ''' <returns></returns>
        Public Property NtMolType As Boolean

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace