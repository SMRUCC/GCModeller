Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Linker.APIHandler
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.Linker

    Public Class Linker : Inherits Runtime.SCOM.RuntimeComponent

        Sub New(ScriptEngine As ShoalShell.Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine)
        End Sub

        ''' <summary>
        ''' 得到内存之中的实际引用位置
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAddress(var As LeftAssignedVariable) As Long
            Dim Addr As String = var.RefEntry
            Dim Ref As Object

            If String.Equals(Addr, "$") Then
                Return 0 '系统保留变量总是处在0位置
            End If

            If var.IsPointer Then  '$var的值为所需要写入的变量名 
                Ref = ScriptEngine.MMUDevice.GetValue(Addr)
                Addr = var.GetAddress(Ref)
                Return __addressOf(Addr)
            End If

            If Addr.First = "*"c Then  '内存指针
                Addr = Mid(Addr, 2)
                Return CLng(Val(Addr))
            End If

            If Len(var.RefEntry) <= 2 Then Return __addressOf(Addr)

            '表达式的指针引用形式长度至少要大于2

            If var.RefEntry.First = "{"c AndAlso var.RefEntry.Last = "}"c Then '内部表达式指针引用
                Addr = Mid(var.RefEntry, 2, Len(var.RefEntry) - 2) '得到表达式
                Call ScriptEngine.Exec(Addr)
                Ref = ScriptEngine.TopOfStack
                Addr = var.GetAddress(Ref)
                Return __addressOf(Addr)
            Else
                Return __addressOf(Addr)
            End If
        End Function

        Private Function __addressOf(var As String) As Long
            Return ScriptEngine.MMUDevice.AddressOf("$" & var, True)
        End Function

        Public Function GetValue(var As Parser.Tokens.LeftAssignedVariable) As Object
            Dim Addr As String = GetAddress(var)
            Dim value As Object = ScriptEngine.MMUDevice.GetValue(Addr)
            Return value
        End Function
    End Class
End Namespace