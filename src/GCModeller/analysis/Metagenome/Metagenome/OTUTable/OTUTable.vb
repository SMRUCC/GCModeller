#Region "Microsoft.VisualBasic::c91603011db729f1a5f7c125127e69fa, analysis\Metagenome\Metagenome\OTUTable\OTUTable.vb"

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

    '   Total Lines: 111
    '    Code Lines: 67 (60.36%)
    ' Comment Lines: 34 (30.63%)
    '    - Xml Docs: 76.47%
    ' 
    '   Blank Lines: 10 (9.01%)
    '     File Size: 4.49 KB


    ' Class OTUTable
    ' 
    '     Properties: taxonomy
    ' 
    '     Function: FromOTUData, LoadSample, SumDuplicatedOTU
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' ### OTU table (sequence count table)
''' 
''' A OTU table contains the number of sequences that are observed for each taxonomic 
''' unit (OTUs) in each samples. Columns usually represent samples and rows represent
''' genera or species specific taxonomic units (OTUs). OTU tables are often saved as
''' BIOM formatted files.
''' 
''' ### Limited taxonomic resolution
''' 
''' OTU resolution depends On the 16S approach which has some limits In distinguishing at 
''' the species level, For example,
''' 
''' Escherichia coli And Shigella spp. share almost identical 16S rRNA gene sequences.
''' 
''' Alternative approaches are developed To achieve higher resolution up To strain level 
''' by considering larger Or complete sets Of genes.
''' </summary>
''' <remarks>
''' 这个对象记录了当前的宏基因组实验之中的每一个OTU在样品之中的含量的多少
''' 
''' 这个对象的数据结构与<see cref="OTUData(Of Double)"/>类似, 二者可以做等价替换
''' </remarks>
Public Class OTUTable : Inherits DataSet
    Implements IGeneExpression

    ''' <summary>
    ''' OTU编号所对应的物种分类信息
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <Column("taxonomy", GetType(BIOMTaxonomyParser))>
    Public Property taxonomy As Taxonomy

    Public Overrides Property Properties As Dictionary(Of String, Double) Implements IGeneExpression.Expression
        Get
            Return MyBase.Properties
        End Get
        Set(value As Dictionary(Of String, Double))
            MyBase.Properties = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return $"{ID} - {taxonomy} [{Properties.Keys.JoinBy(", ")}]"
    End Function

    Public Shared Iterator Function SumDuplicatedOTU(otus As IEnumerable(Of OTUTable)) As IEnumerable(Of OTUTable)
        For Each otu As IGrouping(Of String, OTUTable) In otus.GroupBy(Function(o) o.taxonomy.ToString)
            Dim allSampleName As String() = otu.PropertyNames
            Dim taxonomy = otu.First.taxonomy
            Dim v As Dictionary(Of String, Double) = allSampleName _
                .ToDictionary(Function(name) name,
                              Function(name)
                                  Return Aggregate m As OTUTable
                                         In otu
                                         Into Sum(m(name))
                              End Function)

            Yield New OTUTable With {
                .ID = otu.Select(Function(m) m.ID).JoinBy("+"),
                .taxonomy = taxonomy,
                .Properties = v
            }
        Next
    End Function

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
