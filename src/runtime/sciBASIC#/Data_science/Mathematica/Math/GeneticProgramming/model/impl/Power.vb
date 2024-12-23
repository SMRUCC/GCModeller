﻿#Region "Microsoft.VisualBasic::2e6899e3333cea2fe04f051d422f2812, Data_science\Mathematica\Math\GeneticProgramming\model\impl\Power.vb"

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


    ' Code Statistics:

    '   Total Lines: 25
    '    Code Lines: 17 (68.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (32.00%)
    '     File Size: 770 B


    '     Class Power
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: eval, ToString, toStringExpression
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports std = System.Math

Namespace model.impl

    Public Class Power : Inherits AbstractBinaryExpression

        Public Sub New(leftChild As Expression, rightChild As Expression)
            MyBase.New(leftChild, rightChild)
        End Sub

        Public Overrides Function eval(x As Double) As Double
            Return std.Pow(leftChildField.eval(x), rightChildField.eval(x))
        End Function

        Public Overrides Function toStringExpression() As String
            Return String.Format("({0} ^ {1})", leftChildField.toStringExpression(), rightChildField.toStringExpression())
        End Function

        Public Overrides Function ToString() As String
            Return "L^R"
        End Function

    End Class

End Namespace
