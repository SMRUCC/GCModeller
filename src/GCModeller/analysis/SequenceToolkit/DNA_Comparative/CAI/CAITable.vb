Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Public Class CAITable : Inherits ClassObject

    <XmlAttribute> Public Property CAI As Double
    Public Property BiasTable As KeyValuePairObject(Of Char, CodonFrequencyCAI)()
        Get
            Return _biasTable
        End Get
        Set(value As KeyValuePairObject(Of Char, CodonFrequencyCAI)())
            _biasTable = value
            _BiasList = (From codon In _biasTable
                         Select (From subitem In codon.Value.BiasFrequency
                                 Select New KeyValuePair(Of Char, KeyValuePairObject(Of Codon, Double))(codon.Key, New KeyValuePairObject(Of Codon, Double)(subitem.Key, subitem.Value)))).MatrixToVector
        End Set
    End Property

    Dim _biasTable As KeyValuePairObject(Of Char, CodonFrequencyCAI)()

    ''' <summary>
    ''' 对<see cref="BiasTable"></see>进行展开
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property BiasList As KeyValuePair(Of Char, KeyValuePairObject(Of Codon, Double))()

    Sub New()
    End Sub

    Sub New(Model As RelativeCodonBiases)
        CAI = Model.CAI
        BiasTable = LinqAPI.Exec(Of KeyValuePairObject(Of Char, CodonFrequencyCAI)) <=
            From item As KeyValuePair(Of Char, CodonFrequency)
            In Model.CodonFrequencyStatics
            Select New KeyValuePairObject(Of Char, CodonFrequencyCAI) With {
                .Key = item.Key,
                .Value = New CodonFrequencyCAI With {
                    .AminoAcid = item.Value.AminoAcid,
                    .BiasFrequency = (From c As KeyValuePair(Of Codon, Double)
                                      In item.Value.BiasFrequency
                                      Select New KeyValuePairObject(Of Codon, Double)(c.Key, c.Value)).ToArray,
                    .BiasFrequencyProfile = (From c
                                             In item.Value.BiasFrequencyProfile
                                             Select New KeyValuePairObject(Of Codon, TripleKeyValuesPair(Of Double))(c.Key, c.Value)).ToArray,
                    .MaxBias = New KeyValuePairObject(Of Codon, Double)(item.Value.MaxBias.Key, item.Value.MaxBias.Value)
                    }
                }
    End Sub
End Class

