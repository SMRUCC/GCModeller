#Region "Microsoft.VisualBasic::da2447b2e27c381bf32750676e7281a5, GCModeller\core\Bio.Assembly\Metagenomics\BIOMTaxonomy.vb"

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

    '   Total Lines: 230
    '    Code Lines: 138
    ' Comment Lines: 61
    '   Blank Lines: 31
    '     File Size: 10.07 KB


    '     Module BIOMTaxonomy
    ' 
    '         Properties: BIOMPrefix, BIOMPrefixAlt, BriefParser, CompleteParser
    '         Delegate Function
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: AsTaxonomy, FillLineageEmpty, TaxonomyFromString, (+2 Overloads) TaxonomyParser, TaxonomyParserAlt
    '                       TaxonomyString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace Metagenomics

    Public Module BIOMTaxonomy

        ''' <summary>
        ''' ``k__{x.superkingdom};p__{x.phylum};c__{x.class};o__{x.order};f__{x.family};g__{x.genus};s__{x.species}``
        ''' </summary>
        Public ReadOnly Property BIOMPrefix As String() = {"k__", "p__", "c__", "o__", "f__", "g__", "s__"}

        ''' <summary>
        ''' ``["superkingdom__", "phylum__", "class__", "order__", "family__", "genus__", "species__"]``
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Example:
        ''' 
        ''' ```
        ''' no rank__cellular organisms;superkingdom__Eukaryota;no rank__Opisthokonta;kingdom__Fungi;subkingdom__Dikarya;phylum__Ascomycota;no rank__saccharomyceta;subphylum__Pezizomycotina;no rank__leotiomyceta;no rank__dothideomyceta;class__Dothideomycetes;no rank__Dothideomycetes incertae sedis;order__Botryosphaeriales;family__Botryosphaeriaceae;genus__Macrophomina;species__Macrophomina phaseolina;no rank__Macrophomina phaseolina MS6
        ''' ```
        ''' </remarks>
        Public ReadOnly Property BIOMPrefixAlt As Index(Of String) = {
            "superkingdom__", "phylum__", "class__", "order__", "family__", "genus__", "species__"
        }

        Public ReadOnly Property BriefParser As [Default](Of TaxonomyLineageParser) = New TaxonomyLineageParser(AddressOf TaxonomyParser)
        Public ReadOnly Property CompleteParser As [Default](Of TaxonomyLineageParser) = New TaxonomyLineageParser(AddressOf TaxonomyParserAlt)

        Public Delegate Function TaxonomyLineageParser(taxonomy As String) As Dictionary(Of String, String)

        Dim biomPrefixTable As New Dictionary(Of String, String)

        Sub New()
            Dim word$

            For Each prefix As String In BIOMPrefix
                word = prefix.Trim("_"c)

                Call biomPrefixTable.Add(prefix, word)
                Call biomPrefixTable.Add(word, word)
            Next

            For i As Integer = 0 To BIOMPrefix.Length - 1
                word = BIOMPrefix(i).Trim("_"c)

                Call biomPrefixTable.Add(BIOMPrefixAlt(i), word)
                Call biomPrefixTable.Add(BIOMPrefixAlt(i).Trim("_"c), word)
            Next
        End Sub

        ''' <summary>
        ''' Contact the taxonomy lineage tokens as a taxonomy lineage string uin BIOM format. 
        ''' </summary>
        ''' <param name="lineage">
        ''' Lineage tokens text array data from <see cref="Taxonomy.Select"/> method.
        ''' (这里所输入的字符串数组可以是没有BIOM前缀的，也可以是具有分类等级前缀的，这个函数会自动执行归一化)
        ''' </param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function TaxonomyString(lineage As String()) As String
            Return lineage _
                .SeqIterator _
                .Select(Function(level)
                            Dim node As NamedValue(Of String) = level.value.GetTagValue("__")
                            Dim prefix As String = LCase(node.Name)

                            If Not prefix.StringEmpty Then
                                prefix = biomPrefixTable(prefix)
                            Else
                                prefix = BIOMPrefix(level).Trim("_"c)
                            End If

                            Return $"{prefix}__{node.Value}"
                        End Function) _
                .JoinBy(";")
        End Function

        ReadOnly descRanks$() = NcbiTaxonomyTree.stdranks.Objects.Reverse.ToArray

        ''' <summary>
        ''' 这个函数不像<see cref="TaxonomyParser"/>和<see cref="TaxonomyParserAlt"/>这两个是专门针对
        ''' 具有分类信息前缀的字符串的解析。这个函数是主要针对于没有分类层次信息前缀的字符串的解析函数，
        ''' 由于没有分类信息的前缀字符串，故而这个函数也主要是依靠tokens之间的前后顺序来赋值分类的rank信息
        ''' </summary>
        ''' <param name="lineage">从大到小排序的</param>
        ''' <returns></returns>
        <Extension>
        Public Function TaxonomyFromString(lineage$, Optional deli As Char = ";"c) As Taxonomy
            ' 有些分类字符串里面会存在[....]这种形式的格式，不清楚是什么意思
            ' 在这里去除掉[和]
            Dim tokens$() = lineage _
                .Replace("[", "") _
                .Replace("]", "") _
                .Split(deli) _
                .Select(AddressOf Strings.Trim) _
                .ToArray
            Dim taxonomyRanks = tokens _
                .SeqIterator _
                .ToDictionary(Function(i) descRanks(i),
                              Function(rank)
                                  Return rank.value
                              End Function)

            Return New Taxonomy(taxonomyRanks)
        End Function

        ''' <summary>
        ''' ``New <see cref="Taxonomy"/>(<paramref name="lineage"/>)``
        ''' </summary>
        ''' <param name="lineage"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsTaxonomy(lineage As Dictionary(Of String, String)) As Taxonomy
            Return New Taxonomy(lineage)
        End Function

#Region "Parsing BIOM style taxonomy string"

        ''' <summary>
        ''' For <see cref="BIOMPrefix"/>
        ''' </summary>
        ''' <param name="taxonomy$"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' greengenes数据库之中的taxonomy name会存在``[]``这类的符号，不清楚是因为什么？
        ''' 在这里替换掉
        ''' </remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TaxonomyParser(taxonomy As String) As Dictionary(Of String, String)
            Return TaxonomyParser(taxonomy.Split(";"c))
        End Function

        ''' <summary>
        ''' For <see cref="BIOMPrefix"/>
        ''' </summary>
        ''' <param name="taxonomy$"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' greengenes数据库之中的taxonomy name会存在``[]``这类的符号，不清楚是因为什么？
        ''' 在这里替换掉
        ''' </remarks>
        Public Function TaxonomyParser(taxonomy As String()) As Dictionary(Of String, String)
            Dim tokens$() = taxonomy _
                .Select(Function(name)
                            Return name _
                                .Replace("[", "") _
                                .Replace("]", "") _
                                .StringReplace("\s+", " ")
                        End Function) _
                .Select(AddressOf Strings.Trim) _
                .ToArray
            Dim catalogs As NamedValue(Of String)() = tokens _
                .Select(Function(t) t.GetTagValue("__")) _
                .ToArray
            Dim out As New Dictionary(Of String, String)

            For Each level As NamedValue(Of String) In catalogs
                Dim name$ = level.Value.StringReplace("[_]{2,}", "_")

                ' "superkingdom__", "phylum__", "class__", "order__", "family__", "genus__", "species__"
                Select Case LCase(level.Name)
                    Case "k", "superkingdom"
                        Call out.Add(NcbiTaxonomyTree.superkingdom, name)
                    Case "p", "phylum"
                        Call out.Add(NcbiTaxonomyTree.phylum, name)
                    Case "c", "class"
                        Call out.Add(NcbiTaxonomyTree.class, name)
                    Case "o", "order"
                        Call out.Add(NcbiTaxonomyTree.order, name)
                    Case "f", "family"
                        Call out.Add(NcbiTaxonomyTree.family, name)
                    Case "g", "genus"
                        Call out.Add(NcbiTaxonomyTree.genus, name)
                    Case "s", "species"
                        Call out.Add(NcbiTaxonomyTree.species, name)
                    Case Else
                End Select
            Next

            Return out
        End Function

        ''' <summary>
        ''' For <see cref="BIOMPrefixAlt"/>
        ''' </summary>
        ''' <param name="taxonomy$"></param>
        ''' <returns></returns>
        Public Function TaxonomyParserAlt(taxonomy$) As Dictionary(Of String, String)
            Dim tokens$() = taxonomy.Split(";"c)
            Dim catalogs As NamedValue(Of String)() = tokens _
                .Select(Function(t) t.GetTagValue("__")) _
                .ToArray
            Dim ranks As New Dictionary(Of String, String)
            Dim rankName As String

            For Each rank As NamedValue(Of String) In catalogs
                rankName = LCase(rank.Name)

                If _BIOMPrefixAlt.IndexOf(rankName) > -1 OrElse _BIOMPrefixAlt.IndexOf(rankName & "__") > -1 Then
                    Call ranks.Add(rank.Name, rank.Value)
                ElseIf rankName = "kingdom__" OrElse rankName = "kingdom" Then
                    Call ranks.Add("superkingdom", rank.Value)
                End If
            Next

            Return ranks
        End Function
#End Region

        <Extension>
        Public Function FillLineageEmpty(lineage As Dictionary(Of String, String), Optional empty$ = "NA") As Dictionary(Of String, String)
            For Each level As String In NcbiTaxonomyTree.stdranks
                If Not lineage.ContainsKey(level) Then
                    Call lineage.Add(level, empty)
                End If
            Next

            Return lineage
        End Function
    End Module
End Namespace
