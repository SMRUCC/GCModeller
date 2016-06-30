Imports System.Xml.Serialization

Namespace SearchingModel

    ''' <summary>
    ''' 重复片段搜索程序的参数设置对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure KmerProfile

        ''' <summary>
        ''' Succession of similarly spaced repeats of length k..(CRISPR片段的长度)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Dim k As Integer

        <XmlAttribute> Dim minR, maxR As Integer
        <XmlAttribute> Dim minS, maxS As Integer

        Public Overrides Function ToString() As String
            Return String.Format("[i + minR:={0} + minS:={1} .. i + maxR:={2} + maxS:={3} + k:={3}]", minR, minS, maxR, maxS, k)
        End Function
    End Structure
End Namespace