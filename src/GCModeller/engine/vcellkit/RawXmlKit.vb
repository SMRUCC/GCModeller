
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' 
''' </summary>
<Package("vcellkit.rawXML", Category:=APICategories.UtilityTools, Publisher:="gg.xie@bionovogene.com")>
Module RawXmlKit

    ''' <summary>
    ''' open gcXML raw data file for read/write
    ''' </summary>
    ''' <param name="file$"></param>
    ''' <param name="mode$"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("open.vcellXml")>
    Public Function xmlWriter(file$,
                              Optional mode$ = "read",
                              <RListObjectArgument>
                              Optional args As Object = Nothing,
                              Optional env As Environment = Nothing) As Object

        Dim arguments As list = Internal.Invokes.base.Rlist(args, env)

        If LCase(mode) = "read" Then
            Return New vcXML.Reader(file)
        ElseIf LCase(mode) = "write" Then
            Dim vcell As Engine = arguments.getValue(Of Engine)("vcell", env)

            If vcell Is Nothing Then
                Return Internal.debug.stop("missing vcell engine argument value!", env)
            Else
                Return New VcellAdapterDriver(file, vcell.model, vcell.dynamics)
            End If
        Else
            Return Internal.debug.stop($"unknown I/O mode: {mode}...", env)
        End If
    End Function

    ''' <summary>
    ''' [debug api]
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("frame.index")>
    Public Function getOffsetIndex(raw As vcXML.Reader) As offset()
        Return raw.allFrames
    End Function

    ''' <summary>
    ''' get a frame matrix for compares between different samples.
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="tick"></param>
    ''' <param name="stream"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("frame.matrix")>
    Public Function extractFrameMatrix(raw As String(), tick As Integer, <RListObjectArgument> stream As Object, Optional env As Environment = Nothing) As Object
        Dim args As list = Internal.Invokes.base.Rlist(stream, env)
        Dim message As Message = checkStreamRef(args, env)

        If Not message Is Nothing Then
            Return message
        End If

        Dim moduleName$ = args.slots.Keys.First
        Dim contentType$ = args.getValue(Of String)(moduleName, env)
        Dim matrix As New Dictionary(Of String, DataSet)

        For Each file As String In raw
            Using xml As New vcXML.Reader(file)
                Dim index As offset = xml _
                    .getStreamIndex(moduleName)(contentType) _
                    .Where(Function(frame) frame.tick = tick) _
                    .FirstOrDefault

                If Not index Is Nothing Then
                    Dim entities As String() = xml.getStreamEntities(index.module, index.content_type)
                    Dim vec As Double() = xml.getFrameVector(index.offset)
                    Dim sampleName As String = xml.basename
                    Dim entity As String

                    For i As Integer = 0 To entities.Length - 1
                        entity = entities(i)

                        If Not matrix.ContainsKey(entity) Then
                            Call New DataSet With {
                                .ID = entity,
                                .Properties = New Dictionary(Of String, Double)
                            }.DoCall(Sub(r)
                                         matrix.Add(entity, r)
                                     End Sub)
                        End If

                        matrix(entity).Add(sampleName, vec(i))
                    Next
                Else
                    env.AddMessage($"missing time frame '{tick}' in {xml.basename}!", MSG_TYPES.WRN)
                End If
            End Using
        Next

        Return matrix
    End Function

    Private Function checkStreamRef(args As list, env As Environment) As Message
        If Not {"transcriptome", "proteome", "metabolome"}.Any(AddressOf args.hasName) Then
            Return Internal.debug.stop({
                "no module system name was specificed for read data!",
                "module name should be in one of: transcriptome, proteome, metabolome",
                "example as: time.frames(..., metabolome = ""mass_profile"")"
            }, env)
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Get a sample matrix data in a timeline.
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="stream"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("time.frames")>
    Public Function timeFrames(raw As vcXML.Reader, <RListObjectArgument> stream As Object, Optional env As Environment = Nothing) As Object
        Dim args As list = Internal.Invokes.base.Rlist(stream, env)
        Dim index As offset() = {}
        Dim message As Message = checkStreamRef(args, env)

        If Not message Is Nothing Then
            Return message
        End If

        For Each name As String In {"transcriptome", "proteome", "metabolome"}
            If args.hasName(name) Then
                index = raw.getStreamIndex(name)(args.getValue(Of String)(name, env)) _
                    .OrderBy(Function(p) p.id) _
                    .ToArray
                Exit For
            End If
        Next

        Dim entities As DataSet() = raw _
            .getStreamEntities(index(Scan0).module, index(Scan0).content_type) _
            .Select(Function(id)
                        Return New DataSet With {
                            .ID = id,
                            .Properties = New Dictionary(Of String, Double)
                        }
                    End Function) _
            .ToArray
        Dim vector As Double()

        For Each offset As offset In index
            vector = raw.getFrameVector(offset.offset)

            For i As Integer = 0 To vector.Length - 1
                entities(i).Add(offset.id, vector(i))
            Next
        Next

        Return entities
    End Function
End Module
