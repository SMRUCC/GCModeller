Imports System.Text
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.WebCloud.GIS.MaxMind.geolite2

Namespace MaxMind

    Public Module MySqlImports

        Public Function LoadCsv(path As String) As geolite2_city_blocks_ipv4()
            Return path.LoadCsv(Of geolite2_city_blocks_ipv4)(False).ToArray
        End Function

        Public Function LoadCsvCityLocations(path As String) As geolite2_city_locations()
            Return path.LoadCsv(Of geolite2_city_locations)(False).ToArray
        End Function

        Public Function GenerateInsertSQL(data As geolite2_city_locations()) As String
            Return __insertSQLTransaction(data)
        End Function

        ''' <summary>
        ''' 生成用于导入数据库的SQL插入脚本
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Function GenerateInsertSQL(data As geolite2_city_blocks_ipv4()) As String
            Return __insertSQLTransaction(data)
        End Function

        Private Function __insertSQLTransaction(data As IEnumerable(Of SQLTable)) As String
            Dim LQuery = (From entry In data.AsParallel Select entry.GetInsertSQL).ToArray
            Dim sBuilder As New StringBuilder(2 * 2048)

            For Each Line As String In LQuery
                Call sBuilder.AppendLine(Line)
            Next

            Return sBuilder.ToString
        End Function
    End Module
End Namespace