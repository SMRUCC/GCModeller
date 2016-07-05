#Region "Microsoft.VisualBasic::eda6721c14fa1ab05bca64a17f212fda, ..\GCModeller\core\Bio.Assembly\ComponentModel\Loci.Models\Loci.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace ComponentModel.Loci

    Public Class Loci : Implements ILocationComponent

        Public Property TagData As String
        Public Property Left As Integer Implements ILocationComponent.Left
        Public Property Right As Integer Implements ILocationComponent.Right

        Public Overrides Function ToString() As String
            Return TagData
        End Function
    End Class
End Namespace
