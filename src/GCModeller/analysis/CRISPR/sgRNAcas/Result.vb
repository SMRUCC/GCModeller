Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DocumentFormat.Csv

''' <summary>
''' A.Final_report/report_protospacer_single.txt
''' </summary>
''' <remarks></remarks>
Public Class SingleStrandResult
    Public Property sgRID As String
    Public Property Start As Integer
    Public Property [End] As Integer
    <Column("CRISPR_target_sequence(5'-3')")> Public Property CRISPR_target_sequence As String
    <Column("Length(nt)")> Public ReadOnly Property Length As Integer
        Get
            Return Len(CRISPR_target_sequence)
        End Get
    End Property
    <Column("GC%")> Public Property GC As Double

    Public Overrides Function ToString() As String
        Return String.Join(vbTab, {sgRID, Start, [End], CRISPR_target_sequence, Length, GC & " %"})
    End Function

    Public Shared Function LoadDocument(Path As String) As SingleStrandResult()
        Dim Csv = DataImports.Imports(Path, vbTab)
        Return Csv.AsDataSource(Of SingleStrandResult)(False)
    End Function
End Class
