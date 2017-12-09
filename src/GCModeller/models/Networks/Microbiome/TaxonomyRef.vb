Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML

''' <summary>
''' Combine the UniProt taxonomy information with the KEGG orthology reference.
''' </summary>
Public Class TaxonomyRef : Implements IKeyedEntity(Of String)

    ''' <summary>
    ''' The NCBI taxonomy id
    ''' </summary>
    ''' <returns></returns>
    Public Property TaxonID As String Implements IKeyedEntity(Of String).Key
    Public Property organism As organism
    Public Property genome As OrthologyTerms

    Public Overrides Function ToString() As String
        Return $"[{TaxonID}] {organism.scientificName}"
    End Function
End Class

Public Class TaxonomyRepository : Implements IRepositoryRead(Of String, TaxonomyRef)

    Dim taxonomyTable As Dictionary(Of String, TaxonomyRef)

    <XmlElement("taxonomy")>
    Public Property Taxonomy As TaxonomyRef()
        Get
            Return taxonomyTable.Values.ToArray
        End Get
        Set(value As TaxonomyRef())
            taxonomyTable = value.ToDictionary(Function(t) t.TaxonID)
        End Set
    End Property

    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, TaxonomyRef).Exists
        Throw New NotImplementedException()
    End Function

    Public Function GetByKey(key As String) As TaxonomyRef Implements IRepositoryRead(Of String, TaxonomyRef).GetByKey
        Throw New NotImplementedException()
    End Function

    Public Function GetWhere(clause As Func(Of TaxonomyRef, Boolean)) As IReadOnlyDictionary(Of String, TaxonomyRef) Implements IRepositoryRead(Of String, TaxonomyRef).GetWhere
        Throw New NotImplementedException()
    End Function

    Public Function GetAll() As IReadOnlyDictionary(Of String, TaxonomyRef) Implements IRepositoryRead(Of String, TaxonomyRef).GetAll
        Throw New NotImplementedException()
    End Function
End Class