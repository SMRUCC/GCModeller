Imports System.Xml.Serialization
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels.Translation
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Structure CodonFrequency

    Public Property AminoAcid As Char

    ''' <summary>
    ''' {编码当前的氨基酸<see cref="AminoAcid"></see>的密码子, 在当前的基因之中的使用频率}
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BiasFrequencyProfile As KeyValuePair(Of Codon, TripleKeyValuesPair(Of Double))()
    ''' <summary>
    ''' Value为经过欧几里得距离归一化处理之后的计算结果
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BiasFrequency As Dictionary(Of Codon, Double)
    Public Property MaxBias As KeyValuePair(Of Codon, Double)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure
