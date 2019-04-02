Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.Polypeptides

''' <summary>
''' Create genetic components from Uniprot database.
''' </summary>
Public Module UniProtExtensions

    <Extension>
    Public Iterator Function CreateDump(uniprot As IEnumerable(Of entry)) As IEnumerable(Of GeneticNode)
        For Each protein As entry In uniprot.Where(Function(g) Not g.sequence Is Nothing)
            Dim KO As dbReference = protein.KO

            If KO Is Nothing Then
                Continue For
            End If

            Dim ntAccess As [property] = protein.Xrefs.TryGetValue("RefSeq") _
                ?.FirstOrDefault _
                ?.properties _
                 .FirstOrDefault(Function(p) p.type = "nucleotide sequence ID")

            Yield New GeneticNode With {
                .KO = KO.id,
                .[Function] = protein.proteinFullName,
                .ID = protein.accessions.First,
                .Sequence = Polypeptide _
                    .ConstructVector(protein.ProteinSequence) _
                    .ToArray,
                .GO = protein.Xrefs _
                    .TryGetValue("GO", [default]:={}) _
                    .Select(Function(g) g.id) _
                    .ToArray,
                .Xref = .ID,
                .Accession = ntAccess?.value,
                .Nt = {}
            }
        Next
    End Function
End Module
