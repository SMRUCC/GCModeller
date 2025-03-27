#Region "Microsoft.VisualBasic::5393f912e7a70e8485add45088b2232d, core\Bio.Assembly\ProteinModel\Chou-Fasman\ChouFasman.vb"

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

    '   Total Lines: 137
    '    Code Lines: 60 (43.80%)
    ' Comment Lines: 58 (42.34%)
    '    - Xml Docs: 91.38%
    ' 
    '   Blank Lines: 19 (13.87%)
    '     File Size: 7.17 KB


    '     Module ChouFasman
    ' 
    '         Function: (+2 Overloads) Calculate, sequencePoly, Tabular, ToString
    ' 
    '         Sub: Print
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace ProteinModel.ChouFasmanRules

    ''' <summary>
    ''' The Chou-Fasman method is a bioinformatics technique used for predicting the secondary structure of proteins. 
    ''' It was developed by Peter Y. Chou and Gerald D. Fasman in the 1970s. The method is based on the observation 
    ''' that certain amino acids have a propensity to form specific types of secondary structures, such as alpha-helices, 
    ''' beta-sheets, and turns.
    ''' 
    ''' Here's a brief overview of how the Chou-Fasman method works:
    ''' 
    ''' 1. **Amino Acid Propensities**: Each amino acid is assigned a set of probability values that reflect its 
    '''    tendency to be found in alpha-helices, beta-sheets, and turns. These values are derived from statistical 
    '''    analysis of known protein structures.
    ''' 2. **Sliding Window Technique**: A sliding window of typically 7 to 9 amino acids is moved along the protein 
    '''    sequence. At each position, the average propensity for each type of secondary structure is calculated 
    '''    for the amino acids within the window.
    ''' 3. **Thresholds and Rules**: The method uses predefined thresholds and rules to identify regions of the 
    '''    protein sequence that are likely to form alpha-helices or beta-sheets based on the calculated propensities. 
    '''    For example, a region with a high average propensity for alpha-helix and meeting certain criteria 
    '''    might be predicted to form an alpha-helix.
    ''' 4. **Secondary Structure Prediction**: The method predicts the secondary structure by identifying contiguous 
    '''    regions of the sequence that exceed the thresholds for helix or sheet formation. It also takes into 
    '''    account the likelihood of turns, which are important for the overall folding of the protein.
    ''' 5. **Refinement**: The initial predictions are often refined using additional rules and considerations, such 
    '''    as the tendency of certain amino acids to stabilize or destabilize specific structures, and the overall 
    '''    composition of the protein.
    '''    
    ''' The Chou-Fasman method was one of the first widely used techniques for predicting protein secondary structure
    ''' and played a significant role in the field of structural bioinformatics. However, it has largely been superseded
    ''' by more accurate methods, such as those based on machine learning and neural networks, which can take into
    ''' account more complex patterns and interactions within protein sequences.
    ''' 
    ''' Despite its limitations, the Chou-Fasman method remains a historical milestone in the understanding of 
    ''' protein structure and the development of computational methods for predicting it. It also serves as a 
    ''' foundational concept for those learning about protein structure prediction and bioinformatics.
    ''' </summary>
    ''' <remarks>
    ''' @ - <see cref="SecondaryStructures.AlphaHelix"></see>;
    ''' - - <see cref="SecondaryStructures.BetaSheet"></see>;
    ''' ^ - <see cref="SecondaryStructures.BetaTurn"></see>;
    ''' &amp; - <see cref="SecondaryStructures.Coils"></see>
    ''' </remarks>
    Public Module ChouFasman

        Friend ReadOnly StructureTypesToChar As Dictionary(Of SecondaryStructures, String) =
            Enums(Of SecondaryStructures) _
            .ToDictionary(Function(a) a,
                          Function(a)
                              Return a.Description
                          End Function)

        ''' <summary>
        ''' convert sequence string data as poly array
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <returns></returns>
        Private Function sequencePoly(SequenceData As String) As AminoAcid()
            Dim SequenceEnums = Polypeptide.ConstructVector(SequenceData).ToArray
            Dim AA = (From a In SequenceEnums Where a <> Polypeptides.AminoAcid.NULL Select New AminoAcid(a)).ToArray

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
            Dim poly As AminoAcid() = sequencePoly(sequence)
            Dim AlphaMaskCounts = Rules.RuleAlphaHelix.Invoke(poly)
            Dim BetaSheetMasks_ = Rules.RuleBetaSheet.Invoke(poly)
            Dim BetaTurnMasks__ = Rules.RuleBetaTurn.Invoke(poly)

            Call Rules.RuleOverlap.Invoke(poly)

            Return poly
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

        Public Sub Print(aa As AminoAcid(), str As TextWriter)
            Dim aa_Builder As New StringBuilder(aa.Length - 1)
            Dim st_Builder As New StringBuilder(aa.Length - 1)

            For Each residue As AminoAcid In aa
                Call aa_Builder.Append(Polypeptide.ToChar(residue.AminoAcid))
                Call st_Builder.Append(ChouFasman.StructureTypesToChar(residue.StructureType))
            Next

            Call str.WriteLine(String.Format("Key,Value" & vbCrLf & "SequenceData,{0}" & vbCrLf & "Structure,{1}", aa_Builder.ToString, st_Builder.ToString))

            For Each item As KeyValuePair(Of SecondaryStructures, String) In ChouFasman.StructureTypesToChar
                Call str.WriteLine(String.Format("{0},{1}", item.Key.ToString, item.Value))
            Next

            Call str.Flush()
        End Sub

        Public Function ToString(aa As AminoAcid()) As String
            Return (From a As AminoAcid
                    In aa
                    Select ChouFasman.StructureTypesToChar(a.StructureType)).JoinBy("")
        End Function

        Public Function Tabular(aa As AminoAcid()) As String
            Dim sb As New StringBuilder

            Using writer As New StringWriter(sb)
                Call Print(aa, writer)
            End Using

            Return sb.ToString
        End Function
    End Module
End Namespace
