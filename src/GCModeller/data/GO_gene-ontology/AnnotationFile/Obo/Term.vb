#Region "Microsoft.VisualBasic::6d0efad63da5d4e0376c153667e664d0, ..\GO_gene-ontology\AnnotationFile\Obo\Term.vb"

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


''' <summary>
''' 功能定义
''' </summary>
''' <remarks></remarks>
Public Class Term

    <Field("id")> Public Property id As String
    <Field("name")> Public Property name As String
    <Field("namespace")> Public Property [namespace] As String
    <Field("def")> Public Property def As String
    <Field("synonym")> Public Property synonym As String()
    <Field("xref")> Public Property xref As String()
    <Field("is_a")> Public Property is_a As String
    <Field("subset")> Public Property subset As String
    <Field("relationship")> Public Property relationship As String()

    Public Const TERM As String = "[Term]"

    Public Overrides Function ToString() As String
        Return String.Format("[{0}] {1}: {2}", [namespace], id, name)
    End Function
End Class

