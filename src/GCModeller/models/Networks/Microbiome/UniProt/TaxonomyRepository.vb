#Region "Microsoft.VisualBasic::23e39976893430965ce6835e77c7a36e, models\Networks\Microbiome\UniProt\TaxonomyRepository.vb"

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

    ' Class TaxonomyRepository
    ' 
    '     Properties: base, MimeType, taxonomy
    ' 
    '     Function: Exists, GetAll, GetByKey, getRanks, GetWhere
    '               LoadByTaxonomyId, LoadRepository, Selects, StorageReference, TaxonomyGroup
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' A taxonomy genome database which created based on the uniprot database.
''' The reference data is key indexed by <see cref="TaxonomyRef.TaxonID"/>
''' (使用NCBI的物种编号作为主键的)
''' </summary>
''' <remarks>
''' #### 2019-06-10 因为应用于微生物组分析的话,因为微生物的物种数量很多,这个参考库往往很大
''' 所以在这里不建议将物种参考数据都放在一个文件之中,这样子会造成数据的提取效率过低
''' </remarks>
Public Class TaxonomyRepository
    Implements IFileReference
    Implements IRepositoryRead(Of String, TaxonomyRef)

    ''' <summary>
    ''' ``[taxonomy_id => lineage]``
    ''' 
    ''' 这个参考库应该是使用json格式保存的
    ''' </summary>
    ''' <returns></returns>
    Public Property taxonomy As Dictionary(Of String, Taxonomy)

    ''' <summary>
    ''' [taxonomy_id => reference]
    ''' </summary>
    Friend ReadOnly cache As New Dictionary(Of String, TaxonomyRef)

    ''' <summary>
    ''' 用于查找给定的编号的物种参考数据信息
    ''' </summary>
    ''' <returns></returns>
    Private Property base As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function TaxonomyGroup(rank As TaxonomyRanks) As Dictionary(Of String, TaxonomyRef())
        Return taxonomy _
            .GroupBy(Function(t)
                         Return t.Value.TaxonomyRankString(rank)
                     End Function) _
            .ToDictionary(Function(g) g.Key, AddressOf getRanks)
    End Function

    Private Function getRanks(g As IGrouping(Of String, KeyValuePair(Of String, Taxonomy))) As TaxonomyRef()
        Return g _
            .Select(Function(a) a.Key) _
            .Select(AddressOf GetByKey) _
            .ToArray
    End Function

    ' 2018-3-12
    ' 在这里应该先构建一个二叉树，在进行查找，查找效率会具有很大的提升空间

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Selects(range As Taxonomy) As IEnumerable(Of TaxonomyRef)
        Return taxonomy _
            .SelectByTaxonomyRange(
                ref:=range,
                getTaxonomy:=Function(genome) genome.Value,
                getValue:=Function(genome) genome.Key
            ).Select(Function(taxid)
                         Return GetByKey(taxid)
                     End Function)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, TaxonomyRef).Exists
        Return taxonomy.ContainsKey(key)
    End Function

    ''' <summary>
    ''' Get cache or load from file.(这个函数会利用缓存来加速数据的查找效率)
    ''' </summary>
    ''' <param name="key">NCBI taxonomy id</param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByKey(key As String) As TaxonomyRef Implements IRepositoryRead(Of String, TaxonomyRef).GetByKey
        Return cache.ComputeIfAbsent(key, lazyValue:=AddressOf LoadByTaxonomyId)
    End Function

    ''' <summary>
    ''' 这个函数不考虑Cache,直接从文件加载数据
    ''' </summary>
    ''' <param name="taxid"></param>
    ''' <returns></returns>
    Public Function LoadByTaxonomyId(taxid As String) As TaxonomyRef
        Dim taxonomy As Taxonomy = Me.taxonomy(taxid)
        Dim path As String = $"{StorageReference(taxonomy, relative:=False)}/{taxid}.Xml"

        Return path.LoadXml(Of TaxonomyRef)(throwEx:=False)
    End Function

    ''' <summary>
    ''' 这个函数只是得到了文件夹部分,还需要与taxonomy id组合在一起才能够得到文件
    ''' </summary>
    ''' <param name="taxonomy"></param>
    ''' <returns></returns>
    Public Function StorageReference(taxonomy As Taxonomy, Optional relative As Boolean = True) As String
        Dim part As String = taxonomy _
            .Select(TaxonomyRanks.Genus) _
            .Select(Function(t) t.NormalizePathString(alphabetOnly:=False)) _
            .JoinBy("/") _
            .Trim("/"c, "_"c)
        Dim groupName$ = $"/{part}"

        If Not relative Then
            groupName = $"{base.ParentPath}/taxonomy_ref/{groupName}"
        End If

        Return groupName
    End Function

    ''' <summary>
    ''' 并不推荐使用这个函数以及<see cref="GetAll()"/>函数
    ''' </summary>
    ''' <param name="clause"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetWhere(clause As Func(Of TaxonomyRef, Boolean)) As IReadOnlyDictionary(Of String, TaxonomyRef) Implements IRepositoryRead(Of String, TaxonomyRef).GetWhere
        Return taxonomy.Keys.Select(Function(tax) (taxid:=tax, GetByKey(tax))) _
            .Where(Function(tax)
                       Return clause(tax.Item2)
                   End Function) _
            .ToDictionary(Function(taxon) taxon.taxid,
                          Function(taxon) taxon.Item2)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAll() As IReadOnlyDictionary(Of String, TaxonomyRef) Implements IRepositoryRead(Of String, TaxonomyRef).GetAll
        Return taxonomy.Keys.ToDictionary(Function(taxid) taxid, Function(taxid) GetByKey(taxid))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadRepository(indexJson As String) As TaxonomyRepository
        Return indexJson _
            .LoadJSON(Of TaxonomyRepository) _
            .With(Sub(repo)
                      repo.base = indexJson
                  End Sub)
    End Function
End Class
