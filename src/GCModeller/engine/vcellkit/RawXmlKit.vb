﻿#Region "Microsoft.VisualBasic::2f53e5cbd8ca382c3977aca12b466fce, GCModeller\engine\vcellkit\RawXmlKit.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 256
'    Code Lines: 175
' Comment Lines: 46
'   Blank Lines: 35
'     File Size: 9.88 KB


' Module RawXmlKit
' 
'     Function: checkStreamRef, extractFrameMatrix, getEntityNames, getOffsetIndex, timeFrames
'               xmlWriter
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.Raw
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports HTS_Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports XmlOffset = SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML.XML.offset

''' <summary>
''' the virtual cell raw data
''' </summary>
<Package("rawXML", Category:=APICategories.UtilityTools, Publisher:="gg.xie@bionovogene.com")>
Module RawXmlKit

    <ExportAPI("open.vcellPack")>
    Public Function binaryWriter(file$,
                                 Optional mode$ = "read",
                                 <RListObjectArgument>
                                 Optional args As Object = Nothing,
                                 Optional env As Environment = Nothing) As Object

        Dim arguments As list = Internal.Invokes.base.Rlist(args, env)

        If LCase(mode) = "read" Then
            Return New Raw.Reader(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        ElseIf LCase(mode) = "write" Then
            Dim vcell As Engine = arguments.getValue(Of Engine)("vcell", env)

            If vcell Is Nothing Then
                Return Internal.debug.stop("missing vcell engine argument value!", env)
            Else
                Return New StorageDriver(file, vcell.model)
            End If
        Else
            Return Internal.debug.stop($"unknown I/O mode: {mode}...", env)
        End If
    End Function

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
    Public Function getOffsetIndex(raw As vcXML.Reader) As XmlOffset()
        Return raw.allFrames
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="stream">
    ''' module descripting of the stream content to read, should be a list of content type mapping:
    ''' list element name could be: "transcriptome", "proteome", "metabolome"
    ''' element content type could be: mass_profile, activity, flux_size
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("entity.names")>
    Public Function getEntityNames(raw As vcXML.Reader,
                                   <RListObjectArgument>
                                   stream As Object,
                                   Optional env As Environment = Nothing) As Object

        Dim args As list = Internal.Invokes.base.Rlist(stream, env)
        Dim message As Message = checkStreamRef(args, env)

        If Not message Is Nothing Then
            Return message
        End If

        Dim moduleName$ = Nothing

        For Each name As String In {"transcriptome", "proteome", "metabolome"}
            If args.hasName(name) Then
                moduleName = name
                Exit For
            End If
        Next

        Dim contentType$ = CLRVector.asCharacter(args.getByName(moduleName)).GetValue(Scan0)
        Dim names As String() = raw.getStreamEntities(moduleName, contentType)

        Return names
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
    Public Function extractFrameMatrix(raw As String(),
                                       tick As Integer,
                                       <RListObjectArgument>
                                       stream As Object,
                                       Optional env As Environment = Nothing) As Object

        Dim args As list = Internal.Invokes.base.Rlist(stream, env)
        Dim message As Message = checkStreamRef(args, env)

        If Not message Is Nothing Then
            Return message
        End If

        Dim moduleName$ = Nothing

        For Each name As String In {"transcriptome", "proteome", "metabolome"}
            If args.hasName(name) Then
                moduleName = name
                Exit For
            End If
        Next

        Dim contentType$ = CLRVector.asCharacter(args.getByName(moduleName)).GetValue(Scan0)
        Dim matrix As New Dictionary(Of String, DataSet)

        For Each file As String In raw
            Using xml As New vcXML.Reader(file)
                Dim index As XmlOffset = xml _
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

        Return matrix.Values.ToArray
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
    <RApiReturn(GetType(HTS_Matrix))>
    <ExportAPI("time.frames")>
    Public Function timeFrames(raw As Object,
                               <RListObjectArgument>
                               Optional stream As Object = Nothing,
                               Optional env As Environment = Nothing) As Object

        Dim args As list = Internal.Invokes.base.Rlist(stream, env)

        If TypeOf raw Is vcXML.Reader Then
            Dim modu As String = Nothing
            Dim content_type As String = Nothing
            Dim message As Message = checkStreamRef(args, env)

            If Not message Is Nothing Then
                Return message
            End If

            For Each name As String In {"transcriptome", "proteome", "metabolome"}
                If args.hasName(name) Then
                    modu = name
                    content_type = args.getValue(Of String)(name, env)
                    Exit For
                End If
            Next

            Return DirectCast(raw, vcXML.Reader).GetTimeFrames(modu, content_type)
        ElseIf TypeOf raw Is Raw.Reader Then
            Dim read As Raw.Reader = DirectCast(raw, Raw.Reader).LoadIndex

            If args.hasName("module") Then
                Dim modu As String = args.getValue(Of String)("module", env)
                Dim m As HTS_Matrix = read.GetTimeFrames(modu)

                Return m
            Else
                ' export all molecules

            End If
        End If
    End Function
End Module
