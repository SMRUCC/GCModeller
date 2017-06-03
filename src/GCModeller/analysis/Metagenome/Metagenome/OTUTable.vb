Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Text

''' <summary>
''' 这个对象记录了当前的宏基因组实验之中的每一个OTU在样品之中的含量的多少
''' </summary>
Public Class OTUTable : Inherits DataSet
    
    ''' <summary>
    ''' 这个函数会自动兼容csv或者tsv格式的
    ''' </summary>
    ''' <param name="table$"></param>
    ''' <returns></returns>
    Public Shared Function LoadSample(table$, Optional ID$ = "OTU_ID") As OTUTable()
        Dim fieldMaps As New Dictionary(Of String, String) From {
            {ID, NameOf(OTUTable.ID)}
        }

        If table.ReadFirstLine.Count(ASCII.TAB) > 1 Then
            ' tsv文件
            Return table _
                .ReadAllLines _
                .ImportsTsv(Of OTUTable)(fieldMaps)
        Else
            Return table.LoadCsv(Of OTUTable)(maps:=fieldMaps)
        End If
    End Function
End Class
