#Region "Microsoft.VisualBasic::2f53e5cbd8ca382c3977aca12b466fce, GCModeller\engine\vcellkit\RawXmlKit.vb"

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

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO
Imports SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports HTS_Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports REnv = SMRUCC.Rsharp.Runtime
Imports XmlOffset = SMRUCC.genomics.GCModeller.ModellingEngine.IO.vcXML.XML.offset

''' <summary>
''' 
''' </summary>
<Package("rawXML", Category:=APICategories.UtilityTools, Publisher:="gg.xie@bionovogene.com")>
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
    Public Function timeFrames(raw As vcXML.Reader, <RListObjectArgument> stream As Object, Optional env As Environment = Nothing) As Object
        Dim args As list = Internal.Invokes.base.Rlist(stream, env)
        Dim index As XmlOffset() = {}
        Dim message As Message = checkStreamRef(args, env)

        If Not message Is Nothing Then
            Return message
        End If

        For Each name As String In {"transcriptome", "proteome", "metabolome"}
            If args.hasName(name) Then
                ' get offset index for read data from raw data xml file
                index = raw.getStreamIndex(name)(args.getValue(Of String)(name, env)) _
                    .OrderBy(Function(p) p.id) _
                    .ToArray
                Exit For
            End If
        Next

        ' each row is feature item
        ' and the column value is the time stream data
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

        For Each offset As XmlOffset In index
            vector = raw.getFrameVector(offset.offset)

            For i As Integer = 0 To vector.Length - 1
                entities(i).Add(offset.id, vector(i))
            Next
        Next

        Dim timeTicks As String() = index _
            .Select(Function(o) o.id.ToString) _
            .ToArray
        Dim matrix As New HTS_Matrix With {
            .sampleID = timeTicks,
            .tag = raw.ToString,
            .expression = entities _
                .Select(Function(v)
                            Return New DataFrameRow With {
                                .geneID = v.ID,
                                .experiments = v(timeTicks)
                            }
                        End Function) _
                .ToArray
        }

        Return matrix
    End Function
End Module
