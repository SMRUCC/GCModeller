﻿#Region "Microsoft.VisualBasic::90a088cc3b7f021ae20ef805b57fc4fe, mime\text%html\Markups\Markup.vb"

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

    ' Class Markup
    ' 
    '     Properties: nodes
    ' 
    '     Function: GetEnumerator, IEnumerable_GetEnumerator
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.Markup
Imports Microsoft.VisualBasic.Serialization

''' <summary>
''' The markup document(*.html, *.md) its document syntax structure object. 
''' </summary>
Public Class Markup : Inherits BaseClass
    Implements IEnumerable(Of PlantText)

    Public Property nodes As List(Of PlantText)

    Public Iterator Function GetEnumerator() As IEnumerator(Of PlantText) Implements IEnumerable(Of PlantText).GetEnumerator
        If nodes.IsNullOrEmpty Then
            Return
        End If

        For Each node As PlantText In nodes
            Yield node
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class
