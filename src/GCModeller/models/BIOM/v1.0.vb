Imports Microsoft.VisualBasic.Serialization.JSON

Namespace v10

    Public Class Json

        Public Property id As String
        Public Property format As String
        Public Property format_url As String
        Public Property type As String
        Public Property generated_by As String = "GCModeller"
        Public Property [date] As Date
        Public Property matrix_type As String
        Public Property matrix_element_type As String
        Public Property shape As Integer()
        Public Property data As Integer()()
        Public Property rows As row()
        Public Property columns As column()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class row
        Public Property id As String
        Public Property metadata As meta
    End Class

    Public Class meta
        Public Property taxonomy As String()

    End Class

    Public Class column
        Public Property id As String
        Public Property metadata As columnMeta
    End Class

    Public Class columnMeta
        Public Property BarcodeSequence As String
        Public Property LinkerPrimerSequence As String
        Public Property BODY_SITE As String
        Public Property Description As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace