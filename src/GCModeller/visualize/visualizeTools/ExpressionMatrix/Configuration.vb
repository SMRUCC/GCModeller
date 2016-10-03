#Region "Microsoft.VisualBasic::336dec48c5902775e9b8ee32ec1e9b04, ..\GCModeller\visualize\visualizeTools\ExpressionMatrix\Configuration.vb"

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

Imports Oracle.Java.IO.Properties.Reflector
Imports System.Text.RegularExpressions

Imports System.Runtime.CompilerServices
Imports System.Drawing

Namespace ExpressionMatrix

    Public Class Configuration : Inherits ConfigCommon

        Public Overrides Function Save(Optional Path As String = "", Optional encoding As System.Text.Encoding = Nothing) As Boolean
            Dim File As String = Me.ToConfigDoc
            Return File.SaveTo(Path, encoding)
        End Function
    End Class
End Namespace
