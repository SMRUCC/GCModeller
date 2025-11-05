#Region "Microsoft.VisualBasic::7a0d3a95e6a1b6d8c65cde25cf4281af, meme_suite\MEME.DocParser\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: (+2 Overloads) __createObject, __toSites, MASTSites, TraceName
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MAST
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Public Module Extensions

    ReadOnly __doc As String() = {"meme.txt", "meme.xml", "meme.html", "mast.txt", "mast.xml", "mast.html"}

    <Extension> Public Function TraceName(path As String) As String
        Dim name As String = path.BaseName.ToLower
        If Array.IndexOf(__doc, name) > -1 Then
            Return path.ParentDirName
        Else
            Return name
        End If
    End Function

    <Extension>
    Public Function MASTSites(mast As XmlOutput.MAST.MAST) As SimpleSegment()
        Dim name As String = mast.Motifs.BriefName
        Dim result = mast.Sequences.SequenceList.Select(Function(x) x.__toSites(name))
        Dim sites As SimpleSegment() = result.IteratesALL.ToVector
        Return sites
    End Function

    <Extension>
    Private Function __toSites(seq As SequenceDescript, familyName As String) As SimpleSegment()()
        Return seq.Segments.Select(Function(loci) __createObject(loci, familyName))
    End Function

    Private Function __createObject(site As Segment, trace As String) As SimpleSegment()
        Dim sequence As String = TrimNewLine(site.SegmentData, "").Replace(vbTab, "").Trim
        Dim sites As SimpleSegment() = site.Hits.Select(Function(hit) __createObject(site.start, hit, sequence, OffSet:=5, trace:=trace)).ToArray
        Return sites
    End Function

    ''' <summary>
    ''' 不做任何筛选，直接导出数据
    ''' </summary>
    ''' <param name="start"></param>
    ''' <param name="hit"></param>
    ''' <param name="sequence"></param>
    ''' <param name="OffSet"></param>
    ''' <param name="trace"></param>
    ''' <returns></returns>
    Private Function __createObject(start%, hit As HitResult, sequence$, OffSet%, trace$) As SimpleSegment
        Dim id As String = hit.motif.Split("_"c).Last
        Dim length As Integer = Len(hit.match) + 2 * OffSet  '为了保证在进行分子生物学实验的时候能够得到完整的片段，在这里将位点的范围扩大了10个bp
        start = hit.pos - start
        start -= OffSet
        If start <= 0 Then
            start = 1
            'Call $"{hit.pos} - {start} is not enough".debug
        End If

        Dim site As String = Mid(sequence, start, length)
        Dim strand As String = hit.strand.GetBriefStrandCode
        Dim strands As Strands = strand.GetStrand
        Dim right As Integer

        If strands = Strands.Forward Then
            right = hit.pos + length
        Else
            right = hit.pos - length
        End If

        '需不需要将反向的序列互补为正向的？？？

        Dim mastSite As New SimpleSegment With {
            .ID = $"{trace}::{id}",
            .SequenceData = site,
            .Start = hit.pos,
            .Strand = strand,
            .Ends = right
        }
        Return mastSite
    End Function

    Public Iterator Function ParsePWNFile(file As String) As IEnumerable(Of MotifPWM)
        Dim lines As String() = file.ReadAllLines
        Dim alphabets As Char() = Nothing
        Dim offset As Integer = 0

        Do While offset <= lines.Length - 1
            Dim line As String = lines(offset)

            With line.GetTagValue("=")
                If .Name = "ALPHABET" Then
                    alphabets = Strings.Trim(.Value).ToArray
                End If
                If line.StartsWith("MOTIF") Then
                    Exit Do
                End If
            End With

            offset += 1
        Loop

        For Each block As String() In lines.Skip(offset) _
            .Where(Function(si) Not si.StringEmpty(, True)) _
            .Split(Function(si)
                       Return si.StartsWith("MOTIF")
                   End Function, deliPosition:=DelimiterLocation.NextFirst) _
            .Where(Function(b)
                       Return Not b.IsNullOrEmpty
                   End Function)

            Dim name As String = block(0)
            Dim notes As String = block(1) & vbCrLf & block.Last

            block = block.Skip(2).Take(block.Length - 2).ToArray
            offset = 1

            Dim matrix As New List(Of ResidueSite)

            For Each line As String In block
                Dim p As Double() = line.Trim.StringSplit("\s+").AsDouble
                Dim row As New ResidueSite With {
                    .Bits = 1,
                    .PWM = p,
                    .Site = offset
                }

                offset += 1
                matrix.Add(row)
            Next

            Yield New MotifPWM With {
                .Alphabets = alphabets,
                .name = name,
                .note = notes,
                .PWM = matrix.ToArray
            }
        Next
    End Function
End Module
