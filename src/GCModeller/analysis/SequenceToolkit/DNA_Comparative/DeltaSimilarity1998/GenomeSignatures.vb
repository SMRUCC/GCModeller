#Region "Microsoft.VisualBasic::37ced5fd4eed5badc66ebc77405aaafb, GCModeller\analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\GenomeSignatures.vb"

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

    '   Total Lines: 128
    '    Code Lines: 62
    ' Comment Lines: 58
    '   Blank Lines: 8
    '     File Size: 5.84 KB


    '     Module GenomeSignatures
    ' 
    '         Function: __counts, __counts_p, CodonSignature, DinucleotideBIAS, DinucleotideBIAS_p
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ListExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Conversion
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' 在本模块之中，所有的计算过程都是基于<see cref="NucleicAcid"></see>核酸对象的
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("Genome.Signatures")>
    Public Module GenomeSignatures

        ''' <summary>
        ''' Dinucleotide relative abundance values (dinucleotide bias) are assessed through the odds ratio ``p(XY) = f(XY)/f(X)f(Y)``, 
        ''' where ``fX`` denotes the frequency of the nucleotide ``X`` and ``fXY`` is the frequency of the dinucleotide ``XY`` in the 
        ''' sequence under study.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function DinucleotideBIAS(nt As NucleicAcid, X As DNA, Y As DNA) As Double
            Dim len As Integer = nt.length
            Dim dibias As Double = nt.DNA_segments.__counts({X, Y}) / (len - 1)
            Dim fx# = NucleotideModels.NucleicAcid.Counts(nt.nt, X) / len
            Dim fy# = NucleotideModels.NucleicAcid.Counts(nt.nt, Y) / len
            Dim value As Double = dibias / (fx * fy)
            Return value
        End Function

        ''' <summary>
        ''' 计算某些连续的碱基片段在序列之中的出现频率
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="dpair"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Private Function __counts(nt As SlideWindow(Of DNA)(), dpair As DNA()) As Integer
            Dim equals = Function(n As SlideWindow(Of DNA))
                             Return DegenerateBasesExtensions.Equals(n.Items, dpair)
                         End Function
            Dim c% = nt _
                .Where(predicate:=equals) _
                .Count
            Return c
        End Function

        ''' <summary>
        ''' Dinucleotide relative abundance values (dinucleotide bias) are assessed through the 
        ''' odds ratio ``p(XY) = f(XY)/f(X)f(Y)``, where fX denotes the frequency of 
        ''' the nucleotide X and fXY is the frequency of the dinucleotide XY in the 
        ''' sequence under study.(并行版本)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function DinucleotideBIAS_p(nt As NucleicAcid, X As DNA, Y As DNA) As Double
            Dim len As Integer = nt.length
            Dim DNA_segments = nt.DNA_segments
            Dim dinucleotideBias# = DNA_segments.__counts_p({X, Y}) / (len - 1)
            Dim fx# = NucleotideModels.NucleicAcid.Counts(nt.nt, X) / len
            Dim fy# = NucleotideModels.NucleicAcid.Counts(nt.nt, Y) / len
            Dim out# = dinucleotideBias / (fx * fy)

            Return out
        End Function

        ''' <summary>
        ''' 计算某些连续的碱基片段在序列之中的出现频率(并行版本)
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="dpair">需要进行计算的碱基对</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Private Function __counts_p(nt As SlideWindow(Of DNA)(), dpair As DNA()) As Integer
            Dim c As Integer = nt _
                .AsParallel _
                .Where(Function(npair) DegenerateBasesExtensions.Equals(dpair, npair.Items)) _
                .Count
            Return c
        End Function

        ''' <summary>
        ''' CODON SIGNATURE
        ''' 
        ''' For a given collection of genes, let fX(1); fY(2); fZ(3) denote frequencies of the indicated nucleotide at codon sites 1, 2, and 3, respectively, 
        ''' and let f(XYZ) indicate codon frequency. The embedded dinucleotide frequencies are denoted fXY(1, 2); fYZ(2, 3); and fXZ(1, 3). Dinucleotide 
        ''' contrasts are assessed through the odds ratio pXY = f(XY)/f(X)f(Y). 
        ''' In the context of codons, we define
        ''' 
        ''' ```
        '''    pXY(1, 2) = fXY(1, 2)/fX(1)fY(2)
        '''    pYZ(2, 3) = fYZ(2, 3)/fY(2)fZ(3)
        '''    pXZ(1, 3) = fXZ(1, 3)/fX(1)fZ(3)
        ''' ```
        ''' 
        ''' We refer to the profiles {pXY(1, 2)}; {pXZ(1, 3)}; {pYZ(2, 3)}, and also {pZW(3, 4)}, where 4(=1) is the first position of the next codon, as the 
        ''' codon signature to be distinguished from the global genome signature
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="codon"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function CodonSignature(nt As NucleicAcid, codon As Codon) As CodonBiasVector
            Dim v As New CodonBiasVector With {
                .Codon = $"{ToChar(codon.X)}{ToChar(codon.Y)}{ToChar(codon.Z)}",
                .XY = nt.DinucleotideBIAS(X:=codon.X, Y:=codon.Y),
                .YZ = nt.DinucleotideBIAS(X:=codon.Y, Y:=codon.Z),
                .XZ = nt.DinucleotideBIAS(X:=codon.X, Y:=codon.Z)
            }
            Return v
        End Function
    End Module
End Namespace
