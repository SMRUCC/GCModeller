#Region "Microsoft.VisualBasic::ec356e9bc0e64e953f2ac7f4c8d02817, ncbi_datasets\Models\V2reportsSequenceDataProvenance.vb"

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

    '   Total Lines: 29
    '    Code Lines: 11 (37.93%)
    ' Comment Lines: 11 (37.93%)
    '    - Xml Docs: 54.55%
    ' 
    '   Blank Lines: 7 (24.14%)
    '     File Size: 895 B


    '     Class V2reportsSequenceDataProvenance
    ' 
    '         Properties: SourceAccession, SourceDatabase
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsSequenceDataProvenance.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceDataProvenance
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceDataProvenance

        ''' <summary>
        ''' source_database 属性
        ''' </summary>
        <Field("source_database")>
        Public Property SourceDatabase As String

        ''' <summary>
        ''' source_accession 属性
        ''' </summary>
        <Field("source_accession")>
        Public Property SourceAccession As String

    End Class

End Namespace

