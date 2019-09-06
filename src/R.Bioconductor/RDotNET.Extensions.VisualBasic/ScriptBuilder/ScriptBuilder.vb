#Region "Microsoft.VisualBasic::2525740f9f047bb84cbacf759c0dac5c, RDotNET.Extensions.VisualBasic\ScriptBuilder\ScriptBuilder.vb"

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

    '     Class ScriptBuilder
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Operators: +
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace SymbolBuilder

    ''' <summary>
    ''' The R script builder
    ''' </summary>
    Public Class ScriptBuilder : Inherits Scripting.SymbolBuilder.ScriptBuilder

        Sub New(capacity%)
            Call MyBase.New(capacity)
        End Sub

        Sub New()
            Call MyBase.New(1024)
        End Sub

        ''' <summary>
        ''' AppendLine
        ''' </summary>
        ''' <param name="sb"></param>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Overloads Shared Operator +(sb As ScriptBuilder, s As IRToken) As ScriptBuilder
            Call sb.Script.AppendLine(s.RScript)
            Return sb
        End Operator

        Public Overloads Shared Widening Operator CType(script As String) As ScriptBuilder
            With New ScriptBuilder
                Call .AppendLine(script)
                Return .ByRef
            End With
        End Operator
    End Class
End Namespace
