Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens.ParameterName
Imports Microsoft.VisualBasic.Scripting.MetaData.Parameter
Imports System.Reflection
Imports Microsoft.VisualBasic.Language

Namespace Interpreter.Linker.APIHandler.Alignment

    Friend Class OneParameter : Inherits SpecialAlignment(Of System.Reflection.ParameterInfo)

        Sub New(FuncDef As System.Reflection.ParameterInfo, ByRef InputParam As Dictionary(Of String, Object))
            Call MyBase.New(FuncDef, InputParam)
        End Sub

        Public Overrides Function OverloadsAlignment() As ParamAlignments
            If InputParam.IsNullOrEmpty Then '输入的参数是空的，则判断函数参数是否为可选参数

                If Not FuncDef.IsOptional Then
                    Return Nothing
                Else
                    Return New ParamAlignments With {
                        .Score = 10,
                        .args = {FuncDef.DefaultValue}
                    }
                End If

            End If

            Dim pName As String = GetAliasNameView(FuncDef).ToLower

            If InputParam.ContainsKey(pName) Then
                Return __equals(pName)
            End If

            If InputParam.ContainsKey(FunctionCalls.ExtensionMethodCaller) Then       '拓展方法参数
                Return __equals(FunctionCalls.ExtensionMethodCaller)
            End If

            If InputParam.ContainsKey(FunctionCalls.SingleParameter) Then
                Return __equals(FunctionCalls.SingleParameter)
            End If

            Return Nothing  '都找不到相同的定义，则肯定不可以进行调用
        End Function

        Private Function __equals(Name As String) As ParamAlignments
            Dim equalsValue As i32 = Scan0
            Dim valueInput As Object = InputParam(Name)

            If __boolsEquals(FuncDef, valueInput) Then Return New ParamAlignments With {.Score = 100, .args = {True}}

            Dim inputType As Type = valueInput.GetType

            If (equalsValue = TypeEquals.TypeEquals(FuncDef.ParameterType, inputType)) > 0 Then
                Return New ParamAlignments With {
                    .Score = equalsValue,
                    .args = {valueInput}
                }
            Else

                If InputHandler.Convertible(inputType, FuncDef.ParameterType) Then
                    Return New ParamAlignments With {
                        .Score = 20,
                        .args = {CTypeDynamic(valueInput, FuncDef.ParameterType)}
                    }
                Else
                    Return Nothing
                End If
            End If
        End Function
    End Class
End Namespace