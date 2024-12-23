﻿#Region "Microsoft.VisualBasic::968f018a005a33a3de02eb9322bfcc0f, core\Bio.Assembly\ComponentModel\Annotation\Profiles\CatalogProfiling.vb"

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

    '   Total Lines: 79
    '    Code Lines: 53 (67.09%)
    ' Comment Lines: 10 (12.66%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 16 (20.25%)
    '     File Size: 2.59 KB


    '     Class CatalogProfiling
    ' 
    '         Properties: Catalog, Description, SubCategory
    ' 
    '         Function: FindCategory, GetCategory, ToString
    ' 
    '     Class ClassProfiles
    ' 
    '         Properties: Catalogs
    ' 
    '         Function: FindClass, GetClass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' a level 1 class dataset
    ''' </summary>
    Public Class CatalogProfiling : Implements INamedValue

        ''' <summary>
        ''' COG/KO/GO, etc
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Overridable Property Catalog As String Implements INamedValue.Key
        Public Property Description As String
        Public Property SubCategory As Dictionary(Of String, CatalogList)

        Public Function FindCategory(id As String) As String
            For Each tag As String In SubCategory.Keys
                If SubCategory(tag).IndexOf(id) > -1 Then
                    Return tag
                End If
            Next

            Return ""
        End Function

        Public Function GetCategory(categoryId As String) As CatalogList
            If Not SubCategory.ContainsKey(categoryId) Then
                Call SubCategory.Add(categoryId, New CatalogList With {
                    .Catalog = categoryId,
                    .Description = "",
                    .IDs = {}
                })
            End If

            Return _SubCategory(categoryId)
        End Function

        Public Overrides Function ToString() As String
            Return $"{Catalog} contains {SubCategory.Count} subcategories... {Mid(SubCategory.Keys.GetJson, 1, 20)}..."
        End Function

    End Class

    ''' <summary>
    ''' ID class counter 
    ''' </summary>
    Public Class ClassProfiles

        Public Property Catalogs As New Dictionary(Of String, CatalogProfiling)

        Public Function FindClass(id As String) As String
            For Each tag As String In Catalogs.Keys
                If Not Catalogs(tag).FindCategory(id).StringEmpty Then
                    Return tag
                End If
            Next

            Return ""
        End Function

        Public Function GetClass(classId As String) As CatalogProfiling
            If Not Catalogs.ContainsKey(classId) Then
                Call Catalogs.Add(classId, New CatalogProfiling With {
                    .Catalog = classId,
                    .Description = "",
                    .SubCategory = New Dictionary(Of String, CatalogList)
                })
            End If

            Return _Catalogs(classId)
        End Function

    End Class
End Namespace
