#Region "Microsoft.VisualBasic::8253ed63a6cab5c3a676df90cecfb156, core\Bio.Annotation\GFF\FeatureKeys.vb"

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

    '     Module FeatureKeys
    ' 
    '         Properties: featuresIndex
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: [GetFeatureType], DbXref, GetAllGeneIDs, (+2 Overloads) GetsAllFeatures
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.TabularFormat.GFF

    Public Module FeatureKeys

        Public Const tRNA As String = "tRNA"
        Public Const CDS As String = "CDS"
        Public Const exon As String = "exon"
        Public Const gene As String = "gene"
        Public Const tmRNA As String = "tmRNA"
        Public Const rRNA As String = "rRNA"
        Public Const region As String = "region"

        <Extension>
        Public Function [GetFeatureType](x As Feature) As Features
            If String.IsNullOrEmpty(x.feature) OrElse Not FeatureKeys.featuresIndex.ContainsKey(x.feature) Then
                Return Features.undefine
            Else
                Return FeatureKeys.featuresIndex(x.feature)
            End If
        End Function

        <Extension>
        Public Function GetsAllFeatures(source As IEnumerable(Of Feature), type As Features) As Feature()
            Return LinqAPI.Exec(Of Feature) <= From x As Feature
                                               In source
                                               Where type = x.GetFeatureType
                                               Select x
        End Function

        <Extension>
        Public Function GetsAllFeatures(gff As GFFTable, type As Features) As Feature()
            Return gff.Features.GetsAllFeatures(type)
        End Function

        ''' <summary>
        ''' string to <see cref="Features"/> enum values
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property featuresIndex As IReadOnlyDictionary(Of String, Features)

        Sub New()
            featuresIndex = New Dictionary(Of String, Features) From {
 _
                {FeatureKeys.CDS, Features.CDS},
                {FeatureKeys.exon, Features.exon},
                {FeatureKeys.gene, Features.gene},
                {FeatureKeys.region, Features.region},
                {FeatureKeys.rRNA, Features.rRNA},
                {FeatureKeys.tmRNA, Features.tmRNA},
                {FeatureKeys.tRNA, Features.tRNA}
            }
        End Sub

        ''' <summary>
        ''' 获取所有的CDS的基因编号列表
        ''' </summary>
        ''' <param name="gff"></param>
        ''' <param name="feature">默认是使用所有的feature类型来用作为数据源</param>
        ''' <returns></returns>
        ''' <remarks>这个函数似乎有问题，因为使用人类基因组的第一条染色体的GFF测试才2000多个基因</remarks>
        <Extension>
        Public Function GetAllGeneIDs(gff As GFFTable, Optional feature As Features = Features.undefine) As String()
            Dim fs As Feature() = If(feature = Features.undefine, gff.Features, gff.GetsAllFeatures(feature))
            Dim geneIDs As String() = fs _
                .Where(Function(f) f.attributes.ContainsKey("dbxref")) _
                .Select(Function(f) f.attributes("dbxref")) _
                .Distinct _
                .ToArray
            geneIDs = geneIDs _
                .Select(AddressOf DbXref) _
                .Where(Function(s) s.ContainsKey("GeneID")) _
                .Select(Function(s) s("GeneID")) _
                .IteratesALL _
                .Distinct _
                .ToArray

            Return geneIDs
        End Function

        ''' <summary>
        ''' 解析出DbXref属性之中的外部数据库连接
        ''' </summary>
        ''' <param name="value$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function DbXref(value$) As Dictionary(Of String, String())
            Dim t$() = value.Split(","c)
            Dim d As Dictionary(Of String, String()) = t _
                .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                .GroupBy(Function(o) o.Name) _
                .ToDictionary(Function(k) k.Key,
                              Function(v)
                                  Return v.Select(Function(x) x.Value).ToArray
                              End Function)
            Return d
        End Function
    End Module
End Namespace
