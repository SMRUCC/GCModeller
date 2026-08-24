#Region "Microsoft.VisualBasic::44110b2ddcad9044d5215d6d8d5c9558, ncbi_datasets\Models\V2SeqReply.vb"

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
    '     File Size: 1.25 KB


    '     Class V2SeqReply
    ' 
    '         Properties: Accession, Defline, MolType, SeqLength, Sequence
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2SeqReply.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2SeqReply
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2SeqReply

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' seq_length 属性
        ''' </summary>
        <Field("seq_length")>
        Public Property SeqLength As String

        ''' <summary>
        ''' mol_type 属性
        ''' </summary>
        <Field("mol_type")>
        Public Property MolType As Object

        ''' <summary>
        ''' defline 属性
        ''' </summary>
        <Field("defline")>
        Public Property Defline As String

        ''' <summary>
        ''' sequence 属性
        ''' </summary>
        <Field("sequence")>
        Public Property Sequence As String

    End Class

End Namespace

