Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Text

Namespace ServerApp

    ''' <summary>
    ''' 分析某一个表实体对象的内存占用大小
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class MemorySize(Of T As SQLTable)

        Public ReadOnly Property Type As Type = GetType(T)
        ''' <summary>
        ''' ORM表对象的所有可读的属性集合
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Schema As PropertyInfo()

        Sub New()
            Schema = Type _
                .GetProperties(BindingFlags.Public Or BindingFlags.Instance) _
                .Where(Function(prop) prop.CanRead)
        End Sub

        Shared Sub New()
            Call __sizeOf()
        End Sub

        Shared ReadOnly sizeOf As New Dictionary(Of Type, Integer)
        Shared ReadOnly charSize As Integer = Marshal.SizeOf("$"c)

        Private Shared Sub __sizeOf()
            sizeOf(GetType(Long)) = Marshal.SizeOf(0&)
            sizeOf(GetType(Integer)) = Marshal.SizeOf(0%)
            sizeOf(GetType(Single)) = Marshal.SizeOf(0!)
            sizeOf(GetType(Boolean)) = Marshal.SizeOf(True)
            sizeOf(GetType(Date)) = Marshal.SizeOf(Now)
            sizeOf(GetType(Double)) = Marshal.SizeOf(0#)
            sizeOf(GetType(Decimal)) = Marshal.SizeOf(0@)
            ' sizeOf(GetType(Char)) = Marshal.SizeOf(ASCII.TAB)
            sizeOf(GetType(Short)) = Marshal.SizeOf(CShort(1))
        End Sub

        Public Function MeasureSize(o As T) As Long
            Dim obj As Object = o
            Dim size&

            For Each prop As PropertyInfo In Schema
                If sizeOf.ContainsKey(prop.PropertyType) Then
                    size += sizeOf(prop.PropertyType)
                ElseIf prop.PropertyType.Equals(GetType(String)) Then
                    Dim s$ = DirectCast(prop.GetValue(obj), String)
                    If Not s Is Nothing Then
                        size += s.Length * charSize
                    End If
                Else
                    size += Marshal.SizeOf(prop.GetValue(obj))
                End If
            Next

            Return size
        End Function

        Public Overrides Function ToString() As String
            Return $"sizeof({Type.FullName})"
        End Function
    End Class
End Namespace