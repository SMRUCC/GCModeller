Imports System.ComponentModel
Imports System.Reflection

Namespace Runtime.HybridsScripting

    ''' <summary>
    ''' 与ShellScript进行混合编程的脚本语言，必须要在API的命名空间之中实现这个自定义属性
    ''' </summary>
    ''' <remarks>每一次切换脚本语句环境之前先使用!EntryName来进行，之后直接可以只用左移运算符尽心计算求值</remarks>
    ''' 
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)>
    Public Class LanguageEntryPoint : Inherits DescriptionAttribute

        Public ReadOnly Property Language As String

        ''' <summary>
        ''' 设置脚本的名称
        ''' </summary>
        ''' <param name="Language">脚本语言的名称</param>
        Sub New(Language As String, Optional Description As String = "")
            Call MyBase.New(Description)
            Me.Language = Language
        End Sub

        Public Shared ReadOnly Property TypeInfo As System.Type = GetType(LanguageEntryPoint)

    End Class
End Namespace