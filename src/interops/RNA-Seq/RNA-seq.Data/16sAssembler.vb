#Region "Microsoft.VisualBasic::0a6e1928956e9f35924998a197c4dee6, RNA-Seq\RNA-seq.Data\16sAssembler.vb"

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

    '   Total Lines: 102
    '    Code Lines: 70 (68.63%)
    ' Comment Lines: 13 (12.75%)
    '    - Xml Docs: 61.54%
    ' 
    '   Blank Lines: 19 (18.63%)
    '     File Size: 4.08 KB


    ' Module Assembler
    ' 
    '     Function: SequenceCoverage
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.SAM

Public Module Assembler

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sam$"></param>
    ''' <param name="workspace$"></param>
    ''' <param name="refProvider">
    ''' 函数需要根据参考序列来计算出覆盖度，如果这个接口是空值，则会尝试进行SCS算法序列装配，在再装配好的序列的基础上进行计算操作
    ''' </param>
    ''' <returns></returns>
    Public Function SequenceCoverage(sam$, workspace$, Optional refProvider As Func(Of String(), IEnumerable(Of FastaSeq)) = Nothing) As Dictionary(Of String, Integer)
        Dim reader As New SAMStream(sam)

        Call "Write SAM headers...".info

        Using headWriter = $"{workspace}/head.part".OpenWriter
            For Each header As SAMHeader In reader.IteratesAllHeaders
                If header.TagValue = Tags.SQ Then
                    Call headWriter.WriteLine(header.GenerateDocumentLine)
                End If
            Next
        End Using

        Dim refs As New Dictionary(Of String, IO.StreamWriter)

        Call "Split SAM target file...".info

        For Each read As AlignmentReads In reader _
            .IteratesAllReads _
            .Where(Function(r) Not r.IsUnmappedReads)

            Dim key$ = Mid(read.RNAME, 1, 3)

            ' 可能会处理10GB以上的文件，数据量会非常大
            ' 所以不能够将reads数据都读进入内存中
            ' 在这里将reads缓存到硬盘工作区上的临时文件中
            If Not refs.ContainsKey(key) Then
                refs(key) = $"{workspace}/{key.First}/{key.NormalizePathString}.sam".OpenWriter

                Call Console.WriteLine()
                Call $"Open {key}".info
            Else
                Console.Write("."c)
            End If

            refs(key).WriteLine(read.GenerateDocumentLine)
        Next

        Call "Write SAM file parts...".info

        For Each ref As IO.StreamWriter In refs.Values
            Call ref.Flush()
            Call ref.Close()
            Call ref.Dispose()
        Next

        Call "Calculate Coverage....".info

        ' 下面开始进行装配为contig
        Call (ls - l - r - "*.sam" <= workspace) _
            .AsParallel _
            .Select(Function(path) As Object
                        Dim readsGroup = New SAMStream(path) _
                            .IteratesAllReads _
                            .GroupBy(Function(r) r.RNAME)

                        For Each refer In readsGroup
                            Dim ref$ = refer.Key
                            Dim reads = refer.Select(Function(r) r.SequenceData).AsList
                            Dim contig$ = reads.AsList.ShortestCommonSuperString().First
                            Dim covTxt$ = $"{path.TrimSuffix}/{ref.NormalizePathString}.txt"

                            Using view As IO.StreamWriter = covTxt.OpenWriter
                                Call reads.TableView(contig, view)
                            End Using
                        Next

                        Return Nothing
                    End Function) _
            .ToArray

        Dim coverages As Dictionary(Of String, Integer) =
            (ls - l - r - "*.txt" <= workspace) _
            .ToDictionary(Function(path) path.BaseName,
                          Function(path)
                              Return path _
                                  .IterateAllLines _
                                  .ElementAt(1) _
                                  .GetTagValue("=") _
                                  .Value _
                                  .ParseInteger
                          End Function)
        Return coverages
    End Function
End Module
