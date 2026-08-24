#Region "Microsoft.VisualBasic::76cf35d4143c86eb907536ebc74a631a, ncbi_datasets\Models\V2reportsSeqRangeSetFasta.vb"

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

    '   Total Lines: 47
    '    Code Lines: 17 (36.17%)
    ' Comment Lines: 20 (42.55%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 10 (21.28%)
    '     File Size: 1.31 KB


    '     Class V2reportsSeqRangeSetFasta
    ' 
    '         Properties: AccessionVersion, Range, SeqId, SequenceHash, Title
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsSeqRangeSetFasta.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSeqRangeSetFasta
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSeqRangeSetFasta

        ''' <summary>
        ''' seq_id 属性
        ''' </summary>
        <Field("seq_id")>
        Public Property SeqId As String

        ''' <summary>
        ''' accession_version 属性
        ''' </summary>
        <Field("accession_version")>
        Public Property AccessionVersion As String

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <Field("title")>
        Public Property Title As String

        ''' <summary>
        ''' sequence_hash 属性
        ''' </summary>
        <Field("sequence_hash")>
        Public Property SequenceHash As String

        ''' <summary>
        ''' range 属性
        ''' </summary>
        <Field("range")>
        Public Property Range As List(Of Object)

    End Class

End Namespace

