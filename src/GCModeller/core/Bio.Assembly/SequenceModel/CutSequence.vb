#Region "Microsoft.VisualBasic::96c0cae38f930675e45d75db450d4293, ..\GCModeller\core\Bio.Assembly\SequenceModel\CutSequence.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Runtime.CompilerServices
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
        <Extension>
        Public Function CutSequenceLinear(seq As I_PolymerSequenceModel, site As Location) As SimpleSegment
            Return CutSequenceLinear(seq, site.Left, site.Right, site.ToString)
        End Function

        ''' <summary>
        ''' 核酸分子和蛋白质分子都适用
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CutSequenceLinear(seq As I_PolymerSequenceModel, left%, right%, Optional tag$ = Nothing) As SimpleSegment
            Dim l As Integer = (right - left) + 1
            Dim cut$ = Mid(seq.SequenceData, left, l)

            Return New SimpleSegment With {
                .SequenceData = cut,
                .ID = tag,
                .Start = left,
                .Ends = right,
                .Strand = "?"
            }
        End Function

        <Extension>
        Public Function CutSequenceBylength(seq As I_PolymerSequenceModel, left%, length%, Optional tag$ = Nothing) As SimpleSegment
            Dim cut$ = Mid(seq.SequenceData, left, length)

            Return New SimpleSegment With {
                .SequenceData = cut,
                .ID = tag,
                .Start = left,
                .Ends = length + left - 1,
                .Strand = "?"
            }
        End Function

        <Extension>
        Public Function ReadComplement(seq As I_PolymerSequenceModel, left%, length%, Optional tag$ = Nothing) As SimpleSegment
            Dim cut$ = Mid(seq.SequenceData, left, length)
            cut = New String(NucleicAcid.Complement(cut).Reverse.ToArray)

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
        Public Function CutSequenceLinear(seq As I_PolymerSequenceModel, loci As NucleotideLocation) As SimpleSegment
            Dim site As SimpleSegment = seq.CutSequenceLinear(site:=loci)

            site.Strand = If(loci.Strand = Strands.Forward, "+", "-")
            site.ID = If(
                loci.UserTag.IsBlank,
                loci.NCBIstyle,
                loci.UserTag)

            If loci.Strand = Strands.Forward Then
                Return site
            Else
                ' 反向的链，则还需要反向互补
                site.Complement = site.SequenceData
                site.SequenceData = New String(
                    NucleicAcid.Complement(site.SequenceData) _
                    .Reverse _
                    .ToArray)

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
        Public Function CutSequenceCircular(seq As I_PolymerSequenceModel, site As NucleotideLocation) As SimpleSegment
            Dim [join] As NucleotideLocation
            Dim ntLen% = seq.SequenceData.Length

            If site.Left < 0 Then
                join = New NucleotideLocation(ntLen + site.Left, ntLen, site.Strand)
                site = New NucleotideLocation(1, site.Right)

                Return seq.CutSequenceCircular(join, site)
            ElseIf site.Right > ntLen Then
                join = New NucleotideLocation(1, site.Right - ntLen, site.Strand)
                site = New NucleotideLocation(site.Left, ntLen)

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
        Public Function CutSequenceCircular(seq As I_PolymerSequenceModel,
                                            site As NucleotideLocation,
                                            join As NucleotideLocation) As SimpleSegment

            Dim a As SimpleSegment = seq.CutSequenceLinear(site:=site)
            Dim b As SimpleSegment = seq.CutSequenceLinear(site:=join)
            Dim tag$ = $"{site.Left}..{site.Right} join {join.Left}..{join.Right}"
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

                    .Complement = .SequenceData
                    .SequenceData = New String(
                        NucleicAcid _
                        .Complement(.SequenceData) _
                        .Reverse _
                        .ToArray)

                End With

                Return out
            End If
        End Function

        <Extension>
        Public Function CutSequenceCircular(seq As I_PolymerSequenceModel, site%, join%) As SimpleSegment
            Return seq.CutSequenceCircular(
                New NucleotideLocation(site, seq.SequenceData.Length),
                New NucleotideLocation(1, join))
        End Function
    End Module
End Namespace
