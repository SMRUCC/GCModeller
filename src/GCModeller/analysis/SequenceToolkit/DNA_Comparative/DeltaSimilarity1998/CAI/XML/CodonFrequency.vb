﻿#Region "Microsoft.VisualBasic::46f84ecd782672dc420e0637ae44da2f, analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\CAI\XML\CodonFrequency.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class CodonFrequencyCAI
    ' 
    '         Properties: AminoAcid, BiasFrequency, BiasFrequencyProfile, MaxBias
    ' 
    '         Function: ToString
    ' 
    '     Structure CodonBias
    ' 
    '         Properties: Bias, Codon, CodonString
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace DeltaSimilarity1998.CAI.XML

    Public Class CodonFrequencyCAI

        ''' <summary>
        ''' 应该是<see cref="Char"/>类型，但是XML序列化之后会变为ASCII值，所以在这里使用字符串类型
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property AminoAcid As String

        ''' <summary>
        ''' {编码当前的氨基酸<see cref="AminoAcid"></see>的密码子, 在当前的基因之中的使用频率}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BiasFrequencyProfile As CodonBiasVector()
        ''' <summary>
        ''' Value为经过欧几里得距离归一化处理之后的计算结果
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BiasFrequency As CodonBias()

        Public ReadOnly Property MaxBias As CodonBias
            Get
                Return BiasFrequency _
                    .OrderByDescending(Function(c) c.Bias) _
                    .First
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Structure CodonBias

        <XmlAttribute> Public Property Codon As DNA()
        <XmlAttribute> Public Property Bias As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="codon$">三联体密码子字符串</param>
        ''' <param name="bias#"></param>
        Sub New(codon$, bias#)
            Me.Bias = bias
            Me.Codon = codon _
                .Select(AddressOf Conversion.CharEnums) _
                .ToArray
        End Sub

        Public ReadOnly Property CodonString As String
            Get
                Return NucleicAcid.ToString(Codon)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
