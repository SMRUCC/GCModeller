#Region "Microsoft.VisualBasic::523dcc3a7b5254b5fe04fe4c602c76ee, ..\GCModeller\foundation\OBO_Foundry\header.vb"

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

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.OBO_Foundry

''' <summary>
''' 
''' </summary>
Public Class header

    <Field("format-version")> Public Property Version As String
    <Field("data-version")> Public Property DataVersion As String
    <Field("date")> Public Property [Date] As String
    <Field("saved-by")> Public Property Author As String
    <Field("auto-generated-by")> Public Property Tools As String = "GCModeller"
    <Field("subsetdef")> Public Property SubsetDef As String()
    <Field("synonymtypedef")> Public Property SynonymTypeDef As String()
    <Field("default-namespace")> Public Property DefaultNamespace As String
    <Field("remark")> Public Property Remark As String
    <Field("ontology")> Public Property Ontology As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
