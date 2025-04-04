﻿#Region "Microsoft.VisualBasic::6d36a5826b329c76a3a0c2616cee5e22, foundation\OBO_Foundry\IO\Models\header.vb"

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


    ' Code Statistics:

    '   Total Lines: 27
    '    Code Lines: 20 (74.07%)
    ' Comment Lines: 3 (11.11%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 4 (14.81%)
    '     File Size: 1.21 KB


    '     Class header
    ' 
    '         Properties: [Date], Author, DataVersion, DefaultNamespace, Ontology
    '                     property_value, Remark, SubsetDef, SynonymTypeDef, Tools
    '                     Version
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection

Namespace IO.Models

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class header

        <Field("format-version")> Public Property Version As String
        <Field("data-version")> Public Property DataVersion As String
        <Field("date")> Public Property [Date] As String
        <Field("saved-by")> Public Property Author As String
        <Field("auto-generated-by")> Public Property Tools As String = "GCModeller"
        <Field("subsetdef")> Public Property SubsetDef As Dictionary(Of String, String)
        <Field("synonymtypedef")> Public Property SynonymTypeDef As Dictionary(Of String, String)
        <Field("default-namespace")> Public Property DefaultNamespace As String
        <Field("remark")> Public Property Remark As String()
        <Field("ontology")> Public Property Ontology As String
        <Field("property_value")> Public Property property_value As String()

        Public Overrides Function ToString() As String
            Return $"[ontology] {Ontology}, version={DataVersion}"
        End Function
    End Class
End Namespace
