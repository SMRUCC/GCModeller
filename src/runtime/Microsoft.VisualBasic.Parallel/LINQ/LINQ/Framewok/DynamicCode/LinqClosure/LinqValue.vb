Namespace Framework.DynamicCode

    ''' <summary>
    ''' From x in $source let value as LinqValue = Project(x) Where value.IsTrue Select value.value
    ''' </summary>
    Public Structure LinqValue

        Public Property IsTrue As Boolean
        ''' <summary>
        ''' Linq表达式在Select语句之中所产生的数据投影
        ''' </summary>
        ''' <returns></returns>
        Public Property Projects As Object

        Public Shared Function Unavailable() As LinqValue
            Return New LinqValue With {
                .IsTrue = False,
                .Projects = Nothing
            }
        End Function

        Sub New(obj As Object)
            IsTrue = True
            Projects = obj
        End Sub
    End Structure
End Namespace