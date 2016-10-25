
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.SequenceDump

    ''' <summary>
    ''' The fasta header of the nt database.
    ''' </summary>
    Public Structure NTheader

        Public Const AccessionId$ = "\S+?\d+\.\d+"

        Public gi As String
        Public db As String
        Public uid As String
        Public description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function ParseId(title$) As String
            Dim id$ = Regex.Match(title, AccessionId).Value
            Return id
        End Function

        Public Shared Function ParseNTheader(fa As FastaToken, Optional throwEx As Boolean = True) As IEnumerable(Of NTheader)
            Dim out As New List(Of NTheader)

            Try
                Dim attrs$() = fa.Attributes
                Dim splits$()() = attrs.Skip(1).Split(4%)
                Dim trimGI As Boolean = splits.Length > 1

                For Each b As String() In splits
                    If b.Length = 1 Then
                        Dim x As NTheader = out.Last

                        out(out.Count - 1) = New NTheader With {
                            .db = x.db,
                            .description = x.description & "|" & b(Scan0),
                            .gi = x.gi,
                            .uid = x.uid
                        }

                        Continue For
                    End If

                    Dim descr$ = Strings.Trim(b(3))

                    out += New NTheader With {
                        .gi = b(0),
                        .db = b(1),
                        .uid = b(2),
                        .description = If(trimGI, Trim(descr), descr)
                    }
                Next

                Return out
            Catch ex As Exception
                ex = New Exception(fa.Title, ex)

                If throwEx Then
                    Throw ex
                Else
                    Call App.LogException(ex)
                    Call ex.PrintException

                    Return out
                End If
            End Try
        End Function

        Private Shared Function Trim(s As String) As String
            Dim gi$ = Mid(s, s.Length - 1)
            If String.Equals("gi", gi$, StringComparison.OrdinalIgnoreCase) Then
                s = Mid(s, 1, s.Length - 2)
            End If
            Return s
        End Function
    End Structure
End Namespace