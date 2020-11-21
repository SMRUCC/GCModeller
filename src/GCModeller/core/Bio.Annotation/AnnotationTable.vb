Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Annotation.Ptf

Public Class AnnotationTable : Inherits DynamicPropertyBase(Of String)
    Implements INamedValue

    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property geneName As String
    Public Property ORF As String
    Public Property Entrez As String()
    Public Property fullName As String()
    Public Property uniprot As String()
    Public Property GO As String()
    Public Property EC As String()
    Public Property KO As String()
    Public Property pfam As String
    Public Property organism As String

    Public Shared Function FromUnifyPtf(protein As ProteinAnnotation) As AnnotationTable
        Return New AnnotationTable With {
            .ID = protein.geneId,
            .geneName = protein.geneName,
            .EC = protein.get("ec"),
            .Entrez = protein.get("entrez"),
            .fullName = {protein.description},
            .GO = protein.get("go"),
            .KO = protein.get("ko"),
            .ORF = protein("orf"),
            .pfam = protein("pfamstring"),
            .uniprot = protein.get("synonym"),
            .organism = protein("")
        }
    End Function

End Class
