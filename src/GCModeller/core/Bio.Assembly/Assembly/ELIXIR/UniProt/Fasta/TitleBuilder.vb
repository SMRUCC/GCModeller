
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.Uniprot

    Public Class TitleBuilder : Implements IKeyDataReader

        ReadOnly template As StringTemplate
        ReadOnly extractAll As Boolean

        Dim data As entry

        Sub New(template As StringTemplate, Optional extractAll As Boolean = False)
            Me.template = template
            Me.extractAll = extractAll
        End Sub

        Public Iterator Function Fasta(prot As entry) As IEnumerable(Of FastaSeq)
            Dim prot_seq As String = prot.ProteinSequence

            data = prot

            If extractAll Then
                Call template.SetDefaultKey("uniprot_id", prot.accessions.First)

                Yield New FastaSeq(prot_seq, title:=template.CreateString(Me))
            Else
                For Each id As String In prot.accessions
                    Call template.SetDefaultKey("uniprot_id", id)

                    Yield New FastaSeq With {
                        .SequenceData = prot_seq,
                        .Headers = {template.CreateString(Me)}
                    }
                Next
            End If
        End Function

        Public Function GetData(key As String) As String Implements IKeyDataReader.GetData
            Select Case key
                Case "fullname" : Return data.proteinFullName
                Case "name" : Return data.name
                Case "ncbi_taxid" : Return data.NCBITaxonomyId
                Case "organism" : Return data.OrganismScientificName
                Case "ec_number" : Return data.ECNumberList.JoinBy(",")
                Case "go_id" : Return data.GO.Select(Function(a) a.id).JoinBy(",")
                Case "gene_name" : Return data.geneName
                Case "ORF" : Return data.ORF
                Case "subcellular_location" : Return data.SubCellularLocations.JoinBy(",")

                Case Else
                    If data.xrefs.ContainsKey(key) Then
                        Return data.xrefs(key).Select(Function(a) a.id).JoinBy(",")
                    End If
            End Select

            Return Nothing
        End Function
    End Class
End Namespace