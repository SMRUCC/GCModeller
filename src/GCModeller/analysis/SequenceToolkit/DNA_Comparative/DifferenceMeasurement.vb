#Region "Microsoft.VisualBasic::287c0f72a54af777caae3ef6fcedc12c, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\DifferenceMeasurement.vb"

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

Imports System.ComponentModel
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' MEASURES OF DIFFERENCES WITHIN AND BETWEEN GENOMES.(比较两条核酸序列之间的差异性)
''' </summary>
''' <remarks></remarks>
''' 
<PackageNamespace("Difference.Measurement",
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
    <ExportAPI("Sigma", Info:="A measure of difference between two sequences f and g (from different organisms or from different regions of the same genome) 
 is the average absolute dinucleotide relative abundance difference calculated as

 sigma(f, g) = (1/16)*∑|pXY(f)-pXY(g)|
 
 where the sum extends over all dinucleotides (abbreviated sigma-differences).")>
    Public Function Sigma(f As NucleotideModels.NucleicAcid, g As NucleotideModels.NucleicAcid) As Double
        Dim sum As Double
        sum += InternalBIAS(f, g, DNA.dAMP, DNA.dAMP) + InternalBIAS(f, g, DNA.dAMP, DNA.dCMP) +
               InternalBIAS(f, g, DNA.dAMP, DNA.dGMP) + InternalBIAS(f, g, DNA.dAMP, DNA.dTMP)

        sum += InternalBIAS(f, g, DNA.dCMP, DNA.dAMP) + InternalBIAS(f, g, DNA.dCMP, DNA.dCMP) +
               InternalBIAS(f, g, DNA.dCMP, DNA.dGMP) + InternalBIAS(f, g, DNA.dCMP, DNA.dTMP)

        sum += InternalBIAS(f, g, DNA.dGMP, DNA.dAMP) + InternalBIAS(f, g, DNA.dGMP, DNA.dCMP) +
               InternalBIAS(f, g, DNA.dGMP, DNA.dGMP) + InternalBIAS(f, g, DNA.dGMP, DNA.dTMP)

        sum += InternalBIAS(f, g, DNA.dTMP, DNA.dAMP) + InternalBIAS(f, g, DNA.dTMP, DNA.dCMP) +
               InternalBIAS(f, g, DNA.dTMP, DNA.dGMP) + InternalBIAS(f, g, DNA.dTMP, DNA.dTMP)
        sum = sum / 16
        Return sum
    End Function

    <ExportAPI("Sigma")>
    Public Function Sigma(f As FASTA.FastaToken, g As FASTA.FastaToken) As Double
        Return Sigma(New NucleotideModels.NucleicAcid(f), New NucleotideModels.NucleicAcid(g))
    End Function

    <ExportAPI("Sigma")>
    Public Function Sigma(<Parameter("Nt.f")> f As String, <Parameter("Nt.g")> g As String) As Double
        Return Sigma(New NucleotideModels.NucleicAcid(f), New NucleotideModels.NucleicAcid(g))
    End Function

    Private Function InternalBIAS(f As NucleotideModels.NucleicAcid,
                                  g As NucleotideModels.NucleicAcid,
                                  X As DNA, Y As DNA) As Double
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
        Dim Cache = New NucleicAcid(g)

        sum += __bias(f, Cache, DNA.dAMP, DNA.dAMP) + __bias(f, Cache, DNA.dAMP, DNA.dCMP) +
               __bias(f, Cache, DNA.dAMP, DNA.dGMP) + __bias(f, Cache, DNA.dAMP, DNA.dTMP)

        sum += __bias(f, Cache, DNA.dCMP, DNA.dAMP) + __bias(f, Cache, DNA.dCMP, DNA.dCMP) +
               __bias(f, Cache, DNA.dCMP, DNA.dGMP) + __bias(f, Cache, DNA.dCMP, DNA.dTMP)

        sum += __bias(f, Cache, DNA.dGMP, DNA.dAMP) + __bias(f, Cache, DNA.dGMP, DNA.dCMP) +
               __bias(f, Cache, DNA.dGMP, DNA.dGMP) + __bias(f, Cache, DNA.dGMP, DNA.dTMP)

        sum += __bias(f, Cache, DNA.dTMP, DNA.dAMP) + __bias(f, Cache, DNA.dTMP, DNA.dCMP) +
               __bias(f, Cache, DNA.dTMP, DNA.dGMP) + __bias(f, Cache, DNA.dTMP, DNA.dTMP)
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
        Else  'Value >= 190
            Return SimilarDiscriptions.VeryDistant
        End If
    End Function

    ''' <summary>
    ''' For convenience, we describe levels of sigma-differences for some reference examples (all values mutliplied by 1000)
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SimilarDiscriptions

        ''' <summary>
        ''' (sigma &lt;= 50; pervasively within species, human vs cow, Lactococcus lactis vs Streptococcus pyogenes).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma <= 50; pervasively within species, human vs cow, Lactococcus lactis vs Streptococcus pyogenes")>
        Close

        ''' <summary>
        ''' (55 &lt;= sigma &lt;= 85; human vs chicken, Escherichia coli vs Haemophilus influenzae, Synechococcus vs Anabaena).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [55, 85]; human vs chicken, Escherichia coli vs Haemophilus influenzae, Synechococcus vs Anabaena")>
        ModeratelySimilar

        ''' <summary>
        ''' (90 &lt;= sigma &lt;= 120; human vs sea urchin, M. genitalium vs M. pneumoniae).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [90, 120]; human vs sea urchin, M. genitalium vs M. pneumoniae")>
        WeaklySimilar

        ''' <summary>
        ''' (125 &lt;= sigma &lt;= 145; human vs Sulfolobus, E. coli vs R. prowazekii, M. jannaschii vs M. thermoautotrophicum).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [125, 145]; human vs Sulfolobus, E. coli vs R. prowazekii, M. jannaschii vs M. thermoautotrophicum")>
        DistantlySimilar

        ''' <summary>
        ''' (150 &lt;= sigma &lt;= 180; human vs Drosophilia, E. coli vs Helicobacter pylori).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma = [150, 180]; human vs Drosophilia, E. coli vs Helicobacter pylori")>
        Distant

        ''' <summary>
        ''' (sigma >= 190; human vs E. coli, E. coli vs Sulfolobus, M. jannaschii vs Halobacterium).
        ''' </summary>
        ''' <remarks></remarks>
        <Description("sigma >= 190; human vs E. coli, E. coli vs Sulfolobus, M. jannaschii vs Halobacterium")>
        VeryDistant
    End Enum
End Module

