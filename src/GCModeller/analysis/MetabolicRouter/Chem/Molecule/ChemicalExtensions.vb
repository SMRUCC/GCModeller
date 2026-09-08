Namespace Chem

    Module ChemicalExtensions

        Public Function ValenceOf(el As String, charge As Int32) As Int32
            Dim v As Int32
            Select Case el
                Case "C" : v = 4
                Case "N" : v = 3
                Case "O" : v = 2
                Case "S" : v = 6
                Case "P" : v = 5
                Case "F", "Cl", "Br", "I" : v = 1
                Case "B" : v = 3
                Case "H" : v = 1
                Case Else : v = 4
            End Select
            If el = "N" Then
                If charge > 0 Then
                    v = 4
                ElseIf charge < 0 Then
                    v = 2
                End If
            ElseIf el = "O" Then
                If charge > 0 Then
                    v = 3
                ElseIf charge < 0 Then
                    v = 1
                End If
            ElseIf el = "C" AndAlso charge <> 0 Then
                v = 3
            End If
            Return v
        End Function
    End Module
End Namespace