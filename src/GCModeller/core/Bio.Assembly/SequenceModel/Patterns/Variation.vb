
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceModel.Patterns

    Public Class Variation

        Public Enum Variations
            ''' <summary>
            ''' No changes
            ''' </summary>
            NULL = 0
            NA
            NG
            NC
            NT
            AG
            AC
            AT
            GA
            GC
            GT
            CA
            CG
            CT
            TA
            TG
            TC
        End Enum

        Public Property Nt As SeqValue(Of Dictionary(Of String, Variations))()

        Public Shared Function Build(source As FastaFile, ref$) As Variation
            Try
                Return Build(source, source.Index(ref$))
            Catch ex As Exception
                Throw New Exception("ref:=" & ref, ex)
            End Try
        End Function

        Public Shared Function Build(source As FastaFile, ref%) As Variation
            If ref < 0 Then
                Throw New EvaluateException($"Reference index value:={ref} is not a valid index!")
            End If

            Dim refFa As FastaToken = source(ref)

            Call source.RemoveAt(ref)
            Call $"Using {refFa.Title} as references...".__DEBUG_ECHO

            Dim refSeq$ = refFa.SequenceData.ToUpper.Replace("N", "-")
            Dim out As SeqValue(Of Dictionary(Of String, Variations))() =
                New SeqValue(Of Dictionary(Of String, Variations))(refSeq.Length - 1) {}

            For i As Integer = 0 To out.Length - 1
                out(i) = New SeqValue(Of Dictionary(Of String, Variations)) With {
                    .i = i,
                    .obj = New Dictionary(Of String, Variations)
                }
            Next

            Dim array As NamedValue(Of Char())() =
 _
                LinqAPI.Exec(Of NamedValue(Of Char())) <=
 _
                From seq As FastaToken
                In source
                Let trimSeq As String = seq.SequenceData _
                    .ToUpper _
                    .Replace("N", "-")
                Select New NamedValue(Of Char()) With {
                    .Name = seq.Title,
                    .x = trimSeq.ToArray
                }

            For i As Integer = 0 To out.Length - 1
                Dim refC As Char = refSeq(i)

                For Each seq As NamedValue(Of Char()) In array
                    out(i).obj.Add(seq.Name, Variation(refC, seq.x(i)))
                Next
            Next

            Return New Variation With {
                .Nt = out
            }
        End Function

        Public Shared Function Variation(ref As Char, v As Char) As Variations
            Select Case ref
                Case "-"
                    Select Case v
                        Case "-" : Return Variations.NULL
                        Case "A" : Return Variations.NA
                        Case "T" : Return Variations.NT
                        Case "G" : Return Variations.NG
                        Case "C" : Return Variations.NC
                        Case Else
                            Throw New Exception("nt is not valid: " & ref)
                    End Select
                Case "A"
                    Select Case v
                        Case "-" : Return Variations.NA
                        Case "A" : Return Variations.NULL
                        Case "T" : Return Variations.AT
                        Case "G" : Return Variations.AG
                        Case "C" : Return Variations.AC
                        Case Else
                            Throw New Exception("nt is not valid: " & ref)
                    End Select
                Case "T"
                    Select Case v
                        Case "-" : Return Variations.NT
                        Case "A" : Return Variations.TA
                        Case "T" : Return Variations.NULL
                        Case "G" : Return Variations.TG
                        Case "C" : Return Variations.TC
                        Case Else
                            Throw New Exception("nt is not valid: " & ref)
                    End Select
                Case "G"
                    Select Case v
                        Case "-" : Return Variations.NG
                        Case "A" : Return Variations.GA
                        Case "T" : Return Variations.GT
                        Case "G" : Return Variations.NULL
                        Case "C" : Return Variations.GC
                        Case Else
                            Throw New Exception("nt is not valid: " & ref)
                    End Select
                Case "C"
                    Select Case v
                        Case "-" : Return Variations.NC
                        Case "A" : Return Variations.CA
                        Case "T" : Return Variations.CT
                        Case "G" : Return Variations.CG
                        Case "C" : Return Variations.NULL
                        Case Else
                            Throw New Exception("nt is not valid: " & ref)
                    End Select
                Case Else
                    Throw New Exception("Reference nt is not valid: " & ref)
            End Select
        End Function

        Public Shared Function Build(source As IEnumerable(Of FastaToken), ref$) As Variation
            Return Build(New FastaFile(source), ref$)
        End Function
    End Class
End Namespace