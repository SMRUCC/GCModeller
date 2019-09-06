#Region "Microsoft.VisualBasic::448fd192c04b9fd6ef934d325f33a01a, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\utils\combn.vb"

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

    '     Class combn
    ' 
    '         Properties: FUN, m, simplify, x
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.utils

    ''' <summary>
    ''' Generate all combinations of the elements of x taken m at a time. If x is a positive integer, returns all combinations of the elements of seq(x) taken m at a time. 
    ''' If argument FUN is not NULL, applies a function given by the argument to each point. If simplify is FALSE, returns a list; 
    ''' otherwise returns an array, typically a matrix. ... are passed unchanged to the FUN function, if specified.
    ''' 
    ''' Factors x are accepted from R 3.1.0 (although coincidentally they worked for simplify = FALSE in earlier versions).
    ''' </summary>
    <RFunc("combn")> Public Class combn : Inherits IRToken

        ''' <summary>
        ''' vector source for combinations, or integer n for x &lt;- seq_len(n).
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' number of elements to choose.
        ''' </summary>
        ''' <returns></returns>
        Public Property m As Integer
        ''' <summary>
        ''' function to be applied to each combination; default NULL means the identity, i.e., to return the combination (vector of length m).
        ''' </summary>
        ''' <returns></returns>
        Public Property FUN As RExpression = NULL
        ''' <summary>
        ''' logical indicating if the result should be simplified to an array (typically a matrix); if FALSE, the function returns a list. Note that when simplify = TRUE as by default, the dimension of the result is simply determined from FUN(1st combination) (for efficiency reasons). This will badly fail if FUN(u) is not of constant length.
        ''' </summary>
        ''' <returns></returns>
        Public Property simplify As Boolean = True
    End Class
End Namespace
