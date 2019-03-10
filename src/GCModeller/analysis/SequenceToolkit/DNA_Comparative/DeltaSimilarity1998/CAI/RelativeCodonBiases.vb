﻿#Region "Microsoft.VisualBasic::7ccdefc87762b2622610613cba399ec5, analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\CAI\RelativeCodonBiases.vb"

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

    '     Class RelativeCodonBiases
    ' 
    '         Properties: CodonFrequencyStatics
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __staticsOfMaxFrequencyCodon, CAI, ToString, W
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
Imports SMRUCC.genomics.SequenceModel.Polypeptides
Imports SMRUCC.genomics.SequenceModel.Polypeptides.Polypeptides

Namespace DeltaSimilarity1998.CAI

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

        ReadOnly ORF As NucleicAcid
        ReadOnly _codonHash As Codon() = Codon.CreateHashTable

        ''' <summary>
        ''' <see cref="CodonFrequency.AminoAcid"/>就是这个字典的键名key
        ''' </summary>
        ''' <returns></returns>
        Public Property CodonFrequencyStatics As Dictionary(Of Char, CodonFrequency)

        ''' <summary>
        ''' ORF的核酸序列之中构建出密码子偏好属性
        ''' </summary>
        ''' <param name="nt">ATG -> TGA这一段之间的ORF的核酸序列</param>
        Sub New(nt As FastaSeq)
            ORF = New NucleicAcid(nt)
            CodonFrequencyStatics =
                ToChar _
                .Select(Function(aa) __staticsOfMaxFrequencyCodon(aa.Value)) _
                .ToDictionary(Function(fr) fr.AminoAcid)
        End Sub

        Public Function CAI() As Double
            Try
                Dim Codens = ToCodonCollection(ORF)
                Dim PIValue = (From code As Codon In Codens Select Me.W(code)).ProductALL
                Return PIValue ^ (1 / Codens.Length)
            Catch ex As Exception
                ex = New Exception(ORF.Length / 3 & " is not a valid ORF slides.", ex)
                Call App.LogException(ex)

                Return -1
            End Try
        End Function

        Public Overrides Function ToString() As String
            Return ORF.UserTag
        End Function

        ''' <summary>
        ''' ###### 计算 W(Codon)
        ''' 
        ''' 即计算当前的密码子与编码相同氨基酸的最高频率的密码子的商
        ''' (Defining ``W(xyz) = f(xyz)/max(xyz[a])*f(xyz,H)`` as the ratio of the frequency of the codon (xyz) 
        ''' to the maximal codon frequency in ``H`` for the same amino acid ``a``)
        ''' </summary>
        ''' <param name="Codon"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function W(Codon As Codon) As Double
            Dim AA As Char = TranslationTable.Translate(Codon)
            Dim Profile As CodonFrequency = Me.CodonFrequencyStatics(AA)
            Dim f As Double = (From aac In Profile.BiasFrequency Where aac.Key = Codon.CodonValue Select aac).First.Value
            Dim max As Double = Profile.MaxBias.bias
            Dim value As Double = f / max
            Return value
        End Function

        ''' <summary>
        ''' 统计某一中氨基酸的编码偏好性
        ''' </summary>
        ''' <param name="aminoAcid">可能会有集中密码来编码相同的一个氨基酸</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __staticsOfMaxFrequencyCodon(aminoAcid As Char) As CodonFrequency
            Dim aa As AminoAcid = Polypeptides.ToEnums(aminoAcid)
            Dim hashAA = LinqAPI.Exec(Of SeqValue(Of AminoAcid)) <=
 _
                From tl
                In CodenTable
                Where tl.Value = aa
                Select New SeqValue(Of AminoAcid) With {
                    .i = tl.Key,
                    .value = tl.Value
                } ' 得到蛋白质翻译编码的哈希值

            Dim hashValues%() = hashAA.Indices
            Dim CodonsForAA = LinqAPI.Exec(Of Codon) <=
                From c As Codon
                In Me._codonHash
                Where Array.IndexOf(hashValues, c.TranslHash) > -1
                Select c
                Distinct      ' 从密码子之中查询哈希值

            Dim cfrq As New CodonFrequency With {
                .AminoAcid = aminoAcid,
                .BiasFrequencyProfile = CodonsForAA _
                    .Select(Function(c) Me.ORF.CodonSignature(c)) _
                    .ToDictionary(Function(c) c.Codon),
                .BiasFrequency =
                    .BiasFrequencyProfile _
                    .ToDictionary(Function(k) k.Key,
                                  Function(bias) bias.Value.EuclideanNormalization)
            }

            With cfrq
                Dim sum# = .BiasFrequency.Values.Sum

                ' 归一化处理，从而得到等式 SIGMA(x, y, z)[AA][c(x, y, z)] = 1
                .BiasFrequency =
                    .BiasFrequency _
                    .ToDictionary(Function(c) c.Key,
                                  Function(bias) bias.Value / sum)
                .MaxBias =
                    .BiasFrequency _
                    .OrderByDescending(Function(c) c.Value) _
                    .Select(Function(o) (o.Key, o.Value)) _
                    .First
            End With

            Return cfrq
        End Function
    End Class
End Namespace
