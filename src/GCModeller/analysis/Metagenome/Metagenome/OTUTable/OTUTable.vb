#Region "Microsoft.VisualBasic::4381785e4798595db3cc74dafafe221f, GCModeller\analysis\Metagenome\Metagenome\OTUTable\OTUTable.vb"

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

    '   Total Lines: 73
    '    Code Lines: 49
    ' Comment Lines: 16
    '   Blank Lines: 8
    '     File Size: 2.73 KB


    ' Class OTUTable
    ' 
    '     Properties: taxonomy
    ' 
    '     Function: FromOTUData, LoadSample
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' 这个对象记录了当前的宏基因组实验之中的每一个OTU在样品之中的含量的多少
''' 
''' 这个对象的数据结构与<see cref="OTUData"/>类似, 二者可以做等价替换
''' </summary>
Public Class OTUTable : Inherits DataSet

    ''' <summary>
    ''' OTU编号所对应的物种分类信息
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Column("taxonomy", GetType(BIOMTaxonomyParser))>
    Public Property taxonomy As Taxonomy

    ''' <summary>
    ''' 这个函数会自动兼容csv或者tsv格式的
    ''' </summary>
    ''' <param name="table$"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadSample(table$, Optional uidMap$ = "OTU_ID") As OTUTable()
        Return DataSet.LoadDataSet(Of OTUTable)(
            path:=table,
            uidMap:=uidMap,
            isTsv:=FileFormat.IsTsvFile(table)
        ).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromOTUData(data As IEnumerable(Of OTUData(Of Double)), Optional brief As Boolean = True) As IEnumerable(Of OTUTable)
        Dim parser As TaxonomyLineageParser = If(brief, BriefParser, CompleteParser)

        Return data _
            .SafeQuery _
            .Select(Function(d)
                        Dim lineage As New Taxonomy(parser(d.taxonomy))

                        Return New OTUTable With {
                            .ID = d.OTU,
                            .Properties = d.data,
                            .taxonomy = lineage
                        }
                    End Function)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Narrowing Operator CType(table As OTUTable) As OTUData(Of Double)
        Return New OTUData(Of Double) With {
            .OTU = table.ID,
            .data = table.Properties,
            .taxonomy = table.taxonomy.ToString(BIOMstyle:=True)
        }
    End Operator

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Widening Operator CType(data As OTUData(Of Double)) As OTUTable
        Return New OTUTable With {
            .ID = data.OTU,
            .Properties = data.data,
            .taxonomy = BIOMTaxonomy _
                .TaxonomyParser(data.taxonomy) _
                .AsTaxonomy
        }
    End Operator
End Class
