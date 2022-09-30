Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' The protein annotation metadata
''' </summary>
<Package("ptf")> Module PTFCache

    <ExportAPI("cache.ptf")>
    Public Function writePtfFile(<RRawVectorArgument>
                                 uniprot As Object,
                                 file As Object,
                                 Optional cacheTaxonomy As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim source = getUniprotData(uniprot, env)

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        Else
            Dim stream As StreamWriter

            If file Is Nothing Then
                Return Internal.debug.stop({"file output can not be nothing!"}, env)
            ElseIf TypeOf file Is String Then
                stream = DirectCast(file, String).OpenWriter
            ElseIf TypeOf file Is Stream Then
                stream = New StreamWriter(DirectCast(file, Stream)) With {.NewLine = True}
            ElseIf TypeOf file Is StreamWriter Then
                stream = DirectCast(file, StreamWriter)
            Else
                Return Internal.debug.stop(Message.InCompatibleType(GetType(Stream), file.GetType, env,, NameOf(file)), env)
            End If

            Call source.TryCast(Of IEnumerable(Of entry)).WritePtfCache(stream, cacheTaxonomy)
            Call stream.Flush()

            If TypeOf file Is String Then
                Call stream.Close()
            End If

            Return Nothing
        End If
    End Function
End Module
