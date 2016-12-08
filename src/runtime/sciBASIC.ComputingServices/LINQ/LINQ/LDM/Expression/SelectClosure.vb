#Region "Microsoft.VisualBasic::60c68524a035c61f01d7927c4b87b710, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Expression\SelectClosure.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.CodeDom
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.LINQ.Framework.DynamicCode
Imports Microsoft.VisualBasic.LINQ.Framework.DynamicCode.VBC

Namespace LDM.Expression

    Public Class SelectClosure : Inherits Closure

        Friend SelectMethod As System.Reflection.MethodInfo

        Sub New(source As Statements.Tokens.SelectClosure)
            Call MyBase.New(source)


        End Sub

        Protected Overrides Function __parsing() As CodeExpression

        End Function
    End Class
End Namespace
