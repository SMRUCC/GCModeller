Imports System.Xml.Serialization
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels.Translation
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class CodonFrequencyCAI

    <XmlAttribute> Public Property AminoAcid As Char

    ''' <summary>
    ''' {编码当前的氨基酸<see cref="AminoAcid"></see>的密码子, 在当前的基因之中的使用频率}
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement> Public Property BiasFrequencyProfile As KeyValuePairObject(Of Codon, TripleKeyValuesPair(Of Double))()
    ''' <summary>
    ''' Value为经过欧几里得距离归一化处理之后的计算结果
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement> Public Property BiasFrequency As KeyValuePairObject(Of Codon, Double)()
    <XmlElement> Public Property MaxBias As KeyValuePairObject(Of Codon, Double)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
