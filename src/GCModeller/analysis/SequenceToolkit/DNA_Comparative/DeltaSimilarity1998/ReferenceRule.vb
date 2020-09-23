#Region "Microsoft.VisualBasic::d3066e500f5bff0c02a07ff2f88111fc, analysis\SequenceToolkit\DNA_Comparative\DeltaSimilarity1998\ReferenceRule.vb"

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

    '     Module ReferenceRule
    ' 
    '         Properties: dnaA, gyrB
    ' 
    '         Function: (+2 Overloads) dnaA_gyrB, (+2 Overloads) GetReferenceRule
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ObjectQuery
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports nucl = SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' 生成参考所使用的外标尺核酸片段
    ''' </summary>
    Public Module ReferenceRule

        ''' <summary>
        ''' chromosomal replication initiator protein DnaA
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property dnaA As New NamedCollection(Of String) With {
            .name = "dnaA",
            .value = {
                "chromosomal replication initiator protein DnaA",
                "chromosomal replication initiator"
            }
        }
        ''' <summary>
        ''' DNA gyrase, B subunit
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property gyrB As New NamedCollection(Of String) With {
            .name = "gyrB",
            .value = {
                "DNA gyrase B subunit",
                "DNA gyrase, B subunit"
            }
        }

        ''' <summary>
        ''' Using the DNA segment between the ``dnaA`` and ``gyrB`` as the reference rule.
        ''' </summary>
        ''' <param name="nt"></param>
        ''' <param name="proteins"></param>
        ''' <returns></returns>
        <Extension>
        Public Function dnaA_gyrB(nt As FastaSeq, proteins As PTT) As FastaSeq
            Return nt.GetReferenceRule(proteins, dnaA, gyrB)
        End Function

        ''' <summary>
        ''' Using the DNA segment between the ``dnaA`` and ``gyrB`` as the reference rule.
        ''' </summary>
        ''' <returns></returns>
        <Extension>
        Public Function dnaA_gyrB(genome As GBFF.File) As FastaSeq
            Return genome.GetReferenceRule(dnaA, gyrB)
        End Function

        ''' <summary>
        ''' Gets the segment betweens the dnaA and gyrB nucleotide sequence as 
        ''' the default reference rule for the homogeneity measuring.
        ''' 
        ''' (获取默认的外标尺：基因组之中的dnaA-gyrB之间的序列)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function GetReferenceRule(nt As FastaSeq, PTT As PTT, start As NamedCollection(Of String), ends As NamedCollection(Of String)) As FastaSeq
            Dim dnaA As GeneBrief = PTT.MatchGene(start.name, start.value)
            Dim gyrB As GeneBrief = PTT.MatchGene(ends.name, ends.value)

            If (dnaA Is Nothing OrElse gyrB Is Nothing) Then
                Call $"Could not found gene '{start.name}' or '{ends.name}' on {nt.Title}".PrintException
                Return Nothing
            End If

            ' 默认dnaA - gyrB这个基因簇是位于正义链的
            Dim St As Integer = dnaA.Location.left
            Dim sp As Integer = gyrB.Location.right

            ' 但是有些基因组或者由于测序的原因，位于负义链。。。
            If dnaA.Location.Strand = Strands.Reverse Then
                St = gyrB.Location.left
                sp = dnaA.Location.right
            End If

            Dim ruleSegment As nucl
            Try
                ' 构建基因组外标尺片段的计算模型
                ruleSegment = nt.CutSequenceLinear(left:=St, right:=sp)
                If ruleSegment.Length > 10 * 1000 Then
                    Call $"Location exception on (""{nt.Title}"") parsing segment.".PrintException
                    Return Nothing
                End If
            Catch ex As Exception
                Call App.LogException(ex)
                Call ex.PrintException
                Return Nothing
            End Try

            Return New FastaSeq With {
                .Headers = New String() {$"{start.name}-{ends.name}", nt.Title},
                .SequenceData = ruleSegment.SequenceData
            }
        End Function

        <Extension>
        Public Function GetReferenceRule(genome As GBFF.File, start As NamedCollection(Of String), ends As NamedCollection(Of String)) As FastaSeq
            Dim nt As FastaSeq = genome.Origin.ToFasta
            Dim proteins As PTT = genome.GbffToPTT(ORF:=True)
            Return nt.GetReferenceRule(proteins, start, ends)
        End Function
    End Module
End Namespace
