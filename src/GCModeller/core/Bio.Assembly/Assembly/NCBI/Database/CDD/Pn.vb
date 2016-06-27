Namespace Assembly.NCBI.CDD

    Public Class Pn : Implements Generic.IEnumerable(Of String)

        ''' <summary>
        ''' The file list of the sub database in this CDD database.
        ''' (这个数据库子库的文件列表)
        ''' </summary>
        ''' <remarks></remarks>
        Dim FileList As String()
        ''' <summary>
        ''' The file path of this pn file.
        ''' (这个pn文件的文件路径)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend FilePath As String
        Protected Friend DIR As String

        Default Public Property File(Index As Integer) As String
            Get
                Return FileList(Index)
            End Get
            Set(value As String)
                FileList(Index) = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return FilePath
        End Function

        Public Shared Narrowing Operator CType(pn As Pn) As String()
            Return pn.FileList
        End Operator

        Public Shared Narrowing Operator CType(pn As Pn) As String
            Return pn.FilePath
        End Operator

        Public Shared Widening Operator CType(File As String) As Pn
            Return New Pn With {
                .FileList = IO.File.ReadAllLines(File),
                .FilePath = File,
                .DIR = FileIO.FileSystem.GetParentPath(File)
            }
        End Operator

        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For i As Integer = 0 To FileList.Length - 1
                Yield DIR & "/" & FileList(i)
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace