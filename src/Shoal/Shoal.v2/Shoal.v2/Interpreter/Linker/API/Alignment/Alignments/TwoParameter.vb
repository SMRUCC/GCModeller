Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens.ParameterName
Imports Microsoft.VisualBasic.Scripting.MetaData.Parameter
Imports System.Reflection

Namespace Interpreter.Linker.APIHandler.Alignment

    Friend Class TwoParameter : Inherits SpecialAlignment(Of ParameterInfo())

        Sub New(FuncDef As ParameterInfo(), ByRef InputParam As Dictionary(Of String, Object))
            Call MyBase.New(FuncDef, InputParam)
        End Sub

        Public Overrides Function OverloadsAlignment() As ParamAlignments
            If InputParam.ContainsKey(ExtensionMethodCaller) Then
                Return __extends()
            Else  '普通的, 直接按照顺序进行比较
                Return OrderAlignment(FuncDef, InputParam)
            End If
        End Function

        Private Function __extends() As ParamAlignments
            Dim score As Integer
            Dim valueInput As Object = InputParam(ExtensionMethodCaller)
            Dim args As New List(Of Object)
            Dim paramType As Type = FuncDef(Scan0).ParameterType

            If Not __alignType(paramType, valueInput, score, valueInput) Then
                Return Nothing
            Else
                Call args.Add(valueInput)
            End If

            If InputParam.ContainsKey(ExtSingle) Then

                Dim paramDef As ParameterInfo = FuncDef(1)
                valueInput = InputParam(ExtSingle)

                If __boolsEquals(paramDef, valueInput) Then
                    score += 100
                    Call args.Add(True)
                Else

                    If Not __alignType(paramDef.ParameterType, valueInput, score, valueInput) Then
                        Return Nothing
                    Else
                        Call args.Add(valueInput)
                    End If
                End If

                Return New ParamAlignments With {.Score = score, .args = args.ToArray}
            Else

                Call InputParam.Remove(ExtensionMethodCaller)

                Dim tmpAlign = New OneParameter(FuncDef(1), InputParam).OverloadsAlignment

                If tmpAlign Is Nothing Then
                    Return Nothing
                End If

                score += tmpAlign.Score
                Return New ParamAlignments With {.Score = score, .args = args.Join(tmpAlign.args).ToArray}
            End If
        End Function
    End Class
End Namespace