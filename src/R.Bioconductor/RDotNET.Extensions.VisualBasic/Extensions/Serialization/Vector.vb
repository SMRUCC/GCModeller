#Region "Microsoft.VisualBasic::c1c78aabc6557b417172b2e1e7e92918, RDotNET.Extensions.VisualBasic\Extensions\Serialization\Vector.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module VectorExtensions
    ' 
    '     Function: ToStrings, ToStringsGeneric
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports RDotNET.Internals

Public Module VectorExtensions

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
