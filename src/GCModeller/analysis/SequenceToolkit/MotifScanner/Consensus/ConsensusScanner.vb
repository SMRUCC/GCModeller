﻿#Region "Microsoft.VisualBasic::75f8aec9f62f32a0f97d6a79b59a71d0, analysis\SequenceToolkit\MotifScanner\Consensus\ConsensusScanner.vb"

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

    ' Class ConsensusScanner
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: (+2 Overloads) PopulateMotifs
    ' 
    '     Sub: DumpSequence
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 扫描出相同的KO编号的基因的上游区域的片段的相似片段
''' </summary>
Public Class ConsensusScanner

    Dim KOUpstream As Dictionary(Of String, FastaSeq())
    Dim KO As Dictionary(Of String, String())

    Sub New(genomes As IEnumerable(Of genomic), Optional length As PrefixLength = PrefixLength.L300)
        With genomes.ToArray
            Dim upstreams = .Select(Function(g) g.GetUpstreams(length)) _
                            .Select(Function(up)
                                        Return up.ToDictionary(Function(g) g.Key.ToUpper,
                                                               Function(g) g.Value)
                                    End Function) _
                            .ToArray

            KO = .Select(Function(genome)
                             Return genome.organism _
                                 .genome _
                                 .Select(Function(pathway) pathway.genes) _
                                 .IteratesALL _
                                 .Where(Function(g) Not g.text.StringEmpty)
                         End Function) _
                 .IteratesALL _
                 .GroupBy(Function(gene) gene.text.Split.First) _
                 .ToDictionary(Function(id) id.Key,
                               Function(genes)
                                   ' KEGG之中的基因编号都会存在一个物种缩写的前缀
                                   ' 在这里移除掉
                                   Return genes.Keys _
                                       .Select(Function(id) id.Split(":"c).Last.ToUpper) _
                                       .ToArray
                               End Function)
            KOUpstream = KO.ToDictionary(Function(id) id.Key,
                                         Function(consensus)
                                             Dim geneIDs$() = consensus.Value
                                             Dim upstream = geneIDs _
                                                 .Select(Function(ID)
                                                             Dim selected = upstreams _
                                                                 .Where(Function(genome) genome.ContainsKey(ID)) _
                                                                 .FirstOrDefault

                                                             If selected Is Nothing Then
                                                                 Console.WriteLine(ID)
                                                                 Return Nothing
                                                             Else
                                                                 Return selected(ID)
                                                             End If
                                                         End Function) _
                                                 .Where(Function(seq) Not seq Is Nothing) _
                                                 .ToArray
                                             Return upstream
                                         End Function)
        End With
    End Sub

    Public Iterator Function PopulateMotifs(Optional leastN% = 10, Optional param As PopulatorParameter = Nothing) As IEnumerable(Of NamedCollection(Of Motif))
        For Each KO As String In Me.KO.Keys
            Dim motifs = PopulateMotifs(KO, leastN, param) _
                .OrderByDescending(Function(m)
                                       Return m.score / m.seeds.MSA.Length
                                   End Function) _
                .ToArray

            Yield New NamedCollection(Of Motif) With {
                .Name = KO,
                .Value = motifs
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub DumpSequence(KO$, path$)
        Call New FastaFile(KOUpstream(KO)).Save(path, Encoding.ASCII)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PopulateMotifs(KO$, Optional leastN% = 10, Optional param As PopulatorParameter = Nothing) As IEnumerable(Of Motif)
        Return KOUpstream(KO).PopulateMotifs(leastN, param)
    End Function
End Class
