Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Public Structure RawTerm

    ''' <summary>
    ''' Example: ``[Term]``
    ''' </summary>
    ''' <returns></returns>
    Public Property Type As String
    Public Property data As NamedValue(Of String())()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure
