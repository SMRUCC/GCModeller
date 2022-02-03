#Region "Microsoft.VisualBasic::38bd72bf139ce2ae31370d7b677d92fe, engine\Model\Cellular\Process\Kinetics.vb"

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

'     Structure Kinetics
' 
'         Function: CompileLambda, ExpressionModel, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Linq.Expressions
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.Math.Scripting

Namespace Cellular.Process

    Public Structure Kinetics
        ''' <summary>
        ''' body expression of the lambda expression
        ''' </summary>
        Dim formula As Impl.Expression
        ''' <summary>
        ''' parameter names for create lambda expression
        ''' </summary>
        Dim parameters As String()
        ''' <summary>
        ''' parameter value list data of the lambda <see cref="parameters"/>. 
        ''' numeric value means constant value and string value means
        ''' id reference of the substrate
        ''' </summary>
        Dim paramVals As Object()
        ''' <summary>
        ''' enzyme target
        ''' </summary>
        Dim enzyme As String
        ''' <summary>
        ''' target reaction id
        ''' </summary>
        Dim target As String
        Dim temperature As Double
        Dim PH As Double

        Public Overrides Function ToString() As String
            Return $"[{target}] {formula}"
        End Function

        Public Shared Function ExpressionModel(formula As String) As Impl.Expression
            Return ScriptEngine.ParseExpression(formula)
        End Function

        Public Function CompileLambda() As DynamicInvoke
            Dim lambda As LambdaExpression = ExpressionCompiler.CreateLambda(parameters, formula)
            Dim handler As [Delegate] = lambda.Compile
            Dim vm = Me

            Return Function(getVal As Func(Of String, Double)) As Double
                       Dim vals = vm.paramVals.ToArray

                       If vm.parameters.Length = 0 Then
                           vals = {}
                       Else
                           For i As Integer = 0 To vals.Length - 1
                               If TypeOf vals(i) Is String Then
                                   vals(i) = getVal(vals(i))
                               End If
                           Next
                       End If

                       Return handler.DynamicInvoke(vals)
                   End Function
        End Function
    End Structure
End Namespace
