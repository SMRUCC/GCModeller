Imports System.Collections.ObjectModel
Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens
Imports Microsoft.VisualBasic.SecurityString.MD5Hash

Namespace Interpreter.Linker.APIHandler

    Public Class Arguments : Inherits Runtime.SCOM.RuntimeComponent

        Sub New(ScriptEngine As ShoalShell.Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine)
        End Sub

        Public Function GetParameters(LDM As LDM.Expressions.FunctionCalls) As KeyValuePair(Of String, Object)()
            Dim args As New List(Of KeyValuePair(Of String, InternalExpression))

            If Not LDM.ExtensionVariable Is Nothing Then
                Dim extValue = New KeyValuePair(Of String, InternalExpression)(
                    APIHandler.Alignment.FunctionCalls.ExtensionMethodCaller,
                    LDM.ExtensionVariable)
                Call args.Add(extValue)
            End If

            If Not LDM.Parameters.IsNullOrEmpty Then
                Call args.AddRange((From arg As KeyValuePair(Of ParameterName, InternalExpression)
                                        In LDM.Parameters
                                    Let Name As String = __getName(arg.Key)
                                    Select New KeyValuePair(Of String, InternalExpression)(Name, arg.Value)).ToArray)
            End If

            Dim argsValue = (From arg In args Select New KeyValuePair(Of String, Object)(arg.Key, ScriptEngine.ExecuteModel.Exec(arg.Value.Expression))).ToArray
            Return argsValue
        End Function

        Private Function __getName(ref As Parser.Tokens.ParameterName) As String
            Select Case ref.Type
                Case ParameterName.ParameterType.ExtensionMethodCaller : Return APIHandler.Alignment.FunctionCalls.ExtensionMethodCaller
                Case ParameterName.ParameterType.EXtensionSingleParameter : Return APIHandler.Alignment.FunctionCalls.ExtSingle
                Case ParameterName.ParameterType.OrderReference : Return APIHandler.Alignment.OrderReference
                Case ParameterName.ParameterType.SingleParameter : Return APIHandler.Alignment.SingleParameter
                Case Else
                    Return __calculateRefName(ref)
            End Select
        End Function

        Private Function __calculateRefName(ref As Parser.Tokens.ParameterName) As String
            If ref.Expression.ExprTypeID = LDM.Expressions.ExpressionTypes.DynamicsExpression Then
                Return ref.Expression.PrimaryExpression
            Else
                Return CStr(ScriptEngine.ExecuteModel.Exec(ref.Expression))
            End If
        End Function
    End Class
End Namespace