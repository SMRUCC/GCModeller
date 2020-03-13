Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("uniprot")>
Module uniprot

    <ExportAPI("open.uniprot")>
    Public Function openUniprotXmlAssembly(file As String, Optional isUniParc As Boolean = False) As pipeline
        Return UniProtXML _
            .EnumerateEntries(file, isUniParc) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("protein.seqs")>
    Public Function getProteinSeq(<RRawVectorArgument> uniprot As Object, Optional env As Environment = Nothing) As pipeline
        If uniprot Is Nothing Then
            Return Nothing
        End If

        Dim protFa = Function(prot As entry) As FastaSeq
                         Return New FastaSeq With {
                             .Headers = {prot.accessions(Scan0)},
                             .SequenceData = prot.ProteinSequence
                         }
                     End Function

        If TypeOf uniprot Is entry() Then
            Return DirectCast(uniprot, entry()) _
                .Select(Function(prot)
                            Return protFa(prot)
                        End Function) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        ElseIf TypeOf uniprot Is pipeline AndAlso DirectCast(uniprot, pipeline).elementType Is GetType(entry) Then
            Return DirectCast(uniprot, pipeline) _
                .populates(Of entry) _
                .Select(Function(prot) protFa(prot)) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return Internal.debug.stop($"invalid data source input: {uniprot.GetType.FullName}!", env)
        End If
    End Function
End Module
