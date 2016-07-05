#Region "Microsoft.VisualBasic::30d496760f8ece27aa6b383b3acf14bf, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\CAI\CodonFrequencyCAI.vb"

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
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
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

