Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' 复合物对象的接口
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IComplexes
        ''' <summary>
        ''' The components module of this regulator entity.(构成本复合物对象的组件模块的UniqueId列表)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Components As List(Of String)
    End Interface
End Namespace