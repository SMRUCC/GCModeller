
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("UniProt")>
Public Module UniProt

    <ExportAPI("ECnumber_pack")>
    <RApiReturn(GetType(ECNumberWriter), GetType(ECNumberReader))>
    Public Function OpenOrCreateEnzymeSequencePack(file As String, Optional create_new As Boolean = False) As Object
        If create_new Then
            Return New ECNumberWriter(file.Open(
                mode:=FileMode.OpenOrCreate,
                doClear:=True,
                [readOnly]:=False
            ))
        Else
            Return New ECNumberReader(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        End If
    End Function

    <ExportAPI("add_ecNumbers")>
    Public Function Addnumbers(pack As ECNumberWriter,
                               <RRawVectorArgument>
                               uniprot As Object,
                               Optional env As Environment = Nothing) As Object

        Dim source = getUniprotData(uniprot, env)

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        Else
            For Each protein As entry In source.TryCast(Of IEnumerable(Of entry))
                Call pack.AddProtein(protein)
            Next
        End If

        Return Nothing
    End Function

    <ExportAPI("extract_fasta")>
    Public Function ExtractFasta(pack As ECNumberReader, Optional enzyme As Boolean = True) As FastaFile
        Return New FastaFile(pack.QueryFasta(enzymeQuery:=enzyme))
    End Function
End Module
