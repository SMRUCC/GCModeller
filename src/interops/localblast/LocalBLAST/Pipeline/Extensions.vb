Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Pipeline

    <HideModuleName> Public Module Extensions

        ''' <summary>
        ''' 在反序列化之前，使用这个过滤器过滤掉一些无效的hits来节省反序列的处理时间
        ''' </summary>
        ''' <returns></returns>
        Public Function SkipHitNotFound() As NamedValue(Of Func(Of String, Boolean))
            Return New NamedValue(Of Func(Of String, Boolean)) With {
                .Name = "hit_name",
                .Value = Function(colVal)
                             ' 将所有的HITS_NOT_FOUND的行都跳过
                             ' 这样子可以节省比较多的内存
                             Return colVal = "HITS_NOT_FOUND"
                         End Function
            }
        End Function
    End Module
End Namespace