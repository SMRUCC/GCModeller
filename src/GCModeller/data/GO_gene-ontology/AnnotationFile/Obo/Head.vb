#Region "Microsoft.VisualBasic::7c6e96b787ee61a9a19a2a004dd3a89c, ..\GCModeller\data\GO_gene-ontology\AnnotationFile\Obo\Head.vb"

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

Public Class Head

    <Field("format-version")> Public Property Version As String
    <Field("data-version")> Public Property DataVersion As String
    <Field("date")> Public Property [Date] As String
    <Field("saved-by")> Public Property Author As String
    <Field("auto-generated-by")> Public Property Tools As String
    <Field("subsetdef")> Public Property SubsetDef As String()
    <Field("synonymtypedef")> Public Property SynonymTypeDef As String
    <Field("default-namespace")> Public Property DefaultNamespace As String
    <Field("remark")> Public Property Remark As String
    <Field("ontology")> Public Property Ontology As String
End Class
