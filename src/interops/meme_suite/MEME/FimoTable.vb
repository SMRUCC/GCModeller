Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

''' <summary>
''' Motif在序列上面的搜索结果
''' </summary>
''' <remarks></remarks>
Public Class FimoTable

    Public Property Motif As String
    <Column("Sequence Name")> Public Property Title As String
    Public Property Strand As String
    Public Property Start As Integer
    Public Property [End] As Integer
    <Column("p-value")> Public Property pValue As Double
    <Column("q-value")> Public Property qValue As Double
    <Column("Matched Sequence")> Public Property Matched As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Table"></param>
    ''' <param name="anno"><see cref="SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo"></see>Csv文件的文件路径</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function MatchInterGeneLoci(Table As String, anno As String) As FimoTable()
        Dim FiMOTable = Table.LoadCsv(Of FimoTable)(False).ToArray
        Throw New NotImplementedException
    End Function
End Class
