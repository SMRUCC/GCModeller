#Region "Microsoft.VisualBasic::dd5f422daff63d357f20b8cb4688a710, ncbi_datasets\Models\V2reportsProteinConservedDomain.vb"

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

    '   Total Lines: 71
    '    Code Lines: 25 (35.21%)
    ' Comment Lines: 32 (45.07%)
    '    - Xml Docs: 84.38%
    ' 
    '   Blank Lines: 14 (19.72%)
    '     File Size: 1.89 KB


    '     Class V2reportsProteinConservedDomain
    ' 
    '         Properties: Accession, BitScore, Description, Evalue, Name
    '                     Partial, Specific, Start, Stop
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsProteinConservedDomain.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProteinConservedDomain
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProteinConservedDomain

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' start 属性
        ''' </summary>
        <Field("start")>
        Public Property Start As Integer?

        ''' <summary>
        ''' stop 属性
        ''' </summary>
        <Field("stop")>
        Public Property Stop As Integer?

        ''' <summary>
        ''' specific 属性
        ''' </summary>
        <Field("specific")>
        Public Property Specific As Boolean?

        ''' <summary>
        ''' partial 属性
        ''' </summary>
        <Field("partial")>
        Public Property Partial As Boolean?

        ''' <summary>
        ''' evalue 属性
        ''' </summary>
        <Field("evalue")>
        Public Property Evalue As Single?

        ''' <summary>
        ''' bit_score 属性
        ''' </summary>
        <Field("bit_score")>
        Public Property BitScore As Single?

    End Class

End Namespace

