''' <summary>
''' 具体的细胞实例对象的快照数据
''' </summary>
Public Class CellSnapshot

    ''' <summary>
    ''' the cell unique id
    ''' </summary>
    ''' <returns></returns>
    Public Property cell_id As String
    ''' <summary>
    ''' parent cell its <see cref="cell_id"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property parent_id As String

    ''' <summary>
    ''' organism taxonomy info of current cell genome
    ''' </summary>
    ''' <returns></returns>
    Public Property taxonomy As String
    Public Property rna As Dictionary(Of String, Double)
    Public Property protein As Dictionary(Of String, Double)
    Public Property metabolite As Dictionary(Of String, Double)
    Public Property is_alive As Boolean

End Class
