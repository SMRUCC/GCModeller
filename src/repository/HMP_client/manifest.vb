Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File
Imports Microsoft.VisualBasic.Data.csv

Public Class manifest

    Public Property file_id As String
    Public Property md5 As String
    Public Property size As Long
    Public Property urls As String
    Public Property sample_id As String

    Public ReadOnly Property HttpURL As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return urls _
                .Split(","c) _
                .Where(Function(url)
                           Return InStr(url, "http://", CompareMethod.Text) = 1 OrElse
                                  InStr(url, "https://", CompareMethod.Text) = 1
                       End Function) _
                .FirstOrDefault
        End Get
    End Property

    ''' <summary>
    ''' 从tsv表格文件之中加载资源数据
    ''' </summary>
    ''' <param name="tsv"></param>
    ''' <returns></returns>
    Public Shared Function LoadTable(tsv As String) As IEnumerable(Of manifest)
        Return csv.LoadTsv(tsv, Encodings.UTF8).AsDataSource(Of manifest)()
    End Function
End Class
