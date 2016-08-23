#Region "Microsoft.VisualBasic::04b5ae297d8a19d0b6e4d21ad78ba7f8, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\Services\RPackage\base\ifelse.vb"

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

Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace base

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
