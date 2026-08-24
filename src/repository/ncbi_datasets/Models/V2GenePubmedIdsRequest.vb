#Region "Microsoft.VisualBasic::46e2af33b23cb34a7f2577a1b3585441, ncbi_datasets\Models\V2GenePubmedIdsRequest.vb"

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

    '   Total Lines: 23
    '    Code Lines: 9 (39.13%)
    ' Comment Lines: 8 (34.78%)
    '    - Xml Docs: 37.50%
    ' 
    '   Blank Lines: 6 (26.09%)
    '     File Size: 675 B


    '     Class V2GenePubmedIdsRequest
    ' 
    '         Properties: GeneIds
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2GenePubmedIdsRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GenePubmedIdsRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GenePubmedIdsRequest

        ''' <summary>
        ''' gene_ids 属性
        ''' </summary>
        <Field("gene_ids")>
        Public Property GeneIds As Integer?

    End Class

End Namespace

