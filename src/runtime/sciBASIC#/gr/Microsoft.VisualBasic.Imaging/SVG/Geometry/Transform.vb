Imports System.Drawing
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SVG

    Public Class Transform

        ReadOnly transform As New Dictionary(Of String, String())

        Public ReadOnly Property translate As PointF
            Get
                Dim translate_pars As String() = transform.TryGetValue("translate")

                If translate_pars.IsNullOrEmpty Then
                    Return Nothing
                Else
                    Return New PointF(Val(translate_pars(0)), Val(translate_pars(1)))
                End If
            End Get
        End Property

        Friend Sub New(str As String)
            transform = Parse(str) _
                .ToDictionary(Function(a) a.op,
                              Function(a)
                                  Return a.pars
                              End Function)
        End Sub

        Public Overrides Function ToString() As String
            Return transform.GetJson
        End Function

        Private Shared Iterator Function Parse(str As String) As IEnumerable(Of (op$, pars As String()))
            Dim matches = str.Matches("[a-z]+\s*\(\d+(\s+\d+)*\)")

            For Each op As String In matches
                Dim op_name = op.Match("[a-z]+")
                Dim pars = op.GetStackValue("(", ")") _
                    .Split _
                    .Select(AddressOf Strings.Trim) _
                    .Where(Function(si) si.Length > 0) _
                    .ToArray

                Yield (Strings.LCase(op_name), pars)
            Next
        End Function
    End Class
End Namespace