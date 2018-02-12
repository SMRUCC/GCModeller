#Region "Microsoft.VisualBasic::1e31073336e21442948e94460740bd52, RDotNET.Extensions.VisualBasic\ScriptBuilder\ParameterList.vb"

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

    '     Class ParameterList
    ' 
    '         Properties: parameters
    ' 
    '         Function: ToString
    ' 
    '         Sub: (+2 Overloads) New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace SymbolBuilder

    ''' <summary>
    ''' <see cref="ValueTypes.List"/>
    ''' </summary>
    Public Class ParameterList

        Public Property parameters As String()

        Sub New(list$())
            parameters = list
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return parameters.JoinBy(", ")
        End Function
    End Class
End Namespace
