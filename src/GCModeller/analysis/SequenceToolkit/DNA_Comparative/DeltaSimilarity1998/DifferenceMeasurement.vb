#Region "Microsoft.VisualBasic::30a913ffaf7a10375e4ce2b58286a3d7, GCModeller\analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\DifferenceMeasurement.vb"

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

    '   Total Lines: 161
    '    Code Lines: 87
    ' Comment Lines: 52
    '   Blank Lines: 22
    '     File Size: 8.21 KB


    '     Module DifferenceMeasurement
    ' 
    '         Function: __bias, BIAS, (+5 Overloads) Sigma, SimilarDescription
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' MEASURES OF DIFFERENCES WITHIN AND BETWEEN GENOMES.(比较两条核酸序列之间的差异性)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("Difference.Measurement",
                  Description:="MEASURES OF DIFFERENCES WITHIN AND BETWEEN GENOMES.(比较两条核酸序列之间的差异性)",
                  Cites:="Karlin, S., et al. (1998). ""Comparative DNA analysis across diverse genomes."" Annu Rev Genet 32: 185-225.
	We review concepts and methods for comparative analysis of complete genomes including assessments of genomic compositional contrasts based on dinucleotide and tetranucleotide relative abundance values, identifications of rare and frequent oligonucleotides, evaluations and interpretations of codon biases in several large prokaryotic genomes, and characterizations of compositional asymmetry between the two DNA strands in certain bacterial genomes. The discussion also covers means for identifying alien (e.g. laterally transferred) genes and detecting potential specialization islands in bacterial genomes.

",
                  Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module DifferenceMeasurement

        ''' <summary>
        ''' A measure of difference between two sequences f and g (from different organisms or from different regions of the same genome) 
        ''' is the average absolute dinucleotide relative abundance difference calculated as
        '''
        ''' ```
        ''' sigma(f, g) = (1/16)*∑|pXY(f)-pXY(g)|
        ''' ```
        ''' 
        ''' where the sum extends over all dinucleotides (abbreviated sigma-differences).
        ''' </summary>
        ''' <param name="f"></param>
        ''' <param name="g"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function Sigma(f As NucleotideModels.NucleicAcid, g As NucleotideModels.NucleicAcid) As Double
            Dim sum As Double
            Dim ntf As New NucleicAcid(f)
            Dim ntg As New NucleicAcid(g)

            sum += BIAS(ntf, ntg, DNA.dAMP, DNA.dAMP) + BIAS(ntf, ntg, DNA.dAMP, DNA.dCMP) + BIAS(ntf, ntg, DNA.dAMP, DNA.dGMP) + BIAS(ntf, ntg, DNA.dAMP, DNA.dTMP)
            sum += BIAS(ntf, ntg, DNA.dCMP, DNA.dAMP) + BIAS(ntf, ntg, DNA.dCMP, DNA.dCMP) + BIAS(ntf, ntg, DNA.dCMP, DNA.dGMP) + BIAS(ntf, ntg, DNA.dCMP, DNA.dTMP)
            sum += BIAS(ntf, ntg, DNA.dGMP, DNA.dAMP) + BIAS(ntf, ntg, DNA.dGMP, DNA.dCMP) + BIAS(ntf, ntg, DNA.dGMP, DNA.dGMP) + BIAS(ntf, ntg, DNA.dGMP, DNA.dTMP)
            sum += BIAS(ntf, ntg, DNA.dTMP, DNA.dAMP) + BIAS(ntf, ntg, DNA.dTMP, DNA.dCMP) + BIAS(ntf, ntg, DNA.dTMP, DNA.dGMP) + BIAS(ntf, ntg, DNA.dTMP, DNA.dTMP)
            sum = sum / 16

            Return sum
        End Function

        <ExportAPI("Sigma")>
        Public Function Sigma(f As FASTA.FastaSeq, g As FASTA.FastaSeq) As Double
            Return Sigma(New NucleotideModels.NucleicAcid(f), New NucleotideModels.NucleicAcid(g))
        End Function

        <ExportAPI("Sigma")>
        Public Function Sigma(<Parameter("Nt.f")> f As String, <Parameter("Nt.g")> g As String) As Double
            Return Sigma(New NucleotideModels.NucleicAcid(f), New NucleotideModels.NucleicAcid(g))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function BIAS(f As NucleicAcid, g As NucleicAcid, X As DNA, Y As DNA) As Double
            Return Math.Abs(GenomeSignatures.DinucleotideBIAS(f, X, Y) - GenomeSignatures.DinucleotideBIAS(g, X, Y))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="f">计算缓存：窗口片段数据或者整条DNA链</param>
        ''' <param name="g"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Sigma")>
        Public Function Sigma(f As NucleicAcid, g As NucleicAcid) As Double
            Dim sum As Double
            sum += __bias(f, g, DNA.dAMP, DNA.dAMP) + __bias(f, g, DNA.dAMP, DNA.dCMP) +
               __bias(f, g, DNA.dAMP, DNA.dGMP) + __bias(f, g, DNA.dAMP, DNA.dTMP)

            sum += __bias(f, g, DNA.dCMP, DNA.dAMP) + __bias(f, g, DNA.dCMP, DNA.dCMP) +
               __bias(f, g, DNA.dCMP, DNA.dGMP) + __bias(f, g, DNA.dCMP, DNA.dTMP)

            sum += __bias(f, g, DNA.dGMP, DNA.dAMP) + __bias(f, g, DNA.dGMP, DNA.dCMP) +
               __bias(f, g, DNA.dGMP, DNA.dGMP) + __bias(f, g, DNA.dGMP, DNA.dTMP)

            sum += __bias(f, g, DNA.dTMP, DNA.dAMP) + __bias(f, g, DNA.dTMP, DNA.dCMP) +
               __bias(f, g, DNA.dTMP, DNA.dGMP) + __bias(f, g, DNA.dTMP, DNA.dTMP)
            sum = sum / 16
            Return sum
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="f">计算缓存：窗口片段数据或者整条DNA链</param>
        ''' <param name="g">当需要比对序列上面的某一个片段与整条序列之间的差异性的时候，这个参数可以为该需要进行计算比对的序列片段对象</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Sigma")>
        Public Function Sigma(f As NucleicAcid, g As NucleotideModels.NucleicAcid) As Double
            Dim sum As Double
            Dim cache As New NucleicAcid(g)

            sum += __bias(f, cache, DNA.dAMP, DNA.dAMP) + __bias(f, cache, DNA.dAMP, DNA.dCMP) +
               __bias(f, cache, DNA.dAMP, DNA.dGMP) + __bias(f, cache, DNA.dAMP, DNA.dTMP)

            sum += __bias(f, cache, DNA.dCMP, DNA.dAMP) + __bias(f, cache, DNA.dCMP, DNA.dCMP) +
               __bias(f, cache, DNA.dCMP, DNA.dGMP) + __bias(f, cache, DNA.dCMP, DNA.dTMP)

            sum += __bias(f, cache, DNA.dGMP, DNA.dAMP) + __bias(f, cache, DNA.dGMP, DNA.dCMP) +
               __bias(f, cache, DNA.dGMP, DNA.dGMP) + __bias(f, cache, DNA.dGMP, DNA.dTMP)

            sum += __bias(f, cache, DNA.dTMP, DNA.dAMP) + __bias(f, cache, DNA.dTMP, DNA.dCMP) +
               __bias(f, cache, DNA.dTMP, DNA.dGMP) + __bias(f, cache, DNA.dTMP, DNA.dTMP)
            sum = sum / 16
            Return sum
        End Function

        ''' <summary>
        ''' 使用计算缓存进行计算
        ''' </summary>
        ''' <param name="f">计算缓存</param>
        ''' <param name="g"></param>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __bias(f As NucleicAcid, g As NucleicAcid, X As DNA, Y As DNA) As Double
            Return Math.Abs(f.GetValue(X, Y) - g.GetValue(X, Y))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sigma">value from the calculation of function <see cref="DifferenceMeasurement.Sigma"></see></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Similar.Description")>
        Public Function SimilarDescription(sigma As Double) As SimilarDiscriptions
            Dim value = sigma * 1000

            If value <= 50 Then
                Return SimilarDiscriptions.Close
            ElseIf value > 50 AndAlso value <= 85 Then
                Return SimilarDiscriptions.ModeratelySimilar
            ElseIf value > 85 AndAlso value <= 120 Then
                Return SimilarDiscriptions.WeaklySimilar
            ElseIf value > 120 AndAlso value <= 145 Then
                Return SimilarDiscriptions.DistantlySimilar
            ElseIf value > 145 AndAlso value <= 180 Then
                Return SimilarDiscriptions.Distant
            Else  ' Value >= 190
                Return SimilarDiscriptions.VeryDistant
            End If
        End Function
    End Module
End Namespace
