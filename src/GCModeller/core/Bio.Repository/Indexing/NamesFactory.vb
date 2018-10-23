Public Interface NamesFactory(Of T)

    ''' <summary>
    ''' 从该对象之中提取出名字的方法
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    Function GetNames(obj As T) As String()
    ''' <summary>
    ''' 这个函数是index的数据源
    ''' </summary>
    ''' <returns></returns>
    Function PopulateObjects() As IEnumerable(Of T)

End Interface

