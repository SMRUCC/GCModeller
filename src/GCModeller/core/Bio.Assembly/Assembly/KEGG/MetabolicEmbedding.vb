Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Namespace Assembly.KEGG

    Public Class MetabolicEmbedding

        ReadOnly ec_numbers As New Dictionary(Of String, List(Of BriteTerm))

        Sub New()
            Dim maps = BriteHText.Load_ko00001.Deflate("\d+").ToArray

            For Each ko As BriteTerm In maps
                Dim term = KOrthology.ParseTerm(ko.kegg_id, ko.entry.Value)

                For Each id As String In term.EC_number
                    If Not ec_numbers.ContainsKey(id) Then
                        Call ec_numbers.Add(id, New List(Of BriteTerm))
                    End If

                    Call ec_numbers(id).Add(ko)
                Next
            Next
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function MetabolicKeys(level As Integer) As IEnumerable(Of String)
            Return BriteHText.Load_ko00001 _
                .Deflate("\d+") _
                .Select(Function(term)
                            Select Case level
                                Case 1 : Return term.class
                                Case 2 : Return term.category
                                Case Else : Return term.subcategory
                            End Select
                        End Function) _
                .Distinct
        End Function

        Public Function MakeVector(ec As IEnumerable(Of String), Optional level As Integer = 2) As Dictionary(Of String, Double)
            Dim vec As New Dictionary(Of String, Double)

            Call vec.Add("Unknown", 0)

            For Each ec_number As String In ec.SafeQuery
                If ec_numbers.ContainsKey(ec_number) Then
                    For Each term As BriteTerm In ec_numbers(ec_number)
                        Dim key As String

                        Select Case level
                            Case 1 : key = term.class
                            Case 2 : key = term.category
                            Case Else : key = term.subcategory
                        End Select

                        If Not vec.ContainsKey(key) Then
                            Call vec.Add(key, 0)
                        End If

                        vec(key) += 1
                    Next
                Else
                    vec!Unknown += 1
                End If
            Next

            Return vec
        End Function

    End Class
End Namespace