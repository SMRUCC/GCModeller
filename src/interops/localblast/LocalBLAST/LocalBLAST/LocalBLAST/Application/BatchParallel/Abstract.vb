Namespace LocalBLAST.Application.BatchParallel

    ''' <summary>
    ''' The formatdb and blast operation should be include in this function pointer.(在这个句柄之中必须要包含有formatdb和blast这两个步骤)
    ''' </summary>
    ''' <param name="Query"></param>
    ''' <param name="Subject"></param>
    ''' <param name="Evalue"></param>
    ''' <param name="Export"></param>
    ''' <returns>返回blast的日志文件名</returns>
    ''' <remarks></remarks>
    Public Delegate Function BlastInvoker(query$, subject$, num_threads%, evalue$, EXPORT$, [overrides] As Boolean) As String
End Namespace