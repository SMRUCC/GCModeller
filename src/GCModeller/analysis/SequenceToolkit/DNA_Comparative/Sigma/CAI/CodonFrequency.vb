#Region "Microsoft.VisualBasic::a835c642c93b1cc25df73fbc50010003, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\CAI\CodonFrequency.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
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

