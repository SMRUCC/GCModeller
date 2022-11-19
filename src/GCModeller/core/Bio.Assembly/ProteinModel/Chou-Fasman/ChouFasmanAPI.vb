#Region "Microsoft.VisualBasic::190c49d4307260a1cd38b79bb3e83024, GCModeller\core\Bio.Assembly\ProteinModel\Chou-Fasman\ChouFasmanAPI.vb"

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

    '   Total Lines: 82
    '    Code Lines: 49
    ' Comment Lines: 20
    '   Blank Lines: 13
    '     File Size: 3.69 KB


    '     Module ChouFasman
    ' 
    '         Function: __sequenceData, (+2 Overloads) Calculate, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace ProteinModel.ChouFasmanRules

    ''' <summary>
    ''' @ - <see cref="SecondaryStructures.AlphaHelix"></see>;
    ''' 1 - <see cref="SecondaryStructures.BetaSheet"></see>;
    ''' ^ - <see cref="SecondaryStructures.BetaTurn"></see>;
    ''' &amp; - <see cref="SecondaryStructures.Coils"></see>
    ''' </summary>
    ''' <remarks></remarks>
    Public Module ChouFasman

        Friend ReadOnly StructureTypesToChar As New Dictionary(Of SecondaryStructures, String) From {
            {SecondaryStructures.AlphaHelix, "@"},
            {SecondaryStructures.BetaSheet, "-"},
            {SecondaryStructures.BetaTurn, "^"},
            {SecondaryStructures.Coils, "&"}
        }

        Private Function __sequenceData(SequenceData As String) As AminoAcid()
            Dim SequenceEnums = Polypeptide.ConstructVector(SequenceData).ToArray
            Dim AA = (From a In SequenceEnums Where a <> SequenceModel.Polypeptides.AminoAcid.NULL Select New AminoAcid(a)).ToArray

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
            Dim AlphaMaskCounts = Rules.RuleAlphaHelix.Invoke(SequenceData)
            Dim BetaSheetMasks_ = Rules.RuleBetaSheet.Invoke(SequenceData)
            Dim BetaTurnMasks__ = Rules.RuleBetaTurn.Invoke(SequenceData)

            Call Rules.RuleOverlap.Invoke(SequenceData)

            Return SequenceData
        End Function

        ''' <summary>
        ''' 使用<see cref="ChouFasman"></see>方法计算蛋白质的二级结构
        ''' </summary>
        ''' <param name="sequence"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Calculate(sequence As IPolymerSequenceModel) As AminoAcid()
            Return ChouFasman.Calculate(sequence.SequenceData)
        End Function

        Public Function ToString(aa As AminoAcid()) As String
            Dim aa_Builder As New StringBuilder(aa.Length - 1)
            Dim st_Builder As New StringBuilder(aa.Length - 1)

            For Each residue As AminoAcid In aa
                Call aa_Builder.Append(Polypeptide.ToChar(residue.AminoAcid))
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
