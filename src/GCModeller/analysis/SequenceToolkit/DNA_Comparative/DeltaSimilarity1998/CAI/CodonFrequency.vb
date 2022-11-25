#Region "Microsoft.VisualBasic::7745bd51264d796e5dd84e5f4da03e0d, GCModeller\analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\CAI\CodonFrequency.vb"

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


    ' Code Statistics:

    '   Total Lines: 31
    '    Code Lines: 14
    ' Comment Lines: 12
    '   Blank Lines: 5
    '     File Size: 1.17 KB


    '     Structure CodonFrequency
    ' 
    '         Properties: AminoAcid, BiasFrequency, BiasFrequencyProfile, MaxBias
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation

Namespace DeltaSimilarity1998.CAI

    Public Structure CodonFrequency

        Public Property AminoAcid As Char

        ''' <summary>
        ''' {编码当前的氨基酸<see cref="AminoAcid"></see>的密码子, 在当前的基因之中的使用频率}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BiasFrequencyProfile As Dictionary(Of String, CodonBiasVector)
        ''' <summary>
        ''' Value为经过欧几里得距离归一化处理之后的计算结果，key为三联体密码子字符串
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BiasFrequency As Dictionary(Of String, Double)
        Public Property MaxBias As (codon$, bias#)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
