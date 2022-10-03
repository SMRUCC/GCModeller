Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' The protein annotation metadata
''' </summary>
<Package("ptf")> Module PTFCache

    <ExportAPI("loadBackgroundModel")>
    Public Function loadModel(ptf As StreamPack, database As String) As Background
        Dim data = New PtfReader(ptf).LoadCrossReference(key:=database)
        Dim clusters As Cluster() = data _
            .Select(Function(c)
                        Return New Cluster With {
                            .ID = c.Key,
                            .description = c.Key,
                            .names = c.Key,
                            .members = c.Value _
                                .Select(Function(id)
                                            Return New BackgroundGene With {
                                                .accessionID = id,
                                                .[alias] = {id},
                                                .locus_tag = New NamedValue With {.name = id, .text = id},
                                                .name = id,
                                                .term_id = {id}
                                            }
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return New Background With {
            .id = database,
            .name = database,
            .build = Now,
            .clusters = clusters,
            .comments = database
        }
    End Function

    <ExportAPI("cache.ptf")>
    Public Function writePtfFile(<RRawVectorArgument>
                                 uniprot As Object,
                                 file As Object,
                                 <RRawVectorArgument(GetType(String))>
                                 Optional db_xref As Object = "KEGG|KO|GO|Pfam|RefSeq|EC|InterPro|BioCyc|eggNOG|keyword",
                                 Optional cacheTaxonomy As Boolean = False,
                                 Optional hds_stream As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim source = getUniprotData(uniprot, env)
        Dim keys As String() = REnv.asVector(Of String)(db_xref)

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        Else
            Return source.TryCast(Of IEnumerable(Of entry)).writePtfInternal(
                file:=file,
                cacheTaxonomy:=cacheTaxonomy,
                hds_stream:=hds_stream,
                env:=env,
                idMapping:=keys
            )
        End If
    End Function

    <Extension>
    Private Function writePtfInternal(source As IEnumerable(Of entry),
                                      file As Object,
                                      idMapping As String(),
                                      cacheTaxonomy As Boolean,
                                      hds_stream As Boolean,
                                      env As Environment) As Object
        Dim stream As StreamWriter
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)
        Dim keys As String = idMapping.JoinBy(",")

        If TypeOf file Is StreamWriter Then
            stream = file
        ElseIf buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        If hds_stream Then
            Dim fileObj As Stream = buffer.TryCast(Of Stream)
            Call fileObj.SetLength(0)

            Using cache As New PtfWriter(fileObj, idMapping)
                For Each protein As ProteinAnnotation In source.Select(Function(p) AnnotationCache.toPtf(p, cacheTaxonomy, keys))
                    Call cache.AddProtein(protein)
                Next
            End Using
        Else
            stream = New StreamWriter(buffer.TryCast(Of Stream))

            Call source.WritePtfCache(stream, cacheTaxonomy, keys)
            Call stream.Flush()

            If TypeOf file Is String Then
                Call stream.Close()
            End If
        End If

        Return Nothing
    End Function
End Module
