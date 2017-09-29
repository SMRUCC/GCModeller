Imports System.Reflection
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS.Render

Namespace Driver.CSS

    ''' <summary>
    ''' 因为绘图的函数一般有很多的CSS样式参数用来调整图形上面的元素的样式，
    ''' 通过命令行传递这么多的参数不现实，故而在这里通过CSS文件加反射的形式
    ''' 来传递这些绘图参数，并且同时也保留对函数式编程的兼容性
    ''' </summary>
    Public Module RuntimeInvoker

        Public Class var
            Public name$, value

            Public Shared Operator =(var As var, value As Object) As var
                var.value = value
                Return var
            End Operator

            Public Shared Operator <>(var As var, value As Object) As var
                Throw New NotImplementedException
            End Operator
        End Class

        Public Structure ArgumentList

            Default Public ReadOnly Property Argument(name$) As var
                Get
                    Return New var With {.name = name}
                End Get
            End Property
        End Structure

        Public Function RunPlot(driver As [Delegate], CSS As CssBlock, ParamArray args As var()) As GraphicsData
            Dim type As MethodInfo = driver.Method
            Dim parameters = type.GetParameters

        End Function

        Sub test()

            With New ArgumentList
                RunPlot(Nothing, Nothing, !A = 99, !B = 123, !C = "dertfff")
            End With
        End Sub
    End Module
End Namespace