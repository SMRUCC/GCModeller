Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
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
        Return taxonIDtable.Values.Where(clause).ToDictionary(Function(taxon) taxon.TaxonID)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAll() As IReadOnlyDictionary(Of String, TaxonomyRef) Implements IRepositoryRead(Of String, TaxonomyRef).GetAll
        Return New Dictionary(Of String, TaxonomyRef)(taxonIDtable)
    End Function
End Class