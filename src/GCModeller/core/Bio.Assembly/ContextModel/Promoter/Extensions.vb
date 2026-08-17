#Region "Microsoft.VisualBasic::2d422031a337268fda7c34ad348e5ee3, core\Bio.Assembly\ContextModel\Promoter\Extensions.vb"

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

'   Total Lines: 85
'    Code Lines: 53 (62.35%)
' Comment Lines: 23 (27.06%)
'    - Xml Docs: 91.30%
' 
'   Blank Lines: 9 (10.59%)
'     File Size: 3.57 KB


'     Module Extensions
' 
'         Function: GetPrefixLengths, GetUpstreamSeq, headers, ParseUpstreamByLength
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Slicer

Namespace ContextModel.Promoter

    Public Module Extensions

        ''' <summary>
        ''' Read from <see cref="PrefixLength"/> members.
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPrefixLengths() As IEnumerable(Of Integer)
            Return From L In GetType(PrefixLength).GetEnumValues Select CInt(L)
        End Function

        ''' <summary>
        ''' 解析出所有基因前面的序列片段
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="nt"></param>
        ''' <param name="length%"></param>
        ''' <param name="minLen%">
        ''' 最小有效间区长度（bp）。当上游调控区候选区间与上游基因的终止密码子重叠、
        ''' 且二者之间的间区小于该阈值时，对应基因返回空序列并标记 "intergenic too short"。
        ''' 详见 <see cref="GetUpstreamSeq"/>。
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function ParseUpstreamByLength(context As PTT, nt As IPolymerSequenceModel, length%, Optional minLen% = 20) As Dictionary(Of String, FastaSeq)
            Dim genes As New GenomeContext(Of GeneBrief)(context.GeneObjects)
            Dim parser = From gene As GeneBrief
                         In genes.AsEnumerable.AsParallel
                         Let upstream = gene.GetUpstreamSeq(genes, nt, length, minLen)
                         Select gene.Synonym,
                             promoter = upstream
            Dim table = parser.ToDictionary(Function(g) g.Synonym, Function(g) g.promoter)
            Return table
        End Function

        ''' <summary>
        ''' Get upstream nt sequence in a specific length for target gene.
        ''' </summary>
        ''' <param name="gene">
        ''' The target gene (gene B) whose upstream regulatory region will be extracted.
        ''' </param>
        ''' <param name="context">
        ''' The genome gene context, used to locate the transcriptionally upstream
        ''' neighbour gene (gene A) via <see cref="GenomeContext(Of T).GetDirectionalNeighbours"/>.
        ''' </param>
        ''' <param name="nt">The genomic nucleotide sequence (circular aware).</param>
        ''' <param name="len%">
        ''' The desired length of the upstream regulatory region, measured from the
        ''' transcription start site (TSS, i.e. the ATG end of <paramref name="gene"/>).
        ''' </param>
        ''' <param name="minLen%">
        ''' The minimal effective length of the intergenic spacer. When the candidate
        ''' region overlaps the upstream gene A's stop codon, the regulatory region is
        ''' reduced to the intergenic spacer (gene A stop codon -> gene B TSS). If that
        ''' spacer is shorter than <paramref name="minLen"/> the extraction is skipped and
        ''' an empty <see cref="FastaSeq"/> tagged "intergenic too short" is returned.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' (在这个函数之中，位点的计算的时候会有一个碱基的偏移量是因为为了不将起始密码子ATG之中的A包含在结果序列之中)
        ''' 
        ''' The extraction strategy is overlap-aware:
        ''' 1. Compute the candidate region of <paramref name="len"/> bp upstream of gene B's TSS.
        ''' 2. If this region does not overlap the upstream gene A's stop codon, the full
        '''    <paramref name="len"/> bp is taken directly.
        ''' 3. Otherwise the regulatory region is reduced to the intergenic spacer between
        '''    gene A's stop codon and gene B's TSS. When that spacer is shorter than
        '''    <paramref name="minLen"/> an empty sequence tagged "intergenic too short"
        '''    is returned (the two genes may be co-regulated).
        ''' </remarks>
        <Extension>
        Public Function GetUpstreamSeq(Of T As IGeneBrief)(gene As T, context As GenomeContext(Of T), nt As IPolymerSequenceModel, len%, Optional minLen% = 20) As FastaSeq
            Dim loci As NucleotideLocation = gene.Location.Normalization()
            Dim strand As Strands = loci.Strand

            ' TSS (transcription start site) == the ATG end of the coding region.
            ' The "-1" offset excludes the ATG start base from the result, consistent
            ' with the previous implementation.
            Dim tss As Long
            Dim candidateLeft As Integer, candidateRight As Integer

            If strand = Strands.Forward Then
                tss = loci.left
                candidateLeft = loci.left - len
                candidateRight = loci.left - 1
            Else
                tss = loci.right
                ' 反向序列上游为坐标增大方向（互补链）
                candidateLeft = loci.right + 1
                candidateRight = loci.right + len
            End If

            ' Locate the transcriptionally upstream neighbour gene A (best effort: only
            ' GeneBrief exposes the stop codon position). Fall back to the plain fixed
            ' length extraction when gene is not a GeneBrief.
            Dim upstreamGene As IGeneBrief = Nothing
            Dim geneB = TryCast(gene, IGeneBrief)

            If geneB IsNot Nothing Then
                Dim neighbours = context.GetDirectionalNeighbours(geneB)
                upstreamGene = neighbours.Upstream
            End If

            Dim takeFullLength As Boolean = True
            Dim intergenicLeft As Integer = candidateLeft
            Dim intergenicRight As Integer = candidateRight

            If upstreamGene IsNot Nothing Then
                Dim aStop As Long = upstreamGene.TGA

                ' Does the candidate region reach into / overlap the upstream gene A's
                ' stop codon (the gene A coding body)?
                '
                ' Forward: gene A is on the left, candidate extends leftward (decreasing
                '   coordinate). Overlap when candidateLeft <= aStop.
                ' Reverse : gene A is on the right (physically), candidate extends
                '   rightward (increasing coordinate). Overlap when candidateRight >= aStop.
                Dim overlapsStop As Boolean

                If strand = Strands.Forward Then
                    overlapsStop = candidateLeft <= aStop
                Else
                    overlapsStop = candidateRight >= aStop
                End If

                If overlapsStop Then
                    takeFullLength = False

                    ' Reduce the regulatory region to the intergenic spacer between
                    ' gene A's stop codon and gene B's TSS.
                    If strand = Strands.Forward Then
                        intergenicLeft = aStop + 1
                        intergenicRight = tss - 1
                    Else
                        intergenicLeft = tss + 1
                        intergenicRight = aStop - 1
                    End If
                End If
            End If

            Dim site As SimpleSegment
            Dim attrs$() = Nothing

            If takeFullLength Then
                ' No overlap with upstream gene A: take the full candidate length.
                Dim candidateLoci As NucleotideLocation

                If strand = Strands.Forward Then
                    candidateLoci = New NucleotideLocation(candidateLeft, candidateRight)
                Else
                    candidateLoci = New NucleotideLocation(candidateLeft, candidateRight, ComplementStrand:=True)
                End If

                site = nt.CutSequenceCircular(candidateLoci)
                attrs = gene.headers(site)
            Else
                Dim spacerLength As Long = Math.Abs(intergenicRight - intergenicLeft) + 1

                If spacerLength < minLen Then
                    ' Intergenic spacer is too short to be an independent regulatory
                    ' region; gene B may be co-regulated with the upstream gene A.
                    Return New FastaSeq With {
                        .Headers = {gene.Feature & " " & gene.Key, "intergenic too short"},
                        .SequenceData = ""
                    }
                End If

                Dim intergenicLoci As NucleotideLocation

                If strand = Strands.Forward Then
                    intergenicLoci = New NucleotideLocation(intergenicLeft, intergenicRight)
                Else
                    intergenicLoci = New NucleotideLocation(intergenicLeft, intergenicRight, ComplementStrand:=True)
                End If

                site = nt.CutSequenceCircular(intergenicLoci)
                attrs = gene.headers(site)
            End If

            Dim promoter As New FastaSeq With {
                .Headers = attrs,
                .SequenceData = site.SequenceData
            }

            Return promoter
        End Function

        <Extension>
        Private Function TGA(g As IGeneBrief) As Integer
            Dim loc = g.Location

            If loc.Strand = Strands.Forward Then
                Return loc.right
            Else
                Return loc.left
            End If
        End Function

        <Extension>
        Private Function headers(gene As IGeneBrief, site As SimpleSegment) As String()
            If gene.Product.StringEmpty Then
                Return {gene.Feature & " " & site.ID}
            Else
                Return {gene.Feature & " " & site.ID, gene.Product}
            End If
        End Function
    End Module
End Namespace
