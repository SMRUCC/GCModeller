Imports System.Data.Linq.Mapping
Imports System.Data.SQLite.Linq.DataMapping.Interface.QueryBuilder
Imports SMRUCC.genomics.Assembly.Uniprot

Public Class Uniprot : Inherits DbFileSystemObject
    Implements DbFileSystemObject.ProteinDescriptionModel

    <Column(Name:="uniprot_id", IsPrimaryKey:=True, DbType:="varchar(64)")> Public Property UniprotID As String
    <Column(Name:="Organism_Species", DbType:="varchar(2048)")> Public Property OrganismSpecies As String Implements DbFileSystemObject.ProteinDescriptionModel.OrganismSpecies
    ''' <summary>
    ''' GeneName(基因名称)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Column(Name:="gene_name", DbType:="varchar(1024)")> Public Property GeneName As String
    <Column(Name:="pe", DbType:="varchar(64)")> Public Property PE As String
    <Column(Name:="sv", DbType:="varchar(64)")> Public Property SV As String

    Public Overrides Function GetPath(RepositoryRoot As String) As String
        Dim FilePath As String = RepositoryRoot & "/" & OrganismSpecies.ToUpper.First & "/" & OrganismSpecies.NormalizePathString() & ".fasta"
        Return FilePath
    End Function

    Public Shared Function CreateObject(Fasta As UniprotFasta, MD5 As String) As Uniprot
        Dim Entry As New Uniprot With {
            .Definition = Fasta.ProtName.TrimmingSQLConserved,
            .GeneName = Fasta.GN,
            .LocusID = Fasta.EntryName,
            .MD5Hash = MD5,
            .OrganismSpecies = Fasta.OrgnsmSpName.TrimmingSQLConserved,
            .PE = Fasta.PE,
            .SV = Fasta.SV,
            .UniprotID = Fasta.UniprotID
        }
        Return Entry
    End Function

    Public Property COG As String Implements DbFileSystemObject.ProteinDescriptionModel.COG
End Class
