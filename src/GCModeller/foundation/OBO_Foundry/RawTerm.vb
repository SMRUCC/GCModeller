Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Public Structure RawTerm

    ''' <summary>
    ''' Example: ``[Term]``
    ''' </summary>
    ''' <returns></returns>
    Public Property Type As String
    Public Property data As NamedValue(Of String())()

    Public Function GetData() As Dictionary(Of String, String())
        Return data.ToDictionary(Function(x) x.Name, Function(x) x.x)
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure
