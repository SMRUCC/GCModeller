Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Writer

    ''' <summary>
    ''' BioCyc 平面文件之中的一个属性-值对(slot)，以及附着在这个值上面的注解-值对。
    ''' 
    ''' 注解使用 BioCyc 的 ``^LABEL - value`` 语法，表示该注解归属于**前一个**属性值，
    ''' 例如反应方程式的化学计量数与区室(compartment)：
    ''' 
    ''' ```
    ''' LEFT - CPD-123
    ''' ^COEFFICIENT - 2
    ''' ^COMPARTMENT - CCO-CYTOSOL
    ''' ```
    ''' </summary>
    Public Class AttrSlot

        ''' <summary>
        ''' 属性名称，例如 ``UNIQUE-ID``/``COMMON-NAME``/``LEFT``
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' 属性值。值之中允许出现换行符号，写出的时候会被转换为 BioCyc 的续行格式。
        ''' </summary>
        ''' <returns></returns>
        Public Property Value As String

        ''' <summary>
        ''' 附着于当前属性值上面的注解
        ''' </summary>
        ''' <returns></returns>
        Public Property Annotations As New List(Of NamedValue(Of String))

        Sub New()
        End Sub

        Sub New(name As String, value As String)
            Me.Name = name
            Me.Value = value
        End Sub

        ''' <summary>
        ''' 为当前的属性值添加一个注解，空值将会被忽略
        ''' </summary>
        ''' <param name="label"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function Annotate(label As String, value As String) As AttrSlot
            If Not String.IsNullOrEmpty(label) AndAlso Not String.IsNullOrEmpty(value) Then
                Call Annotations.Add(New NamedValue(Of String)(label, value))
            End If

            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return $"{Name} - {Value}"
        End Function
    End Class

End Namespace
