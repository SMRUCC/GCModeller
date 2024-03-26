Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO

Public Module GeneName

    <Extension>
    Public Iterator Function GroupBy(genes As IEnumerable(Of EntityObject), field As String, Optional cutoff As Double = 0.8) As IEnumerable(Of NamedCollection(Of EntityObject))

    End Function
End Module
