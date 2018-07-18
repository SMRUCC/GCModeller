#Region "Microsoft.VisualBasic::60906f1ea5c990eeb9f3e934c217bad8, Bio.Assembly\SequenceModel\Polypeptides\Chou-Fasman\ChouFasmanAPI.vb"

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

    '     Module ChouFasman
    ' 
    ' 
    '         Enum SecondaryStructures
    ' 
    '             AlphaHelix, BetaSheet, BetaTurn, Coils
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: __sequenceData, (+2 Overloads) Calculate, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace SequenceModel.Polypeptides.SecondaryStructure

    ''' <summary>
    ''' @ - <see cref="ChouFasman.SecondaryStructures.AlphaHelix"></see>;
    ''' 1 - <see cref="ChouFasman.SecondaryStructures.BetaSheet"></see>;
    ''' ^ - <see cref="ChouFasman.SecondaryStructures.BetaTurn"></see>;
    ''' &amp; - <see cref="ChouFasman.SecondaryStructures.Coils"></see>
    ''' </summary>
    ''' <remarks></remarks>
    Public Module ChouFasman

        ''' <summary>
        ''' 蛋白质的二级结构分类
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum SecondaryStructures

            ''' <summary>
            ''' alpha螺旋
            ''' </summary>
            ''' <remarks></remarks>
            AlphaHelix
            ''' <summary>
            ''' Beta折叠
            ''' </summary>
            ''' <remarks></remarks>
            BetaSheet
            ''' <summary>
            ''' Beta转角
            ''' </summary>
            ''' <remarks></remarks>
            BetaTurn
            ''' <summary>
            ''' 无规则卷曲
            ''' </summary>
            ''' <remarks></remarks>
            Coils
        End Enum

        Friend ReadOnly StructureTypesToChar As Dictionary(Of ChouFasman.SecondaryStructures, String) =
            New Dictionary(Of SecondaryStructures, String) From {
                {SecondaryStructures.AlphaHelix, "@"},
                {SecondaryStructures.BetaSheet, "-"},
                {SecondaryStructures.BetaTurn, "^"},
                {SecondaryStructures.Coils, "&"}
        }

        Private Function __sequenceData(SequenceData As String) As AminoAcid()
            Dim SequenceEnums = SequenceModel.Polypeptides.ConstructVector(SequenceData)
            Dim AA = (From Token In SequenceEnums Where Token <> SequenceModel.Polypeptides.AminoAcid.NULL Select New AminoAcid(Token)).ToArray

            If AA.Length < SequenceEnums.Length Then
                Call VBDebugger.Warning("There is illegal character contains in your protein sequence, they was removed:  " & SequenceData)
            End If

            Return AA
        End Function

        ''' <summary>
        ''' 使用<see cref="ChouFasman"></see>方法计算蛋白质的二级结构
        ''' </summary>
        ''' <param name="sequence">序列之中不能够包含有除了氨基酸的字符之外的任何其他字符，否则函数会出错</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Calculate(sequence As String) As AminoAcid()
            Dim SequenceData As AminoAcid() = __sequenceData(sequence)
            Dim AlphaMaskCounts = ChouFasmanRules.RuleAlphaHelix.Invoke(SequenceData)
            Dim BetaSheetMasks_ = ChouFasmanRules.RuleBetaSheet.Invoke(SequenceData)
            Dim BetaTurnMasks__ = ChouFasmanRules.RuleBetaTurn.Invoke(SequenceData)

            Call ChouFasmanRules.RuleOverlap.Invoke(SequenceData)

            Return SequenceData
        End Function

        ''' <summary>
        ''' 使用<see cref="ChouFasman"></see>方法计算蛋白质的二级结构
        ''' </summary>
        ''' <param name="sequence"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Calculate(sequence As IPolymerSequenceModel) As AminoAcid()
            Return ChouFasman.Calculate(sequence.SequenceData)
        End Function

        Public Function ToString(aa As AminoAcid()) As String
            Dim aa_Builder As StringBuilder = New StringBuilder(aa.Length - 1)
            Dim st_Builder As StringBuilder = New StringBuilder(aa.Length - 1)

            For Each residue As AminoAcid In aa
                Call aa_Builder.Append(SequenceModel.Polypeptides.Polypeptides.ToChar(residue.AminoAcid))
                Call st_Builder.Append(ChouFasman.StructureTypesToChar(residue.StructureType))
            Next

            Dim sBuilder As StringBuilder = New StringBuilder(String.Format("Key,Value" & vbCrLf & "SequenceData,{0}" & vbCrLf & "Structure,{1}", aa_Builder.ToString, st_Builder.ToString))
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine()
            For Each item As KeyValuePair(Of SecondaryStructures, String) In ChouFasman.StructureTypesToChar
                Call sBuilder.AppendLine(String.Format("{0},{1}", item.Key.ToString, item.Value))
            Next
            Return sBuilder.ToString
        End Function
    End Module
End Namespace
