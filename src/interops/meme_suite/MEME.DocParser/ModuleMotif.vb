''' <summary>
''' 相当于Regulon信息
''' </summary>
Public Class ModuleMotif

    ''' <summary>
    ''' 模块的名称
    ''' </summary>
    ''' <returns></returns>
    Public Property [module] As String
    ''' <summary>
    ''' 产生这个motif的基因的列表
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String()
    ''' <summary>
    ''' 这个模块的描述信息
    ''' </summary>
    ''' <returns></returns>
    Public Property describ As String
    ''' <summary>
    ''' A
    ''' </summary>
    ''' <returns></returns>
    Public Property type As String
    ''' <summary>
    ''' B
    ''' </summary>
    ''' <returns></returns>
    Public Property [class] As String
    ''' <summary>
    ''' C
    ''' </summary>
    ''' <returns></returns>
    Public Property category As String
    Public Property motif As String
    Public Property family As String
    Public Property regulators As String()
    Public Property evalue As Double
    Public Property tom As String
End Class
