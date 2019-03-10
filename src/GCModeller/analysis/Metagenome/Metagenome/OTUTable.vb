﻿#Region "Microsoft.VisualBasic::6ea4610aee405e9e0a37470db678a19a, analysis\Metagenome\Metagenome\OTUTable.vb"

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

    ' Class OTUTable
    ' 
    '     Properties: ID, Taxonomy
    ' 
    '     Function: FromOTUData, LoadSample
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' 这个对象记录了当前的宏基因组实验之中的每一个OTU在样品之中的含量的多少
''' </summary>
Public Class OTUTable : Inherits DataSet

    <Column("OTU_ID")>
    Public Overrides Property ID As String
    Public Property Taxonomy As Taxonomy

    ''' <summary>
    ''' 这个函数会自动兼容csv或者tsv格式的
    ''' </summary>
    ''' <param name="table$"></param>
    ''' <returns></returns>
    Public Shared Function LoadSample(table$, Optional ID$ = "OTU_ID") As OTUTable()
        If table.ReadFirstLine.Count(ASCII.TAB) > 1 Then
            ' tsv文件
            Return table _
                .ReadAllLines _
                .ImportsTsv(Of OTUTable)({
                    {ID, NameOf(OTUTable.ID)}
                })
        Else
            Return table.LoadCsv(Of OTUTable)(maps:={
                {ID, NameOf(OTUTable.ID)}
            })
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromOTUData(data As IEnumerable(Of OTUData), Optional brief As Boolean = True) As IEnumerable(Of OTUTable)
        Dim parser As TaxonomyLineageParser = If(brief, BriefParser, CompleteParser)

        Return data _
            .SafeQuery _
            .Select(Function(d)
                        Dim lineage As New Taxonomy(parser(d.Taxonomy))

                        Return New OTUTable With {
                            .ID = d.OTU,
                            .Properties = d.Data.AsNumeric,
                            .Taxonomy = lineage
                        }
                    End Function)
    End Function
End Class
