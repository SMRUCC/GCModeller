Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Class IDConvertor

    Sub New(entries As IEnumerable(Of entry))

    End Sub

    Public Overloads Function [GetType](geneSet As String()) As IDTypes

    End Function

    Public Iterator Function Converts(geneSet As String(), type As IDTypes) As IEnumerable(Of EntityObject)

    End Function
End Class
