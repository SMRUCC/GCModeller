﻿#Region "Microsoft.VisualBasic::94fffb6118695820a03f44339dc9c9a0, Data_science\Mathematica\Math\Math\Scripting\Expression\MathToken.vb"

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

    '   Total Lines: 34
    '    Code Lines: 25 (73.53%)
    ' Comment Lines: 4 (11.76%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (14.71%)
    '     File Size: 822 B


    '     Class MathToken
    ' 
    '         Properties: ZERO
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Enum MathTokens
    ' 
    '         [Operator], Close, Comma, Invalid, Literal
    '         LogicalLiteral, Open, Symbol, Terminator, UnaryNot
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Scripting.MathExpression

    Public Class MathToken : Inherits CodeToken(Of MathTokens)

        ''' <summary>
        ''' get a new literal zero token
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property ZERO As MathToken
            Get
                Return New MathToken(MathTokens.Literal, "0")
            End Get
        End Property

        Sub New(name As MathTokens, text As String)
            Call MyBase.New(name, text)
        End Sub
    End Class

    Public Enum MathTokens
        Invalid
        Literal
        LogicalLiteral
        [Operator]
        UnaryNot
        Open
        Close
        Symbol
        Comma
        Terminator
    End Enum
End Namespace
