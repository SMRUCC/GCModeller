#Region "Microsoft.VisualBasic::54b335103c55bae8d2050e9fb8436a49, ncbi_datasets\Models\V2reportsBiocollection.vb"

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

    '   Total Lines: 53
    '    Code Lines: 19 (35.85%)
    ' Comment Lines: 23 (43.40%)
    '    - Xml Docs: 78.26%
    ' 
    '   Blank Lines: 11 (20.75%)
    '     File Size: 1.44 KB


    '     Class V2reportsBiocollection
    ' 
    '         Properties: BioCollectionId, Code, Comments, Name, NcbiUniqueCode
    '                     Type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsBiocollection.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBiocollection
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsBiocollection

        ''' <summary>
        ''' bio_collection_id 属性
        ''' </summary>
        <Field("bio_collection_id")>
        Public Property BioCollectionId As String

        ''' <summary>
        ''' code 属性
        ''' </summary>
        <Field("code")>
        Public Property Code As String

        ''' <summary>
        ''' ncbi_unique_code 属性
        ''' </summary>
        <Field("ncbi_unique_code")>
        Public Property NcbiUniqueCode As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As String

        ''' <summary>
        ''' comments 属性
        ''' </summary>
        <Field("comments")>
        Public Property Comments As String

    End Class

End Namespace

