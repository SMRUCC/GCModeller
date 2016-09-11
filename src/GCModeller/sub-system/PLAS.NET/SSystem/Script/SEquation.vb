#Region "Microsoft.VisualBasic::929bf1c0f73da8294817a1ac0862278a, ..\GCModeller\sub-system\PLAS.NET\SSystem\Script\SEquation.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Mathematical.Types
Imports Microsoft.VisualBasic.Serialization
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
        Public Function GetModel(engine As Mathematical.Expression) As SimpleExpression
            Return ExpressionParser.TryParse(Expression, engine)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
