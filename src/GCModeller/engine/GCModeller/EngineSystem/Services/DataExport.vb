Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Namespace EngineSystem.Services

    ''' <summary>
    ''' 数据库中的计算数据导出服务
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataExport : Inherits EngineSystem.Services.MySQL.Service
        Implements System.IDisposable

        Dim FetchedData As DataFlowF()
        Dim [Handles] As HandleF()

        Public Sub Connect(uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
            MYSQL = uri
        End Sub

        ''' <summary>
        ''' The sql command text for load the data from the database server.(从数据库服务器之中加载数据的SQL命令语句)
        ''' </summary>
        ''' <remarks></remarks>
        Public Const SQL_DATA_FETCH As String = "SELECT * FROM {0};"
        Public Const SQL_HANDLE_FETCH As String = "SELECT * FROM {0}_handles;"
        Public Const SQL_FETCH_TABLES As String = "SELECT * FROM storages;"

        ''' <summary>
        ''' 从指定名称的数据表之中加载数据，当指定名称的数据表不存在的时候，则返回0(会生成一个空的CSV文件)
        ''' </summary>
        ''' <param name="TableName"></param>
        ''' <returns>返回所获取的数据的行数</returns>
        ''' <remarks></remarks>
        Public Function FetchData(TableName As String) As Long
            Dim SQL As String = String.Format(SQL_DATA_FETCH, TableName)
            Dim Table = MYSQL.Fetch(SQL)

            If Table Is Nothing Then Return 0

            Dim LQuery = From row As System.Data.DataRow In Table.Tables(0).Rows Select New DataFlowF With {
                         .Id = CType(row.Item(columnName:="id"), Long),
                         .Handle = CType(row.Item(columnName:="handle"), Integer),
                         .Time = CType(row.Item(columnName:="time"), Integer),
                         .Value = CType(row.Item(columnName:="value"), Double)} '
            FetchedData = LQuery.ToArray
            Table = MYSQL.Fetch(SQL:=String.Format(SQL_HANDLE_FETCH, TableName))
            Dim LQuery2 = From row As System.Data.DataRow In Table.Tables(0).Rows Let Element = New HandleF With {
                          .Handle = CType(row.Item(columnName:="handle"), Long),
                          .Identifier = CType(row.Item(columnName:="unique_id"), String)}
                          Select Element Order By Element.Handle Ascending  '
            [Handles] = LQuery2.ToArray
            Return FetchedData.Count
        End Function

        Public Function GetStorageTables() As String()
            Dim Table = MYSQL.Fetch(SQL_FETCH_TABLES)

            If Table Is Nothing Then
                Return New String() {}
            End If

            Return (From row As System.Data.DataRow In Table.Tables(0).Rows Select CType(row.Item(columnName:="storage_table"), String)).ToArray
        End Function

        Public Function Export() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            If Me.FetchedData.IsNullOrEmpty Then
                Return New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Else
                Return Export(Data:=Me.FetchedData, [Handles]:=Me.Handles)
            End If
        End Function

        Private Shared Function Export(Data As IEnumerable(Of DataFlowF), [Handles] As IEnumerable(Of HandleF)) As DocumentStream.File
            Dim CsvData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Dim Time As Integer() = (From row In Data.AsParallel Let i = row.Time Select i Distinct Order By i Ascending).ToArray

            Dim HeadRow As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject From {"#TIME"}
            Call HeadRow.AddRange((From row In [Handles] Select row.Identifier).ToArray) '名称按照句柄值进行排序
            Call CsvData.AppendLine(HeadRow)

            For Each TimePoint In Time
                Dim Query = From row In Data.AsParallel Where row.Time = TimePoint Select row Order By row.Handle Ascending '按照句柄值进行排序
                Dim Line As String() = (From e In Query.ToArray Select CStr(e.Value)).ToArray
                Dim row2 As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject

                Call row2.Add(TimePoint)
                Call row2.AddRange(Line)
                Call CsvData.AppendLine(row2)
            Next

            Return CsvData
        End Function

        Public Shared Function Export(Service As EngineSystem.Services.DataAcquisition.DataSerializer.Csv) As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File
            Return Export(Data:=Service._DataFlows, [Handles]:=Service.GetHandles)
        End Function
    End Class
End Namespace