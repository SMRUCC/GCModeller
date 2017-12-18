Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' Combine the UniProt taxonomy information with the KEGG orthology reference.
''' </summary>
Public Class TaxonomyRef : Implements IKeyedEntity(Of String)

    ''' <summary>
    ''' The NCBI taxonomy id
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <XmlAttribute("ncbi_taxon_id")>
    Public Property TaxonID As String Implements IKeyedEntity(Of String).Key
    Public Property organism As organism
    Public Property genome As OrthologyTerms
    ''' <summary>
    ''' 具有KEGG直系同源注释结果的基因的数量和该基因组内的总基因数量的比值结果
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property Coverage As Double

    Dim ts As Taxonomy

    Public ReadOnly Property TaxonomyString As Taxonomy
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            ' IsFalse not equals not operator??
            If ts Then
                ' Do Nothing
            Else
                ts = New Taxonomy(organism.lineage.taxonlist)
            End If

            Return ts
        End Get
    End Property

    Public ReadOnly Property KOTerms As String()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return genome _
                .Terms _
                .SafeQuery _
                .Select(Function(t) t.name) _
                .ToArray
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"[{TaxonID}] {organism.scientificName}"
    End Function
End Class

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