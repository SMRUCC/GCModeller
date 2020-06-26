Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Module AnnotationCache

    <Extension>
    Public Sub WritePtfCache(proteins As IEnumerable(Of entry), cache As TextWriter)
        For Each protein As entry In proteins
            Call cache.WriteLine(PtfFile.ToString(toPtf(protein)))
        Next
    End Sub

    Private Function toPtf(protein As entry) As ProteinAnnotation
        Return New ProteinAnnotation With {
            .geneId = protein.accessions(Scan0),
            .description = protein.proteinFullName
        }
    End Function
End Module
