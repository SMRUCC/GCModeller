#Region "Microsoft.VisualBasic::bf2adcdf4c26d90a3e74cfa247244db0, ncbi_datasets\Models\Ncbiprotddv2VastScore.vb"

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
    '     File Size: 1.29 KB


    '     Class Ncbiprotddv2VastScore
    ' 
    '         Properties: AlignLength, PctIdentity, PValue, Rmsd, VastScore
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2VastScore.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2VastScore
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2VastScore

        ''' <summary>
        ''' vast_score 属性
        ''' </summary>
        <Field("vast_score")>
        Public Property VastScore As Single?

        ''' <summary>
        ''' align_length 属性
        ''' </summary>
        <Field("align_length")>
        Public Property AlignLength As Integer?

        ''' <summary>
        ''' pct_identity 属性
        ''' </summary>
        <Field("pct_identity")>
        Public Property PctIdentity As Single?

        ''' <summary>
        ''' rmsd 属性
        ''' </summary>
        <Field("rmsd")>
        Public Property Rmsd As Single?

        ''' <summary>
        ''' p_value 属性
        ''' </summary>
        <Field("p_value")>
        Public Property PValue As Single?

    End Class

End Namespace

