
Imports Microsoft.VisualBasic.CommandLine.Reflection
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
            Dim vcell As Engine = arguments!vcell

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
    Public Function timeFrames(raw As vcXML.Reader, stream As list) As Object

    End Function
End Module
