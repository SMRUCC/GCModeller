
''' <summary>
''' 细胞系统之中的一个实体对象，请注意，这个对象只用来表示一个现实的物理世界之中存在的生物分子对象
''' </summary>
Public Class Entity

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Guid As Long
    ''' <summary>
    ''' The unique-id
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property uid As String
    ''' <summary>
    ''' 数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Quantity As Double
    ''' <summary>
    ''' 在细胞内或者细胞外的区域的编号
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Compartment As String

    Sub New()

    End Sub
End Class
