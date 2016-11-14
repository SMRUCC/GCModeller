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
            site.ID = loci.UserTag

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
        ''' <paramref name="site"/> at the end of nt sequence join with <paramref name="join"/> location to consist a completed gene. 
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
            Dim out As New SimpleSegment With {
                .Strand = If(site.Strand = Strands.Forward, "+", "-"),
                .ID = site.UserTag,
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
    End Module
End Namespace