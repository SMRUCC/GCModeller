Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Restriction_enzyme

    Public Module TranslatePatterns

        ReadOnly debase As New DegenerateBasesExtensions

        ''' <summary>
        ''' translate the recongnition site string as regular expression motif pattern data.
        ''' </summary>
        ''' <param name="enzyme"></param>
        ''' <returns></returns>
        <Extension>
        Public Function TranslateRegular(enzyme As Enzyme, Optional direction As Strands = Strands.Forward) As MotifPattern
            Dim site As String = enzyme.Recognition(direction)
            Dim regular As String = TranslateRegular(site)
            Dim motif As New MotifPattern With {
                .Id = enzyme.Enzyme,
                .Motif = enzyme.Enzyme,
                .Width = site.Length,
                .Expression = regular
            }

            Return motif
        End Function

        Private Function TranslateRegular(site As String) As String
            Dim sb As New StringBuilder
            Dim tmp As New List(Of Char)

            For Each c As Char In site
                If debase.DegenerateBases.ContainsKey(c) Then
                    If tmp.Any Then
                        Call sb.Append(TranslateRegular(tmp))
                    End If

                    Call tmp.Add(c)
                ElseIf Char.IsDigit(c) Then
                    ' is number/digit
                    Call tmp.Add(c)
                Else
                    If tmp.Any Then
                        Call sb.Append(TranslateRegular(tmp))
                    End If

                    Call sb.Append(c)
                End If
            Next

            If tmp.Any Then
                Call sb.Append(TranslateRegular(tmp))
            End If

            Return sb.ToString
        End Function

        Private Function TranslateRegular(ByRef tmp As List(Of Char)) As String
            Dim r As String

            If Char.IsDigit(tmp.First) Then
                r = $"{{{tmp.CharString}}}"
            ElseIf tmp.Count = 1 Then
                r = "[" & debase.DegenerateBases(tmp(0)).JoinBy("") & "]"
            Else
                r = $"[{debase.DegenerateBases(tmp(0)).JoinBy("")}]{{{tmp.Skip(1).CharString}}}"
            End If

            Call tmp.Clear()

            Return r
        End Function
    End Module
End Namespace