
Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Emit.Delegates

    ''' <summary>
    ''' 将属性的<see cref="PropertyInfo.SetValue(Object, Object)"/>编译为方法调用
    ''' </summary>
    Public Class DataValue(Of T)

        ReadOnly type As Type = GetType(T)
        ReadOnly data As T()
        ReadOnly properties As Dictionary(Of String, PropertyInfo)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="name$">The property name, using the ``nameof`` operator to get the property name!</param>
        ''' <returns></returns>
        Public Property Evaluate(Of V)(name$) As V()
            Get
                Dim [property] As New BindProperty(Of DataFrameColumnAttribute)(properties(name))
                Return data.Select(Function(x) [property].__getValue(x)).ToArray
            End Get
            Set(value As V())
                Dim [property] As New BindProperty(Of DataFrameColumnAttribute)(properties(name))

                If value.IsNullorEmpty Then
                    For Each x In data
                        Call [property].__setValue(x, Nothing)
                    Next
                ElseIf value.Length = 1 Then
                    Dim v As Object = value(Scan0)
                    For Each x In data
                        Call [property].__setValue(x, v)
                    Next
                Else
                    For i As Integer = 0 To data.Length - 1
                        Call [property].__setValue(data(i), value(i))
                    Next
                End If
            End Set
        End Property

        'Public Property Evaluate(Of V)(name$) As V()
        '    Get
        '        Dim [property] As New BindProperty(Of DataFrameColumnAttribute)(properties(name))
        '        Return data _
        '            .Select(Function(x) DirectCast([property].__getValue(x), V)) _
        '            .ToArray
        '    End Get
        '    Set(value As V())
        '        Dim [property] As New BindProperty(Of DataFrameColumnAttribute)(properties(name))

        '        If value.IsNullorEmpty Then  ' value array is nothing or have no data, then means set all property value to nothing 
        '            For Each x In data
        '                Call [property].__setValue(x, Nothing)
        '            Next
        '        ElseIf value.Length = 1 Then ' value array only have one element, then means set all property value to a specific value
        '            Dim v As Object = value(Scan0)
        '            For Each x In data
        '                Call [property].__setValue(x, v)
        '            Next
        '        Else
        '            If value.Length <> data.Length Then
        '                Throw New InvalidExpressionException("value array should have the same length as the target data array")
        '            End If

        '            For i As Integer = 0 To data.Length - 1
        '                Call [property].__setValue(data(i), value(i))
        '            Next
        '        End If
        '    End Set
        'End Property

        Sub New(src As IEnumerable(Of T))
            data = src.ToArray
            properties = type.Schema(PropertyAccess.NotSure, PublicProperty, True)
        End Sub

        Public Overrides Function ToString() As String
            Return type.FullName
        End Function
    End Class
End Namespace