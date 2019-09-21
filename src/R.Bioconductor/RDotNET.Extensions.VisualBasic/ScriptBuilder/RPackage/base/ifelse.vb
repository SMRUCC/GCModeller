#Region "Microsoft.VisualBasic::9a43bb5d36f690092e95c44d0b8051a7, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\base\ifelse.vb"

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

    '     Class ifelse
    ' 
    '         Properties: no, test, yes
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.base

    <RFunc("ifelse")> Public Class ifelse : Inherits IRToken
        Public Property test As RExpression
        Public Property yes As RExpression
        Public Property no As RExpression

        Sub New(test As String, yes As String, no As String)
            Me.yes = yes
            Me.no = no
            Me.test = test
        End Sub
    End Class
End Namespace
