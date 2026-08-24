#Region "Microsoft.VisualBasic::5d6518b5972046c6ae5f3fe85da398ff, ncbi_datasets\Models\V2reportsPairedAssembly.vb"

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
    '     File Size: 1.99 KB


    '     Class V2reportsPairedAssembly
    ' 
    '         Properties: Accession, AnnotationName, Changed, Differences, ManualDiff
    '                     OnlyGenbank, OnlyRefseq, RefseqGenbankAreDifferent, Status
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsPairedAssembly.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsPairedAssembly
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsPairedAssembly

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' status 属性
        ''' </summary>
        <Field("status")>
        Public Property Status As Object

        ''' <summary>
        ''' annotation_name 属性
        ''' </summary>
        <Field("annotation_name")>
        Public Property AnnotationName As String

        ''' <summary>
        ''' only_genbank 属性
        ''' </summary>
        <Field("only_genbank")>
        Public Property OnlyGenbank As String

        ''' <summary>
        ''' only_refseq 属性
        ''' </summary>
        <Field("only_refseq")>
        Public Property OnlyRefseq As String

        ''' <summary>
        ''' changed 属性
        ''' </summary>
        <Field("changed")>
        Public Property Changed As String

        ''' <summary>
        ''' manual_diff 属性
        ''' </summary>
        <Field("manual_diff")>
        Public Property ManualDiff As String

        ''' <summary>
        ''' refseq_genbank_are_different 属性
        ''' </summary>
        <Field("refseq_genbank_are_different")>
        Public Property RefseqGenbankAreDifferent As Boolean?

        ''' <summary>
        ''' differences 属性
        ''' </summary>
        <Field("differences")>
        Public Property Differences As String

    End Class

End Namespace

