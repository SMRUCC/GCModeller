#Region "Microsoft.VisualBasic::5f22b37e93fe17d11760ec68f1d63ac4, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\base\Control.vb"

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

    '     Module ControlFlow
    ' 
    '         Function: (+3 Overloads) [if], ifelse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace SymbolBuilder.packages.base

    ''' <summary>
    ''' These are the basic control-flow constructs of the R language. They function in much the same way as control statements in any Algol-like language. 
    ''' They are all reserved words.
    ''' </summary>
    Public Module ControlFlow

        ''' <summary>
        ''' A length-one logical vector that is not NA. Conditions of length greater than one are accepted with a warning, but only the first element is used. Other types are coerced to logical if possible, ignoring any class.
        ''' </summary>
        ''' <param name="cond"></param>
        ''' <param name="action"></param>
        ''' <returns></returns>
        Public Function [if](cond As String, action As String) As String
            Return $"if({cond}) {action}"
        End Function

        ''' <summary>
        ''' A length-one logical vector that is not NA. Conditions of length greater than one are accepted with a warning, but only the first element is used. Other types are coerced to logical if possible, ignoring any class.
        ''' </summary>
        ''' <param name="cond"></param>
        ''' <param name="action"></param>
        ''' <returns></returns>
        Public Function [if](cond As String, action As Func(Of String)) As String
            Dim sb As New ScriptBuilder
            sb += $"if({cond}) " & "{"
            sb += action()
            sb += "}"

            Return sb.ToString
        End Function

        ''' <summary>
        ''' These are the basic control-flow constructs of the R language. They function in much the same way as control statements in any Algol-like language. They are all reserved words.
        ''' </summary>
        ''' <param name="cond">
        ''' A length-one logical vector that is not NA. Conditions of length greater than one are accepted with a warning, but only the first element is used. Other types are coerced to logical if possible, ignoring any class.
        ''' </param>
        ''' <param name="[true]">An expression in a formal sense. This is either a simple expression or a so called compound expression, usually of the form { expr1 ; expr2 }.</param>
        ''' <param name="[else]">An expression in a formal sense. This is either a simple expression or a so called compound expression, usually of the form { expr1 ; expr2 }.</param>
        ''' <returns></returns>
        Public Function [if](cond As String, [true] As Func(Of String), [else] As Func(Of String)) As String
            Dim sb As New ScriptBuilder
            sb += $"if({cond}) " & "{"
            sb += [true]()
            sb += "} else {"
            sb += [else]()
            sb += "}"

            Return sb.ToString
        End Function

        ''' <summary>
        ''' ifelse returns a value with the same shape as test which is filled with elements selected from either yes or no depending on whether the element of test is TRUE or FALSE.
        ''' 
        ''' If yes or no are too short, their elements are recycled. yes will be evaluated if and only if any element of test is true, and analogously for no.
        ''' Missing values In test give missing values In the result.
        ''' </summary>
        ''' <param name="test">an object which can be coerced to logical mode.</param>
        ''' <param name="yes">Return values For True elements Of test.</param>
        ''' <param name="no">return values for false elements of test.</param>
        ''' <returns>
        ''' A vector of the same length and attributes (including dimensions and "class") as test and data values from the values of yes or no. 
        ''' The mode of the answer will be coerced from logical to accommodate first any values taken from yes and then any values taken from no.
        ''' </returns>
        Public Function ifelse(test As String, yes As String, no As String) As String
            Return New ifelse(test, yes, no)
        End Function
    End Module
End Namespace
