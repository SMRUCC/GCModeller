#Region "Microsoft.VisualBasic::02af5ec236b1e29642f7aca92d928c34, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\CAI\RelativeCodonBiases.vb"

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
Imports SMRUCC.genomics.SequenceModel.Polypeptides.Polypeptides
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Measures of Relative Codon Biases
''' 
''' CAI: codon adaptation index
''' 
''' This specifies a reference set of genes, almost invariably, H, chosen from among “highly expressed genes.”
''' 
''' Defining W(xyz) = f(xyz)/max(xyz[a])*f(xyz,H) as the ratio of the frequency of the codon (xyz) to the 
''' maximal codon frequency in H for the same amino acid a ,the CAI of a gene of length L is taken as PI(Wi)^(1/L) (the log average), 
''' where i refers to the ith codon of the gene and w is calculated as above. 
''' 
''' H 集合是所选择的高表达量的参考基因
''' </summary>
''' <remarks></remarks>
Public Class RelativeCodonBiases

    ReadOnly ORF As NucleotideModels.NucleicAcid
    ReadOnly _codonHash As Codon() = Codon.CreateHashTable

    Public Property CodonFrequencyStatics As Dictionary(Of Char, CodonFrequency)

    ''' <summary>
    ''' ORF的核酸序列之中构建出密码子偏好属性
    ''' </summary>
    ''' <param name="Sequence">ATG -> TGA这一段之间的ORF的核酸序列</param>
    Sub New(Sequence As FastaToken)
        ORF = New NucleotideModels.NucleicAcid(Sequence)
        CodonFrequencyStatics =
            ToChar.ToArray(Function(aa) __staticsOfMaxFrequencyCodon(aa.Value)) _
                  .ToDictionary(Function(fr) fr.AminoAcid)
    End Sub

    Public Function CAI() As Double
        Try
            Dim Codens = ToCodonCollection(ORF)
            Dim PIValue = (From code As Codon In Codens Select Me.W(code)).PI
            Return PIValue ^ (1 / Codens.Length)
        Catch ex As Exception
            ex = New Exception(ORF.Length / 3 & " is not a valid ORF slides.", ex)
            Call App.LogException(ex)

            Return -1
        End Try
    End Function

    ''' <summary>
    ''' 对Profile进行归一化处理
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function Normalization(p As TripleKeyValuesPair(Of Double)) As Double
        Dim d As Double = {p.Value1, p.Value2, p.Value3}.EuclideanDistance
        Return d
    End Function

    ''' <summary>
    ''' 计算 W(Codon)
    ''' 即计算当前的密码子与编码相同氨基酸的最高频率的密码子的商( 
    ''' Defining W(xyz) = f(xyz)/max(xyz[a])*f(xyz,H) as the ratio of the frequency of the codon (xyz) to the maximal codon frequency in H for the same amino acid a)
    ''' </summary>
    ''' <param name="Codon"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function W(Codon As Codon) As Double
        Dim AA As Char = TranslationTable.Translate(Codon)
        Dim Profile As CodonFrequency = Me.CodonFrequencyStatics(AA)
        Dim f As Double = (From aac In Profile.BiasFrequency Where aac.Key.Equals(Codon) Select aac).First.Value
        Dim max As Double = Profile.MaxBias.Value
        Dim value As Double = f / max
        Return value
    End Function

    ''' <summary>
    ''' 统计某一中氨基酸的编码偏好性
    ''' </summary>
    ''' <param name="AminoAcid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __staticsOfMaxFrequencyCodon(AminoAcid As Char) As CodonFrequency
        Dim AA = Polypeptides.ToEnums(AminoAcid)
        Dim HashLQueryValue = (From tl In CodenTable
                               Where tl.Value = AA
                               Select Hash = tl.Key,
                                   AAC = tl.Value).ToArray '得到蛋白质翻译编码的哈希值
        Dim HashLQuery = (From item In HashLQueryValue Select item.Hash).ToArray
        Dim CodonsForAA = (From item In Me._codonHash
                           Where Array.IndexOf(HashLQuery, item.TranslHash) > -1
                           Select item Distinct).ToArray      '从密码子之中查询哈希值
        Dim F As New CodonFrequency With {
            .AminoAcid = AminoAcid,
            .BiasFrequencyProfile = (From Codon As Codon
                                     In CodonsForAA
                                     Let BIAS = GenomeSignatures.CodonSignature(Me.ORF, Codon)
                                     Let C = New KeyValuePair(Of Codon, TripleKeyValuesPair(Of Double))(Codon, BIAS)
                                     Select C).ToArray
        }
        F.BiasFrequency = (From item In F.BiasFrequencyProfile
                           Select New KeyValuePair(Of Codon, Double)(item.Key, Normalization(item.Value))) _
                                .ToDictionary(Function(item) item.Key, Function(item) item.Value)
        Dim sum As Double = (From n In F.BiasFrequency Select n.Value).Sum
        F.BiasFrequency = (From item In F.BiasFrequency
                           Select New KeyValuePair(Of Codon, Double)(item.Key, item.Value / sum)) _
                                .ToDictionary(Function(item) item.Key, Function(item) item.Value) ' 归一化处理，从而得到等式 SIGMA(x, y, z)[AA][c(x, y, z)] = 1
        F.MaxBias = (From item In F.BiasFrequency Select item Order By item.Value Descending).First
        Return F
    End Function
End Class

