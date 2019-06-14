Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DecisionTree

    ''' <summary>
    ''' Node attribute value
    ''' </summary>
    Public Class NodeAttr

        Public Property name As String
        Public Property differentAttributeNames As List(Of String)
        Public Property informationGain As Double

        Public Sub New(name As String, differentAttributenames As List(Of String))
            Me.name = name
            Me.differentAttributeNames = differentAttributenames
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return $"Dim {name} As {differentAttributeNames.GetJson} = {informationGain}"
        End Function

        Public Shared Function GetDifferentAttributeNamesOfColumn(data As DataTable, columnIndex As Integer) As List(Of String)
            Dim differentAttributes As New List(Of String)()

            For i As Integer = 0 To data.Rows.Count - 1
                Dim index = i
                Dim found = differentAttributes.Any(Function(t) t.ToUpper().Equals(data.Rows(index)(columnIndex).ToString().ToUpper()))

                If Not found Then
                    differentAttributes.Add(data.Rows(i)(columnIndex).ToString())
                End If
            Next

            Return differentAttributes
        End Function
    End Class
End Namespace