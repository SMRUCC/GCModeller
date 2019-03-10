Imports RDotNet.Internals

''' <summary>
''' An expression object.
''' </summary>
Public Class Expression
	Inherits SymbolicExpression
	''' <summary>
	''' Creates an expression object.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="pointer">The pointer to an expression.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Evaluates the expression in the specified environment.
	''' </summary>
	''' <param name="environment">The environment.</param>
	''' <returns>The evaluation result.</returns>
	Public Function Evaluate(environment As REnvironment) As SymbolicExpression
		If environment Is Nothing Then
			Throw New ArgumentNullException("environment")
		End If
		If Engine IsNot environment.Engine Then
			Throw New ArgumentException(Nothing, "environment")
		End If

		Return New SymbolicExpression(Engine, Me.GetFunction(Of Rf_eval)()(handle, environment.DangerousGetHandle()))
	End Function

	''' <summary>
	''' Evaluates the expression in the specified environment.
	''' </summary>
	''' <param name="environment">The environment.</param>
	''' <param name="result">The evaluation result, or <c>null</c> if the evaluation failed</param>
	''' <returns><c>True</c> if the evaluation succeeded.</returns>
	Public Function TryEvaluate(environment As REnvironment, ByRef result As SymbolicExpression) As Boolean
		If environment Is Nothing Then
			Throw New ArgumentNullException("environment")
		End If
		If Engine IsNot environment.Engine Then
			Throw New ArgumentException(Nothing, "environment")
		End If

		Dim errorOccurred As Boolean
		Dim pointer As IntPtr = Me.GetFunction(Of R_tryEval)()(handle, environment.DangerousGetHandle(), errorOccurred)
		result = If(errorOccurred, Nothing, New SymbolicExpression(Engine, pointer))
		Return Not errorOccurred
	End Function
End Class
