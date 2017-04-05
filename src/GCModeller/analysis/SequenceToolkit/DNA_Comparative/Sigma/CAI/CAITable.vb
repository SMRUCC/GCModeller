#Region "Microsoft.VisualBasic::8d506a6173ccf97feefabaa003cc3863, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\Sigma\CAI\CAITable.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation

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
                                 Select New KeyValuePair(Of Char, KeyValuePairObject(Of Codon, Double))(codon.Key, New KeyValuePairObject(Of Codon, Double)(subitem.Key, subitem.Value)))).ToVector
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
