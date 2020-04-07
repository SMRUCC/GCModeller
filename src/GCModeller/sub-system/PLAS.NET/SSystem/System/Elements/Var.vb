#Region "Microsoft.VisualBasic::b71d24b65fb91823c33db374dfcdde5f, PLAS.NET\SSystem\System\Elements\Var.vb"

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

    '     Class var
    ' 
    '         Properties: comment, title, Value
    ' 
    '         Function: ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports Microsoft.VisualBasic.Math.Framework
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Namespace Kernel.ObjectModels

    Public Class var : Inherits VariableObject
        Implements Ivar

        <XmlAttribute> Public Property title As String
        <XmlAttribute> Public Overrides Property Value As Double Implements Ivar.value

        <XmlElement>
        Public Property comment As String

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(comment) Then
                Return String.Format("{0}={1}", IIf(Len(title) > 0, title, Id), Value)
            Else
                Return String.Format("{0}={1}; //{2}", IIf(Len(title) > 0, title, Id), Value, comment)
            End If
        End Function

        Public Overloads Shared Narrowing Operator CType(e As var) As Double
            Return e.Value
        End Operator

        Public Overloads Shared Narrowing Operator CType(e As var) As String
            Return IIf(Len(e.title) > 0, e.title, e.Id)
        End Operator

        Public Overloads Shared Widening Operator CType(e As Specie) As var
            Return New var With {
                .Id = e.ID,
                .title = e.name,
                .Value = Val(e.InitialAmount)
            }
        End Operator

        ''' <summary>
        ''' 用来解析值是一个表达式的情况
        ''' </summary>
        ''' <param name="strData"></param>
        ''' <param name="val"></param>
        ''' <returns></returns>
        Public Shared Function TryParse(strData As String, val As ExpressionEngine) As var
            Dim tokens = strData.GetTagValue("=", trim:=True)

            Return New var With {
                .Id = tokens.Name,
                .Value = val.Evaluate(tokens.Value)
            }
        End Function

        ''' <summary>
        ''' 请注意，假若值是一个表达式的话，请不要使用这个方法来解析
        ''' </summary>
        ''' <param name="s">Script line.(脚本行文本)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Widening Operator CType(s As String) As var
            Dim Token = s.GetTagValue("=")

            Return New var With {
                .Id = Token.Name,
                .Value = Val(Token.Value)
            }
        End Operator
    End Class
End Namespace
