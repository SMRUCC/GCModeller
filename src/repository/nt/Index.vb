Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' 序列数据的索引服务
''' </summary>
Public Class Index : Inherits IndexAbstract

    ''' <summary>
    ''' 索引文件的文件路径
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property URI As String

    ''' <summary>
    ''' 序列文件的文件句柄
    ''' </summary>
    ReadOnly __handle As StreamReader
    ''' <summary>
    ''' 序列数据的读取范围
    ''' </summary>
    ReadOnly __index As New SortedDictionary(Of String, BlockRange)

    Public Structure BlockRange

        Dim start&, end&

        Public Overrides Function ToString() As String
            Return $"{start} --> {end&}"
        End Function
    End Structure

    Sub New(uid$, Data$)
        Dim path$ = $"{Data}/{uid}.nt"
        __handle = New StreamReader(File.OpenRead(path), Encoding.ASCII)
    End Sub

    Public Sub MakeIndex()

    End Sub
End Class
