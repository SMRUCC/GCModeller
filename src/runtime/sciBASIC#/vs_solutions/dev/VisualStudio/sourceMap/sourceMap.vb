Public Class sourceMap

    Public Property version As Integer
    Public Property file As String
    Public Property sourceRoot As String
    Public Property sources As String()
    Public Property names As String()
    Public Property mappings As String

End Class

''' <summary>
''' 
''' </summary>
Public Class mappingLine

    ''' <summary>
    ''' 第一位，表示这个位置在（转换后的代码的）的第几列。
    ''' </summary>
    ''' <returns></returns>
    Public Property targetCol As Integer
    ''' <summary>
    ''' 第二位，表示这个位置属于sources属性中的哪一个文件。
    ''' </summary>
    ''' <returns></returns>
    Public Property fileIndex As Integer
    ''' <summary>
    ''' 第三位，表示这个位置属于转换前代码的第几行。
    ''' </summary>
    ''' <returns></returns>
    Public Property sourceLine As Integer
    ''' <summary>
    ''' 第四位，表示这个位置属于转换前代码的第几列。
    ''' </summary>
    ''' <returns></returns>
    Public Property sourceCol As Integer
    ''' <summary>
    ''' 第五位，表示这个位置属于names属性中的哪一个变量。
    ''' </summary>
    ''' <returns></returns>
    Public Property nameIndex As Integer

    Public Overrides Function ToString() As String
        Return MyBase.ToString()
    End Function
End Class