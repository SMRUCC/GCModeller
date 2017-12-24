#Region "Microsoft.VisualBasic::f813aa0f17674517b6a1448edaf82caf, ..\GCModeller\models\Networks\Microbiome\UniProt\TaxonomyRepository.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' 使用NCBI的物种编号作为主键的
''' </summary>
Public Class TaxonomyRepository : Implements IRepositoryRead(Of String, TaxonomyRef)

    Dim taxonIDtable As Dictionary(Of String, TaxonomyRef)

    <XmlElement("taxonomy")>
    Public Property Taxonomy As TaxonomyRef()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return taxonIDtable.Values.ToArray
        End Get
        Set(value As TaxonomyRef())
            taxonIDtable = value _
                .ToDictionary(Function(t)
                                  Return t.TaxonID
                              End Function)
        End Set
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function TaxonomyGroup(rank As TaxonomyRanks) As Dictionary(Of String, TaxonomyRef())
        Return taxonIDtable _
            .Values _
            .GroupBy(Function(t)
                         Return t.TaxonomyString.TaxonomyRankString(rank)
                     End Function) _
            .ToDictionary(Function(g) g.Key,
                          Function(g) g.ToArray)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Selects(range As Taxonomy) As IEnumerable(Of TaxonomyRef)
        Return taxonIDtable _
            .Values _
            .SelectByTaxonomyRange(
                ref:=range,
                getTaxonomy:=Function(genome) genome.TaxonomyString,
                getValue:=Function(genome) genome
            )
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, TaxonomyRef).Exists
        Return taxonIDtable.ContainsKey(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByKey(key As String) As TaxonomyRef Implements IRepositoryRead(Of String, TaxonomyRef).GetByKey
        Return taxonIDtable(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetWhere(clause As Func(Of TaxonomyRef, Boolean)) As IReadOnlyDictionary(Of String, TaxonomyRef) Implements IRepositoryRead(Of String, TaxonomyRef).GetWhere
        Return taxonIDtable _
            .Values _
            .Where(clause) _
            .ToDictionary(Function(taxon) taxon.TaxonID)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAll() As IReadOnlyDictionary(Of String, TaxonomyRef) Implements IRepositoryRead(Of String, TaxonomyRef).GetAll
        Return New Dictionary(Of String, TaxonomyRef)(taxonIDtable)
    End Function
End Class
