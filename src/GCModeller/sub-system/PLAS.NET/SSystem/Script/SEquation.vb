#Region "Microsoft.VisualBasic::476243540e765f0dc4d2f0397ef6a21e, sub-system\PLAS.NET\SSystem\Script\SEquation.vb"

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

    '     Class SEquation
    ' 
    '         Properties: Expression, x
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetModel, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.Types
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Script

    ''' <summary>
    ''' S-system equation.(S-系统方程数据模型)
    ''' </summary>
    Public Class SEquation

        ''' <summary>
        ''' UniqueId，由于可能会存在多个过程，所以这里的值不再唯一
        ''' </summary>
        ''' <returns></returns>
        Public Property x As String
        ''' <summary>
        ''' The mathematics equation of this reaction channel its dynamics
        ''' </summary>
        ''' <returns></returns>
        Public Property Expression As String

        Sub New(id As String, expr As String)
            x = id
            Expression = expr
        End Sub

        Sub New()
        End Sub

        ''' <summary>
        ''' Parsing the math expression property <see cref="Expression"/>
        ''' </summary>
        ''' <param name="engine"></param>
        ''' <returns></returns>
        Public Function GetModel(engine As Expression) As SimpleExpression
            Return ExpressionParser.TryParse(Expression, engine)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
