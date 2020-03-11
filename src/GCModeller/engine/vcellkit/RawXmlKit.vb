
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML
Imports SMRUCC.Rsharp.Runtime
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

    <ExportAPI("time.frames")>
    Public Function timeFrames(raw As vcXML.Reader,
                               <RListObjectArgument>
                               Optional stream As Object = Nothing,
                               Optional env As Environment = Nothing) As Object

        Dim args As list = Internal.Invokes.base.Rlist(stream, env)

        If Not {"transcriptome", "proteome", "metabolome"}.Any(AddressOf args.hasName) Then
            Return Internal.debug.stop({
                "no module system name was specificed for read data!",
                "module name should be in one of: transcriptome, proteome, metabolome",
                "example as: time.frames(..., metabolome = ""mass_profile"")"
            }, env)
        End If

        Dim index As offset() = {}

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
