#Region "Microsoft.VisualBasic::33baa0da15101c99e1e4e79041402bb3, core\Bio.Assembly\ContextModel\Operon\FeatureScores.vb"

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

'     Module FeatureScores
' 
'         Function: DHamming, IntergenicDistance, L, LengthRatio, NeighborhoodConservation
'                   (+2 Overloads) P
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports stdNum = System.Math

Namespace ContextModel.Operon

    ''' <summary>
    ''' To evaluate the contribution of selected features in operon prediction, we have calculated 
    ''' the numerical values of the features, And then used these values individually And in combination 
    ''' to train a classifier. The features used in our study are
    ''' 
    ''' + (i)   the intergenic distance, 
    ''' + (ii)  the conserved gene neighborhood, 
    ''' + (iii) distances between adjacent genes' phylogenetic profiles, 
    ''' + (iv)  the ratio between the lengths of two adjacent genes And 
    ''' + (v)   frequencies Of specific DNA motifs in the intergenic regions.
    ''' </summary>
    ''' <remarks>
    ''' Operon prediction using both genome-specific and general genomic information 
    ''' 
    ''' https://academic.oup.com/nar/article/35/1/288/2401876
    ''' </remarks>
    Public Module FeatureScores

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IntergenicDistance(upstream As NucleotideLocation, downstream As NucleotideLocation) As Integer
            If upstream.Strand <> downstream.Strand Then
                Throw New Exception("Invalid strand data!")
            Else
                If upstream.Strand = Strands.Forward Then
                    Return (downstream.left - (upstream.right + 1))
                Else
                    Return (upstream.left - (downstream.right + 1))
                End If
            End If
        End Function

        ''' <summary>
        ''' 这个函数定义了基因i和基因j在某一个基因组之中是相邻的概率高低
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function NeighborhoodConservation(Of T As IGeneBrief)(gi$, gj$, genomes As GenomeContext(Of T)(), P As Dictionary(Of String, Double)) As Double
            Dim S = -(Aggregate Gk In genomes Let log = stdNum.Log(L(gi, gj, G:=Gk, P:=P)) Into Sum(log))
            Return S
        End Function

        ''' <summary>
        ''' where ``L(gi, gj, Gk)`` is the loglikelihood of a gene pair to be neighbors in the kth genome Gk. 
        ''' The log-likelihood score Is computed as the probability that gi And gj are neighbors within a distance dk(i,j) In Gk, 
        ''' Or L(gi, gj, Gk) = log Pij; Pij Is defined as follows:
        ''' 
        ''' (i)   Pij = (1-pik)(1-pjk), if both genes are absent from genome Gk,
        ''' (ii)  Pij = (1-pik) pjk, if only gene j Is present In genome Gk,
        ''' (iii) Pij = pik (1-pjk), if only gene i Is present In genome Gk,
        ''' (iv)  Pij = (pikpjkdk(i, j) (2Nk-dk(i,j)-1))/(Nk(Nk-1)), If genes i And j are present In genome Gk.
        ''' 
        ''' dk(ij) Is the number of genes between gi And gj; Nk Is the number Of genes In genome Gk; And pik Is the probability 
        ''' that gene gi Is present In genome Gk.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="gi$"></param>
        ''' <param name="gj$"></param>
        ''' <param name="G"></param>
        ''' <param name="P"></param>
        ''' <returns></returns>
        Public Function L(Of T As IGeneBrief)(gi$, gj$, G As GenomeContext(Of T), P As Dictionary(Of String, Double)) As Double
            If G.Absent(gi) AndAlso G.Absent(gj) Then
                Return (1 - P(gi)) * (1 - P(gj))
            ElseIf Not G.Absent(gi) AndAlso Not G.Absent(gj) Then
                Dim d = G.Delta(gi, gj)
                Dim N = G.N
                Return (P(gi) * P(gj) * d * (2 * N - d - 1)) / (N * (N - 1))
            ElseIf Not G.Absent(gj) Then
                Return (1 - P(gi)) * P(gj)
            ElseIf Not G.Absent(gi) Then
                Return P(gi) * (1 - P(gj))
            Else
                ' ???
                Throw New NotImplementedException("This exception is never happend!")
            End If
        End Function

        ''' <summary>
        ''' ``pik`` was calculated as the frequency Of gene ``gi`` present In the phylum that ``Gk`` belongs to.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="i$">The feature ``gi``</param>
        ''' <param name="genomes">phylum, the collection of ``Gk``</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function P(Of T As IGeneBrief)(i$, genomes As GenomeContext(Of T)()) As Double
            Return genomes _
                .Where(Function(g) Not g.GetByFeature(i).IsNullOrEmpty) _
                .Count / genomes.Length
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function P(Of T As IGeneBrief)(genomes As GenomeContext(Of T)()) As Dictionary(Of String, Double)
            Return genomes _
                .Select(Function(g) g.AllFeatureKeys) _
                .IteratesALL _
                .Distinct _
                .OrderBy(Function(f) f) _
                .ToDictionary(Function(feature) feature,
                              Function(i) P(i, genomes))
        End Function

        ''' <summary>
        ''' For the Hamming distance between two genes A And B, we sum the number Of times that only A Or B Is found in the genome, 
        ''' DH=Sum([1,n], di)‚ where n Is the number of genomes, di=0 if the orthologs of A And B are both present Or both absent in genome i, 
        ''' And di = 1 otherwise.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="A$"></param>
        ''' <param name="B$"></param>
        ''' <param name="genomes"></param>
        ''' <returns></returns>
        Public Function DHamming(Of T As IGeneBrief)(A$, B$, genomes As GenomeContext(Of T)()) As Integer
            Dim D%

            For Each i In genomes
                If i.Absent(A) AndAlso i.Absent(B) Then
                    D += 0
                ElseIf Not i.Absent(A) AndAlso Not i.Absent(B) Then
                    D += 0
                Else
                    D += 1
                End If
            Next

            Return D
        End Function

        ''' <summary>
        ''' The score is calculated as the natural log of the length ratio of upstream gene And downstream gene, 
        ''' Or L = ln(li/lj), j = i + 1, whereas li And lj are the length of the genes.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="i%"></param>
        ''' <param name="genome"></param>
        ''' <returns></returns>
        Public Function LengthRatio(Of T As IGeneBrief)(i%, genome As GenomeContext(Of T)) As Double
            Dim j = i + 1
            Dim gi = genome(i), gj = genome(j)
            Dim l = stdNum.Log(gi.Length / gj.Length)
            Return l
        End Function
    End Module
End Namespace
