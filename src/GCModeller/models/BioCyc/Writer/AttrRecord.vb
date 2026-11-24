Imports Microsoft.VisualBasic.Linq

Namespace Writer

    ''' <summary>
    ''' BioCyc 平面文件之中的一条对象记录(frame)。
    ''' 
    ''' 一条记录以 ``UNIQUE-ID`` 开始，由若干属性-值对组成，以单独一行的 ``//`` 结束。
    ''' 本类型仅负责按照添加顺序保存槽位，实际的文本转义与续行处理由 <see cref="AttrFileWriter"/> 完成。
    ''' </summary>
    Public Class AttrRecord

        ''' <summary>
        ''' 当前对象的唯一标识符，对应 ``UNIQUE-ID`` 槽位
        ''' </summary>
        ''' <returns></returns>
        Public Property uniqueId As String

        ''' <summary>
        ''' 按照添加顺序排列的槽位集合
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property slots As New List(Of AttrSlot)

        Sub New()
        End Sub

        Sub New(uniqueId As String)
            Me.uniqueId = uniqueId
        End Sub

        ''' <summary>
        ''' 添加一个属性-值对，返回新添加的槽位以便继续添加注解
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function Add(name As String, value As String) As AttrSlot
            Dim slot As New AttrSlot(name, value)

            Call slots.Add(slot)

            Return slot
        End Function

        ''' <summary>
        ''' 添加一组同名的属性-值对，空值会被忽略
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="values"></param>
        ''' <returns></returns>
        Public Function Add(name As String, values As IEnumerable(Of String)) As AttrRecord
            For Each value As String In values.SafeQuery
                If Not String.IsNullOrEmpty(value) Then
                    Call Me.Add(name, value)
                End If
            Next

            Return Me
        End Function

        ''' <summary>
        ''' 将注解添加到最近添加的那个槽位上面
        ''' </summary>
        ''' <param name="label"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function Annotate(label As String, value As String) As AttrRecord
            If slots.Count = 0 Then
                Return Me
            End If

            Call slots.Last.Annotate(label, value)

            Return Me
        End Function

        ''' <summary>
        ''' 当前记录是否除了 ``UNIQUE-ID`` 之外没有任何槽位
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return String.IsNullOrEmpty(uniqueId) OrElse slots.Count = 0
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return If(uniqueId, MyBase.ToString)
        End Function
    End Class

End Namespace
