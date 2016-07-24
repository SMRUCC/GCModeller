Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.Metagenome.gast

Public Class BIOM : Inherits ClassObject

    Public Property id As String
    Public Property format As String
    Public Property format_url As String
    Public Property type As String
    Public Property generated_by As String = "GCModeller"
    Public Property [date] As String
    Public Property matrix_type As String
    Public Property matrix_element_type As String
    Public Property shape As Integer()
    Public Property data As Integer()()
    Public Property rows As row()
    Public Property columns As column()

    Public Shared Function [Imports](source As IEnumerable(Of Names)) As BIOM
        Dim array As Names() = source.ToArray

        Return New BIOM With {
            .id = Guid.NewGuid.ToString,
            .format = "Biological Observation Matrix 1.0.0",
            .format_url = "http://biom-format.org",
            .type = "OTU table",
            .generated_by = "GCModeller",
            .date = Now.ToString,
            .matrix_type = "sparse",
            .matrix_element_type = "int",
            .shape = {array.Length, 4}
        }
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
    Public Property metadata As String
End Class