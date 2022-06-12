#Region "Microsoft.VisualBasic::de7392c4ef7be2d7d38f9caf880b03c9, core\Bio.Assembly\SequenceModel\CutSequence.vb"

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

    '     Module CutSequence
    ' 
    '         Function: CutSequenceBylength, (+3 Overloads) CutSequenceCircular, (+4 Overloads) CutSequenceLinear, ReadComplement
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace SequenceModel

    ''' <summary>
    ''' Cut sequence for DNA/protein
    ''' </summary>
    Public Module CutSequence

        ''' <summary>
        ''' 核酸分子和蛋白质分子都适用
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="site"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CutSequenceLinear(seq As IPolymerSequenceModel, site As Location) As SimpleSegment
            Return CutSequenceLinear(seq, site.left, site.right, site.ToString)
        End Function

#Region "Implementation"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="seq$"></param>
        ''' <param name="left">下标是从1开始的</param>
        ''' <param name="right%"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CutSequenceLinear(ByRef seq$, left%, right%) As String
            Dim l As Integer = (right - left) + 1
            Dim start = left - 1
            Dim cut$

            If start >= seq.Length Then
                Return ""
            ElseIf start + l >= seq.Length Then
                cut = seq.Substring(start)
            Else
                ' 计算是从1开始的，不是从零开始的
                If left <= 0 Then
                    Call $"set negative start position({left}) to base 1.".Warning
                    left = 1
                End If

                cut = seq.Substring(left - 1, l)
            End If

            Return cut
        End Function

        ''' <summary>
        ''' 核酸分子和蛋白质分子都适用
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="left">
        ''' 下标是从1开始的
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function CutSequenceLinear(seq As IPolymerSequenceModel, left%, right%, Optional tag$ = Nothing) As SimpleSegment
            Dim cut$ = seq.SequenceData.CutSequenceLinear(left, right)

            Return New SimpleSegment With {
                .SequenceData = cut,
                .ID = tag,
                .Start = left,
                .Ends = right,
                .Strand = "?"
            }
        End Function
#End Region

        <Extension>
        Public Function CutSequenceBylength(seq As IPolymerSequenceModel, left%, length%, Optional tag$ = Nothing) As SimpleSegment
            Dim cut$ = Mid(seq.SequenceData, left, length)

            Return New SimpleSegment With {
                .SequenceData = cut,
                .ID = tag,
                .Start = left,
                .Ends = length + left - 1,
                .Strand = "?"
            }
        End Function

        ''' <summary>
        ''' 将目标序列之中的给定区域的序列剪下，并返回该片段的互补序列
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="left">请注意，这个参数是以1为起始的</param>
        ''' <param name="length%"></param>
        ''' <param name="tag$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ReadComplement(seq As IPolymerSequenceModel, left%, length%, Optional tag$ = Nothing) As SimpleSegment
            Dim cut$ = Mid(seq.SequenceData, left, length)
            cut = NucleicAcid.Complement(cut).Reverse.CharString

            Return New SimpleSegment With {
                .SequenceData = cut,
                .ID = tag,
                .Start = left,
                .Ends = length + left - 1,
                .Strand = "-"
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="loci"></param>
        ''' <returns></returns>
        ''' <remarks>Tested by XC_1184/XC_0012, no problem.</remarks>
        <Extension>
        Public Function CutSequenceLinear(seq As IPolymerSequenceModel, loci As NucleotideLocation) As SimpleSegment
            Dim site As SimpleSegment = seq.CutSequenceLinear(site:=loci)

            site.Strand = If(loci.Strand = Strands.Forward, "+", "-")
            site.ID = loci.tagStr Or loci.NCBIstyle.AsDefault

            If loci.Strand = Strands.Forward Then
                Return site
            Else
                ' 反向的链，则还需要反向互补
                ' site.Complement = site.SequenceData
                site.SequenceData = NucleicAcid.Complement(site.SequenceData) _
                                               .Reverse _
                                               .CharString

                Return site
            End If
        End Function

        ''' <summary>
        ''' 这个函数会自动计算出上半部分的位置
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="site"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CutSequenceCircular(seq As IPolymerSequenceModel, site As NucleotideLocation) As SimpleSegment
            Dim [join] As NucleotideLocation
            Dim ntLen% = seq.SequenceData.Length

            If site.left < 0 Then
                join = New NucleotideLocation(ntLen + site.left, ntLen, site.Strand)
                site = New NucleotideLocation(1, site.right)

                Return seq.CutSequenceCircular(join, site)
            ElseIf site.right > ntLen Then
                join = New NucleotideLocation(1, site.right - ntLen, site.Strand)
                site = New NucleotideLocation(site.left, ntLen)

                Return seq.CutSequenceCircular(site, join)
            Else
                ' 没有超出范围，则直接切割序列
                Return seq.CutSequenceLinear(site)
            End If
        End Function

        ''' <summary>
        ''' <paramref name="site"/> at the end of nt sequence join with <paramref name="join"/> location to consist a completed gene. 
        ''' (请注意，在这里两个位点是直接进行序列拼接的，所以在这里两个参数是有顺序之分的)
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="site">环状的分子只能够是DNA分子，所以这里是核酸序列的位置</param>
        ''' <returns></returns>
        ''' <remarks>Not sure, probably success.</remarks>
        <Extension>
        Public Function CutSequenceCircular(seq As IPolymerSequenceModel,
                                            site As NucleotideLocation,
                                            join As NucleotideLocation) As SimpleSegment

            Dim a As SimpleSegment = seq.CutSequenceLinear(site:=site)
            Dim b As SimpleSegment = seq.CutSequenceLinear(site:=join)
            Dim tag$ = $"{site.left}..{site.right} join {join.left}..{join.right}"
            Dim out As New SimpleSegment With {
                .Strand = If(site.Strand = Strands.Forward, "+", "-"),
                .ID = If(site.Strand = Strands.Forward, tag, $"complement({tag})"),
                .Start = site.Start,
                .SequenceData = a.SequenceData & b.SequenceData,
                .Ends = site.Start + .SequenceData.Length
            }

            If site.Strand = Strands.Forward Then
                Return out
            Else
                ' 反向的链，则还需要反向互补
                With out

                    ' .Complement = .SequenceData
                    .SequenceData = NucleicAcid _
                        .Complement(.SequenceData) _
                        .Reverse _
                        .CharString

                End With

                Return out
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CutSequenceCircular(seq As IPolymerSequenceModel, site%, join%) As SimpleSegment
            Dim a = New NucleotideLocation(site, seq.SequenceData.Length)
            Dim b = New NucleotideLocation(1, join)

            Return seq.CutSequenceCircular(a, b)
        End Function
    End Module
End Namespace
