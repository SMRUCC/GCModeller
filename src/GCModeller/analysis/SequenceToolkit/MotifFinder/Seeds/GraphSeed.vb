#Region "Microsoft.VisualBasic::4af458a849805254e458f33a8f75e481, analysis\SequenceToolkit\MotifFinder\Seeds\FileName.vb"

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

    '   Total Lines: 157
    '    Code Lines: 112 (71.34%)
    ' Comment Lines: 19 (12.10%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 26 (16.56%)
    '     File Size: 5.59 KB


    ' Module FileName
    ' 
    '     Function: Cluster, CreateMotifs, (+2 Overloads) RandomSeed
    ' 
    ' Class GraphSeed
    ' 
    '     Properties: embedding, graph, part, source, start
    ' 
    '     Function: GetCompares, ToString, UMAP
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.DataMining.UMAP
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Model.MotifGraph
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Module GraphSeedTool

    ''' <summary>
    ''' bootstrapping sampling
    ''' </summary>
    ''' <param name="seq"></param>
    ''' <param name="n"></param>
    ''' <param name="range"></param>
    ''' <returns></returns>
    Public Iterator Function RandomSeed(seq As FastaSeq, n As Integer, range As IntRange) As IEnumerable(Of GraphSeed)
        Dim seqtitle As String = seq.Title
        Dim seqdata As String = seq.SequenceData

        Call VBDebugger.EchoLine(seq.Title)

        For i As Integer = 0 To n
            Dim klen As Integer = randf.GetNextBetween(range)
            Dim left As Integer = randf.NextInteger(seq.Length - klen)
            Dim part As String = seqdata.Substring(left, klen)

            Yield New GraphSeed With {
                .part = part,
                .start = left,
                .graph = Builder _
                    .SequenceGraph(part, SequenceModel.NT) _
                    .GetVector(SequenceModel.NT),
                .source = seqtitle
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function RandomSeed(seqs As IEnumerable(Of FastaSeq), n As Integer, range As IntRange) As IEnumerable(Of GraphSeed)
        Return seqs.Select(Function(fa) RandomSeed(fa, n, range)).IteratesALL
    End Function

    Public Iterator Function Cluster(seeds As IEnumerable(Of GraphSeed), member As Double) As IEnumerable(Of NamedCollection(Of GraphSeed))
        Dim tree As New AVLClusterTree(Of GraphSeed)(GraphSeed.GetCompares(cluster:=member), views:=Function(a) a.part)
        Dim allSeeds As GraphSeed() = seeds.ToArray
        Dim d As Integer = allSeeds.Length / 100
        Dim i As i32 = Scan0
        Dim t0 As Date = Now

        Call VBDebugger.EchoLine($"run debug for {allSeeds.Length} data seeds!")

        For Each seed As GraphSeed In allSeeds
            Call tree.Add(seed)

            If ++i Mod d = 1 Then
                Call VBDebugger.EchoLine($"[{i}/{allSeeds.Length}  #{(Now - t0).FormatTime}] {(i / allSeeds.Length * 100).ToString("F1")}% [{seed.part}] {seed.source}")
            End If
        Next

        For Each node As ClusterKey(Of GraphSeed) In tree.AsEnumerable
            If node.NumberOfKey > 2 Then
                Yield New NamedCollection(Of GraphSeed)("", node.ToArray)
            End If
        Next
    End Function

    <Extension>
    Public Function CreateMotifs(node As NamedCollection(Of GraphSeed), param As PopulatorParameter) As SequenceMotif
        Dim pwm = node.Select(Function(a) a.part).BuildMotifPWM(param)

        If Not pwm Is Nothing Then
            pwm = pwm.Cleanup

            If pwm Is Nothing Then
                Return Nothing
            End If

            Dim nsig = pwm.SignificantSites

            If pwm.score > 0 AndAlso nsig >= param.significant_sites Then
                If nsig / pwm.width > 0.35 Then
                    If nsig / pwm.width <= 0.8 Then
                        Return pwm
                    End If
                End If
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If

        Return Nothing
    End Function

End Module

Public Class GraphSeed

    ''' <summary>
    ''' the part of the sequence
    ''' </summary>
    ''' <returns></returns>
    Public Property part As String
    Public Property start As Integer
    ''' <summary>
    ''' the raw graph data
    ''' </summary>
    ''' <returns></returns>
    Public Property graph As Double()
    ''' <summary>
    ''' the raw <see cref="graph"/> vector umap embedding data result
    ''' </summary>
    ''' <returns></returns>
    Public Property embedding As Double()
    Public Property source As String

    Public Overrides Function ToString() As String
        Return part
    End Function

    Public Shared Function GetCompares(cluster As Double) As Comparison(Of GraphSeed)
        Return Function(a, b)
                   Dim score As Double = SSM(a.graph, b.graph)

                   If score >= cluster Then
                       Return 0
                   ElseIf score > 0 Then
                       Return 1
                   Else
                       Return -1
                   End If
               End Function
    End Function

    Public Shared Iterator Function UMAP(seeds As GraphSeed(), ndims As Integer) As IEnumerable(Of GraphSeed)
        Dim manifold As New Umap(AddressOf DistanceFunctions.CosineForNormalizedVectors, dimensions:=ndims)
        Dim x As Double()() = seeds.Select(Function(a) a.graph).ToArray
        Dim nloops As Integer

        nloops = manifold.InitializeFit(x)
        manifold = manifold.Step(nloops)
        x = manifold.GetEmbedding

        For i As Integer = 0 To seeds.Length - 1
            seeds(i).embedding = x(i)
            Yield seeds(i)
        Next
    End Function
End Class
