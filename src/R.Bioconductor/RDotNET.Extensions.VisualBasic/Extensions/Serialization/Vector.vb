Imports System.Runtime.CompilerServices
Imports RDotNET.Internals

Public Module Vector

    ''' <summary>
    ''' 目标必须是一个character向量类型，否则会出现空值错误的
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ToStrings(s As SymbolicExpression) As String()
        Return s.AsCharacter.ToStringArray
    End Function

    ''' <summary>
    ''' 可以是任意类型的向量
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ToStringsGeneric(s As SymbolicExpression) As String()
        s = s.AsVector

        Select Case s.Type
            Case SymbolicExpressionType.CharacterVector
                Return s.ToStrings
            Case SymbolicExpressionType.IntegerVector
                Return s.AsInteger.ToArray.Select(Function(n) CStr(n)).ToArray
            Case SymbolicExpressionType.LogicalVector
                Return s.AsLogical.ToArray.Select(Function(b) CStr(b).ToUpper).ToArray ' R 里面的逻辑值都是大写的单词
            Case SymbolicExpressionType.NumericVector
                Return s.AsNumeric.ToArray.Select(Function(d) CStr(d)).ToArray
            Case Else
                Return {}  ' 无法转换的都返回空数组
        End Select
    End Function
End Module
