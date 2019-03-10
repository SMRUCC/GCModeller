Imports System.Reflection

Namespace Runtime.HybridsScripting

    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=True, Inherited:=True)>
    Public Class DataTransform : Inherits EntryInterface

        Public ReadOnly Property TypeChar As Char
        Public ReadOnly Property ReservedStringTLTR As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="type">
        ''' 数据类型的后缀，推荐：
        ''' $ <see cref="String"></see>; 
        ''' &amp; <see cref="Long"></see>; 
        ''' % <see cref="Integer"></see>; 
        ''' ? <see cref="Boolean"></see>; 
        ''' ! <see cref="Double"></see>; 
        ''' @ <see cref="Date"></see>
        ''' </param>
        ''' <param name="ReservedString">在脚本环境之中必须要有一个保留的字符串转换方法</param>
        ''' <remarks></remarks>
        Sub New(type As Char, Optional ReservedString As Boolean = False)
            Call MyBase.New(InterfaceTypes.DataTransform)
            _TypeChar = type
            _ReservedStringTLTR = ReservedString
        End Sub

        Public Overrides Function ToString() As String
            Return _TypeChar.ToString
        End Function
    End Class
End Namespace