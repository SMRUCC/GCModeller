Imports Microsoft.VisualBasic.Language.UnixBash

Namespace ComponentModel

    ''' <summary>
    ''' NCBI PTT和MetaCyc数据库所公用的多文件的数据库加载器的基本类型
    ''' </summary>
    Public MustInherit Class TabularLazyLoader

        Protected _DIR As String, _filters As String()

        Sub New(DIR As String, filter As String())
            _filters = filter
            _DIR = DIR
        End Sub

        Protected Function __getFiles(filter As String) As IEnumerable(Of String)
            Return ls - l - wildcards(filter) <= _DIR
        End Function

        Protected Function __getFiles(filters As String()) As IEnumerable(Of String)
            Return ls - l - wildcards(filters) <= _DIR
        End Function

        Public Overrides Function ToString() As String
            If Not FileIO.FileSystem.DirectoryExists(_DIR) Then
                Return MyBase.ToString
            End If
            Dim files As String() = __getFiles(_filters).ToArray
            Return String.Format("Stream://{0} {1}", _DIR, String.Join("; ", files))
        End Function
    End Class
End Namespace