
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.Medical

    Public Class DrugGroup : Implements INamedValue

        Public Property Entry As String Implements INamedValue.Key
        Public Property Names As String()
        Public Property Members As String()
        Public Property Remarks As String()
        Public Property Comments As String
        Public Property Targets As String()
        Public Property [Class] As NamedCollection(Of String)()
        Public Property Metabolism As NamedValue(Of String)()
        Public Property Interaction As NamedValue(Of String)()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace