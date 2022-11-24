#Region "Microsoft.VisualBasic::d7ab0913fc37dcbf48c69b0ba3a04ba0, GCModeller\core\Bio.Assembly\ComponentModel\Annotation\Profiles\CatalogProfiling.vb"

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

    '   Total Lines: 25
    '    Code Lines: 16
    ' Comment Lines: 4
    '   Blank Lines: 5
    '     File Size: 842 B


    '     Class CatalogProfiling
    ' 
    '         Properties: Catalog, Description, SubCategory
    ' 
    '         Function: ToString
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

    End Class
End Namespace
