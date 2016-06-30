Public Interface IWeightPaired
    ''' <summary>
    ''' 获取两个指定编号的基因的相关度
    ''' </summary>
    ''' <param name="Id1"></param>
    ''' <param name="Id2"></param>
    ''' <param name="Parallel"></param>
    ''' <returns></returns>
    Function GetValue(Id1 As String, Id2 As String, Optional Parallel As Boolean = True) As Double
End Interface
