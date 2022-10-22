
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("UniProt")>
Public Module UniProt

    <ExportAPI("ECnumber_pack")>
    Public Function OpenOrCreateEnzymeSequencePack(file As String, Optional create_new As Boolean = False) As ECNumberWriter
        If create_new Then
            Return New ECNumberWriter(file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
        Else
            Return New ECNumberWriter(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
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
End Module
