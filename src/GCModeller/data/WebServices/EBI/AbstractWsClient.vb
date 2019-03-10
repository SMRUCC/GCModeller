﻿#Region "Microsoft.VisualBasic::be1062c5d7d6a8fd84717b73f78349a2, data\WebServices\EBI\AbstractWsClient.vb"

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

    '     Class ClientException
    ' 
    '         Constructor: (+4 Overloads) Sub New
    ' 
    '     Class AbstractWsClient
    ' 
    '         Properties: Action, Async, DebugLevel, Email, JobId
    '                     JobTitle, MaxCheckInterval, OutFile, OutFormat, OutputLevel
    '                     ParamName, ServiceEndPoint
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: constuctUserAgentStr, LoadBinData, LoadData, NextIdentifier, NextSequence
    '                   ObjectFieldsToString, ObjectPropertiesToString, ObjectValueToString, ReadFile, ReadTextFile
    ' 
    '         Sub: ClientPoll, CloseIdentifierFile, CloseSequenceFile, (+2 Overloads) Dispose, (+2 Overloads) GetResults
    '              PrintDebugMessage, PrintGenericOptsUsage, PrintParams, PrintProgressMessage, PrintStatus
    '              SetIdentifierFile, SetSequenceFile, WriteBinaryFile, (+2 Overloads) WriteTextFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' $Id: AbstractWsClient.vb 2523 2013-02-06 13:51:28Z hpm $
' ======================================================================
' 
' Copyright 2011-2013 EMBL - European Bioinformatics Institute
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' 
' ======================================================================
' Common structure and methods for JDispatcher SOAP clients.
'
' See:
' http://www.ebi.ac.uk/Tools/webservices/
' http://www.ebi.ac.uk/Tools/webservices/tutorials/vb.net
' ======================================================================

Option Explicit On
Option Strict On

Imports Microsoft.VisualBasic.ControlChars ' Character constants (e.g. Tab).
Imports System
Imports System.Collections
Imports System.IO
Imports System.Reflection
Imports System.Text

Namespace EbiWS
    ' Generic exception for use in clients.
    <SerializableAttribute()>
    Public Class ClientException
        Inherits Exception
        ' Default constructor.
        Public Sub New()
            MyBase.New()
        End Sub

        ' Constructor.
        Public Sub New( message As String)
            MyBase.New(message)
        End Sub

        ' Constructor.
        Public Sub New( message As String,  inner As Exception)
            MyBase.New(message, inner)
        End Sub

        ' Constructor.
        Public Sub New(
             info As System.Runtime.Serialization.SerializationInfo,
             context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class

    ' Abstract definition of a client to the EMBL-EBI tool Web Services.
    Public MustInherit Class AbstractWsClient
        Implements IDisposable
        ' Output level
        Public Property OutputLevel() As Integer
            Get
                Return _outputLevel
            End Get
            Set( value As Integer)
                If value > -1 Then
                    _outputLevel = value
                End If
            End Set
        End Property
        Private _outputLevel As Integer = 1
        ' Debug output level.
        Public Property DebugLevel() As Integer
            Get
                Return _debugLevel
            End Get
            Set( value As Integer)
                If value > -1 Then
                    _debugLevel = value
                End If
            End Set
        End Property
        Private _debugLevel As Integer = 0
        ' Maximum interval between status checks when polling a submited job (ms).
        Public Property MaxCheckInterval() As Integer
            Get
                Return _maxCheckInterval
            End Get
            Set( value As Integer)
                If value > 5000 Then
                    _maxCheckInterval = value
                End If
            End Set
        End Property
        Private _maxCheckInterval As Integer = 60000
        ' Specified endpoint for the SOAP service. If null the default 
        ' endpoint specified in the WSDL (and thus in the generated 
        ' stubs) is used.
        Public Property ServiceEndPoint() As String
            Get
                Return _serviceEndPoint
            End Get
            Set( value As String)
                _serviceEndPoint = value
            End Set
        End Property
        Private _serviceEndPoint As String = Nothing
        ' Parameter name to be used to get parameter details.
        Public Property ParamName As String
            Get
                Return _paramName
            End Get
            Set( value As String)
                _paramName = value
            End Set
        End Property
        Private _paramName As String = Nothing
        ' Output file name base.
        Public Property OutFile() As String
            Get
                Return _outFile
            End Get
            Set( value As String)
                _outFile = value
            End Set
        End Property
        Private _outFile As String = Nothing
        ' Output result format.
        Public Property OutFormat() As String
            Get
                Return _outFormat
            End Get
            Set( value As String)
                _outFormat = value
            End Set
        End Property
        Private _outFormat As String = Nothing
        ' User e-mail address for job submissions.
        Public Property Email() As String
            Get
                Return _email
            End Get
            Set( value As String)
                _email = value
            End Set
        End Property
        Private _email As String = Nothing
        ' Title for job.
        Public Property JobTitle() As String
            Get
                Return _jobTitle
            End Get
            Set( value As String)
                _jobTitle = value
            End Set
        End Property
        Private _jobTitle As String = Nothing
        ' Job Id for getting status or results.
        Public Property JobId() As String
            Get
                Return _jobId
            End Get
            Set( value As String)
                _jobId = value
            End Set
        End Property
        Private _jobId As String = Nothing
        ' Submission mode: async or sync.
        Public Property Async() As Boolean
            Get
                Return _async
            End Get
            Set( value As Boolean)
                _async = value
            End Set
        End Property
        Private _async As Boolean = False
        ' Action to perform.
        Public Property Action As String
            Get
                Return _action
            End Get
            Set( value As String)
                _action = value
            End Set
        End Property
        Private _action As String = "unknown"
        ' Reader for fasta format sequence input data.
        Private sequenceFileReader As TextReader = Nothing
        ' Reader for entry identifier input data.
        Private identifierFileReader As TextReader = Nothing
        ' Usage message for generic options.
        Private genericOptsStr As String =
"[General]" & Environment.NewLine &
Environment.NewLine &
"      --params         :      : list tool parameters" & Environment.NewLine &
"      --paramDetail    : str  : information about a parameter" & Environment.NewLine &
"      --email          : str  : e-mail address" & Environment.NewLine &
"      --title          : str  : title for the job" & Environment.NewLine &
"      --async          :      : perform an asynchronous submission" & Environment.NewLine &
"      --jobid          : str  : job identifier" & Environment.NewLine &
"      --status         :      : get status of a job" & Environment.NewLine &
"      --resultTypes    :      : get list of result formats for a job" & Environment.NewLine &
"      --polljob        :      : get results for a job" & Environment.NewLine &
"      --outfile        : str  : name of the file results should be written to" & Environment.NewLine &
"                                (default is based on the jobid; ""-"" for STDOUT)" & Environment.NewLine &
"      --outformat      : str  : output format, see --resultTypes" & Environment.NewLine &
"      --help           :      : prints this help text" & Environment.NewLine &
"      --quiet          :      : decrease output" & Environment.NewLine &
"      --verbose        :      : increase output" & Environment.NewLine &
"      --debugLevel     : int  : set debug output level" & Environment.NewLine &
Environment.NewLine &
"Synchronous job:" & Environment.NewLine &
Environment.NewLine &
"  The results/errors are returned as soon as the job is finished." & Environment.NewLine &
"  Usage: tool.exe --email <your@email> [options...] seqFile" & Environment.NewLine &
"  Returns: results as an attachment" & Environment.NewLine &
Environment.NewLine &
"Asynchronous job:" & Environment.NewLine &
Environment.NewLine &
"  Use this if you want to retrieve the results at a later time. The results" & Environment.NewLine &
"  are stored for up to 24 hours." & Environment.NewLine &
"  Usage: tool.exe --async --email <your@email> [options...] seqFile" & Environment.NewLine &
"  Returns: jobid" & Environment.NewLine &
Environment.NewLine &
"  Use the jobid to query for the status of the job. If the job is finished," & Environment.NewLine &
"  it also returns the results/errors." & Environment.NewLine &
"  Usage: tool.exe --polljob --jobid <jobId> [--outfile string]" & Environment.NewLine &
"  Returns: string indicating the status of the job and if applicable, results" & Environment.NewLine &
"  as an attachment." & Environment.NewLine

        ' Default constructor
        Public Sub New()
            MyBase.New()
            OutputLevel = 1 ' Normal output
            DebugLevel = 0 ' Debug output off.
            MaxCheckInterval = 60000 ' 1 min between checks
            OutFile = Nothing
            OutFormat = Nothing
            Email = Nothing
            JobTitle = "My Sequence"
            JobId = Nothing
            Async = False
            Action = "UNKNOWN"
        End Sub

        ' Print the generic options usage message to STDOUT.
        Protected Sub PrintGenericOptsUsage()
            Console.WriteLine(genericOptsStr)
        End Sub

        ' Print a debug message at the specified level.
        Protected Sub PrintDebugMessage( methodName As String,  message As String,  level As Integer)
            If level <= DebugLevel Then
                Console.Error.WriteLine("[{0}()] {1}", methodName, message)
            End If
        End Sub

        ' Construct a string of the values of an object, both fields and properties.
        Protected Function ObjectValueToString( obj As Object) As String
            PrintDebugMessage("ObjectValueToString", "Begin", 31)
            Dim strBuilder As StringBuilder = New StringBuilder()
            If obj IsNot Nothing Then
                strBuilder.Append(ObjectFieldsToString(obj))
                strBuilder.Append(ObjectPropertiesToString(obj))
            End If
            PrintDebugMessage("ObjectValueToString", "End", 31)
            Return strBuilder.ToString()
        End Function

        ' Construct a string of the fields of an object.
        Protected Function ObjectFieldsToString( obj As Object) As String
            PrintDebugMessage("ObjectFieldsToString", "Begin", 32)
            Dim strBuilder As StringBuilder = New StringBuilder()
            Dim objType As Type = obj.GetType()
            PrintDebugMessage("ObjectFieldsToString", "objType: " & objType.ToString(), 33)
            For Each info As FieldInfo In objType.GetFields()
                PrintDebugMessage("ObjectFieldsToString", "info: " & info.Name & " (" & info.FieldType.FullName & ")", 33)
                If info.FieldType.IsArray Then
                    strBuilder.Append(info.Name & ":" & Environment.NewLine)
                    Dim subObjList As Object() = TryCast(info.GetValue(obj), Object())
                    For Each subObj As Object In subObjList
                        strBuilder.Append(Tab & subObj.ToString)
                    Next subObj
                Else
                    strBuilder.Append(info.Name & ": " & info.GetValue(obj).ToString & Environment.NewLine)
                End If
            Next info
            PrintDebugMessage("ObjectFieldsToString", "End", 32)
            Return strBuilder.ToString()
        End Function

        ' Construct a string of the properties of an object.
        Protected Function ObjectPropertiesToString( obj As Object) As String
            PrintDebugMessage("ObjectPropertiesToString", "Begin", 31)
            Dim strBuilder As StringBuilder = New StringBuilder()
            If obj Is Nothing Then
                Return "<null>"
            End If
            Dim objType As Type = obj.GetType()
            If objType Is Nothing Then
                Return "unknown"
            End If
            PrintDebugMessage("ObjectPropertiesToString", "objType: " & objType.ToString(), 32)
            For Each info As PropertyInfo In objType.GetProperties()
                PrintDebugMessage("ObjectPropertiesToString", "info: " & info.Name & " (" & info.PropertyType.FullName & ")", 32)
                If info.PropertyType.IsArray Then
                    Dim objArray As IList = TryCast(info.GetValue(obj, Nothing), IList)
                    If objArray IsNot Nothing AndAlso objArray.Count > 0 Then
                        PrintDebugMessage("ObjectPropertiesToString", "Array: " & objArray.Count, 33)
                        strBuilder.Append(info.Name & ":" & Environment.NewLine)
                        For Each subObj As Object In objArray
                            If subObj IsNot Nothing Then
                                strBuilder.Append(Tab & subObj.ToString)
                            End If
                        Next subObj
                    Else
                        strBuilder.Append(info.Name & ": <null>" & Environment.NewLine)
                    End If
                Else
                    PrintDebugMessage("ObjectPropertiesToString", "Object: " & obj.ToString, 33)
                    strBuilder.Append(info.Name & ": ")
                    If info.GetValue(obj, Nothing) IsNot Nothing Then
                        strBuilder.Append(info.GetValue(obj, Nothing).ToString)
                    Else
                        strBuilder.Append("<null>")
                    End If
                    strBuilder.Append(Environment.NewLine)
                End If
                PrintDebugMessage("ObjectPropertiesToString", strBuilder.ToString(), 33)
            Next info
            PrintDebugMessage("ObjectPropertiesToString", "End", 31)
            Return strBuilder.ToString()
        End Function

        ' Print a progress message, at the specified output level.
        Protected Sub PrintProgressMessage( msg As String,  level As Integer)
            If OutputLevel >= level Then
                Console.Error.WriteLine(msg)
            End If
        End Sub

        ' Construct a User-agent string.
        Protected Function constuctUserAgentStr( revision As String,  clientClassName As String,  userAgent As String) As String
            PrintDebugMessage("constuctUserAgentStr", "Begin", 31)
            Dim retUserAgent As String = "EBI-Sample-Client"
            Dim clientVersion As String = "0"
            ' Client version.
            If revision IsNot Nothing AndAlso revision.Length > 0 Then
                ' CVS/Subversion revision tag.
                If revision.StartsWith("$") Then
                    ' Populated tag, extract revision number.
                    If revision.Length > 13 Then
                        clientVersion = revision.Substring(11, (revision.Length - 13))
                    End If
                    ' Alternative revision/version string.
                Else
                    clientVersion = revision
                End If
            End If
            Dim strBuilder As StringBuilder = New StringBuilder()
            strBuilder.Append(retUserAgent & "/" & clientVersion)
            strBuilder.Append(" (" & clientClassName & "; VB.NET; " & Environment.OSVersion.ToString)
            If userAgent Is Nothing OrElse userAgent.Length < 1 Then ' No agent
                strBuilder.Append(")")
            ElseIf userAgent.Contains("(") Then ' MS .NET
                strBuilder.Append(") " & userAgent)
            Else ' Mono
                strBuilder.Append("; " & userAgent & ")")
            End If
            retUserAgent = strBuilder.ToString
            PrintDebugMessage("constuctUserAgentStr", "retUserAgent: " & retUserAgent, 32)
            PrintDebugMessage("constuctUserAgentStr", "End", 31)
            Return retUserAgent
        End Function

        ' Read data from a text file into a string.
        Protected Function ReadTextFile( fileName As String) As String
            PrintDebugMessage("ReadTextFile", "Begin", 1)
            If fileName Is Nothing OrElse fileName.Length < 1 Then
                Throw New ClientException("A file name is required to read data from.")
            End If
            PrintDebugMessage("ReadTextFile", "fileName: " & fileName, 2)
            Dim retVal As String = ""
            ' Read from STDIN
            If fileName = "-" Then
                retVal = Console.In.ReadToEnd()
                ' Read from file
            Else
                retVal = File.ReadAllText(fileName)
            End If
            PrintDebugMessage("ReadTextFile", "read " & retVal.Length & " characters", 1)
            PrintDebugMessage("ReadTextFile", "End", 1)
            Return retVal
        End Function

        ' Read a file into a byte array.
        Protected Function ReadFile( fileName As String) As Byte()
            PrintDebugMessage("ReadFile", "Begin", 1)
            If fileName Is Nothing OrElse fileName.Length < 1 Then
                Throw New ClientException("A file name is required to read data from.")
            End If
            PrintDebugMessage("ReadFile", "fileName: " & fileName, 1)
            Dim retVal As Byte() = Nothing
            If fileName = "-" Then ' Read from STDIN
                Dim s As Stream = Console.OpenStandardInput()
                Dim sr As BinaryReader = New BinaryReader(s)
                retVal = sr.ReadBytes(CInt(s.Length))
                ' Note: do not close since this is STDIN.
            Else ' Read from file
                retVal = File.ReadAllBytes(fileName)
            End If
            PrintDebugMessage("ReadFile", "read " & retVal.Length & " bytes", 1)
            PrintDebugMessage("ReadFile", "End", 1)
            Return retVal
        End Function

        ' Load text data to be submitted to the tool.
        Protected Function LoadData( fileOptionStr As String) As String
            PrintDebugMessage("LoadData", "Begin", 1)
            If fileOptionStr Is Nothing OrElse fileOptionStr.Length < 1 Then
                Throw New ClientException("A file name is required to read data from.")
            End If
            PrintDebugMessage("LoadData", "fileOptionStr: " & fileOptionStr, 2)
            Dim retVal As String = Nothing
            If fileOptionStr IsNot Nothing Then
                If fileOptionStr = "-" Then ' STDIN
                    retVal = ReadTextFile(fileOptionStr)
                ElseIf File.Exists(fileOptionStr) Then ' File
                    retVal = ReadTextFile(fileOptionStr)
                Else ' Entry Id or raw sequence
                    retVal = fileOptionStr
                End If
            End If
            PrintDebugMessage("LoadData", "End", 1)
            Return retVal
        End Function

        ' Load binary data for submission to the tool service.
        Protected Function LoadBinData( fileOptionStr As String) As Byte()
            PrintDebugMessage("LoadBinData", "Begin", 1)
            If fileOptionStr Is Nothing OrElse fileOptionStr.Length < 1 Then
                Throw New ClientException("A file name is required to read data from.")
            End If
            PrintDebugMessage("LoadBinData", "fileOptionStr: " & fileOptionStr, 2)
            Dim retVal As Byte() = Nothing
            If fileOptionStr IsNot Nothing Then
                If fileOptionStr = "-" Then ' STDIN
                    retVal = ReadFile(fileOptionStr)
                ElseIf File.Exists(fileOptionStr) Then ' File
                    retVal = ReadFile(fileOptionStr)
                Else ' Entry Id or raw sequence
                    Dim enc As System.Text.ASCIIEncoding = New System.Text.ASCIIEncoding()
                    retVal = enc.GetBytes(fileOptionStr)
                End If
            End If
            PrintDebugMessage("LoadBinData", "End", 1)
            Return retVal
        End Function

        ' Write a byte array to a file.
        Protected Sub WriteBinaryFile( fileName As String,  content As Byte())
            PrintDebugMessage("WriteBinaryFile", "Begin", 1)
            If fileName Is Nothing OrElse fileName.Length < 1 Then
                Throw New ClientException("A file name is required to write data to.")
            End If
            PrintDebugMessage("WriteBinaryFile", "fileName: " & fileName, 1)
            PrintDebugMessage("WriteBinaryFile", "content: " & content.Length & " bytes", 1)
            If fileName = "-" Then ' STDOUT
                Dim s As Stream = Console.OpenStandardOutput()
                Dim sw As BinaryWriter = New BinaryWriter(s)
                sw.Write(content)
                ' Note: do not close, since this is STDOUT.
            Else ' Data file
                File.WriteAllBytes(fileName, content)
                Console.WriteLine("Wrote: {0}", fileName)
            End If
            PrintDebugMessage("WriteBinaryFile", "End", 1)
        End Sub

        ' Write text data encoded as a byte array to a file.
        Protected Sub WriteTextFile( fileName As String,  content As Byte())
            PrintDebugMessage("WriteTextFile", "Begin", 1)
            If fileName Is Nothing OrElse fileName.Length < 1 Then
                Throw New ClientException("A file name is required to write data to.")
            End If
            PrintDebugMessage("WriteTextFile", "fileName: " & fileName, 1)
            PrintDebugMessage("WriteTextFile", "content: " & content.Length & " bytes", 1)
            Dim enc As System.Text.ASCIIEncoding = New System.Text.ASCIIEncoding()
            Dim contentStr As String = enc.GetString(content)
            WriteTextFile(fileName, contentStr)
            PrintDebugMessage("WriteTextFile", "End", 1)
        End Sub

        ' Write a string to a file.
        Protected Sub WriteTextFile( fileName As String,  content As String)
            PrintDebugMessage("WriteTextFile", "Begin", 1)
            If fileName Is Nothing OrElse fileName.Length < 1 Then
                Throw New ClientException("A file name is required to write data to.")
            End If
            PrintDebugMessage("WriteTextFile", "fileName: " & fileName, 1)
            PrintDebugMessage("WriteTextFile", "content: " & content.Length & " characters", 1)
            If fileName = "-" Then ' STDOUT
                Console.Write(content)
            Else ' Data file
                File.WriteAllText(fileName, content)
                Console.WriteLine("Wrote: {0}", fileName)
            End If
            PrintDebugMessage("WriteTextFile", "End", 1)
        End Sub

        ' Set file to read sequence data from in multi-sequence mode. 
        Protected Sub SetSequenceFile( fileName As String)
            PrintDebugMessage("SetSequenceFile", "Begin", 11)
            If fileName Is Nothing OrElse fileName.Length < 1 Then
                Throw New ClientException("A file name is required to read sequences from.")
            End If
            PrintDebugMessage("SetSequenceFile", "fileName: " & fileName, 12)
            If fileName = "-" Then ' STDIN.
                Me.sequenceFileReader = Console.In
            Else ' Data file.
                Me.sequenceFileReader = New StreamReader(fileName)
            End If
            PrintDebugMessage("SetSequenceFile", "End", 11)
        End Sub

        ' Read next sequence from sequence data file. Assumes fasta 
        ' formatted sequence data. File to read from is set by 
        ' SetSequenceFile(fileName).
        Protected Function NextSequence() As String
            PrintDebugMessage("NextSequence", "Begin", 11)
            If Me.sequenceFileReader Is Nothing Then
                Throw New ClientException("Sequence data file to read from not set.")
            End If
            Dim retVal As String = Nothing
            Dim line As String = Nothing
            ' Skip to start of fasta sequence.
            While Microsoft.VisualBasic.ChrW(sequenceFileReader.Peek()) <> ">"c
                line = Me.sequenceFileReader.ReadLine()
                PrintDebugMessage("NextSequence", "skip line: " & line, 12)
                If line Is Nothing Then
                    Exit While
                End If
            End While
            If Microsoft.VisualBasic.ChrW(sequenceFileReader.Peek()) = ">"c Then
                line = Me.sequenceFileReader.ReadLine()
                PrintProgressMessage(line, 1)
                retVal = line & Environment.NewLine
                While Microsoft.VisualBasic.ChrW(sequenceFileReader.Peek()) <> ">"c
                    line = Me.sequenceFileReader.ReadLine()
                    If line Is Nothing Then
                        Exit While
                    End If
                    PrintDebugMessage("NextSequence", "line: " & line, 12)
                    retVal &= line & Environment.NewLine
                End While
            End If
            PrintDebugMessage("NextSequence", "retVal:" & Environment.NewLine & retVal, 12)
            PrintDebugMessage("NextSequence", "End", 11)
            Return retVal
        End Function

        ' Close the stream used to read from the sequence data file in 
        ' multi-sequence mode.
        Protected Sub CloseSequenceFile()
            PrintDebugMessage("CloseSequenceFile", "Begin", 11)
            If Me.sequenceFileReader IsNot Nothing AndAlso Me.sequenceFileReader IsNot Console.In Then
                Me.sequenceFileReader.Close()
            End If
            Me.sequenceFileReader = Nothing
            PrintDebugMessage("CloseSequenceFile", "End", 11)
        End Sub

        ' Set the input file to read a list of sequence entry identifiers 
        ' from.
        Protected Sub SetIdentifierFile( fileName As String)
            PrintDebugMessage("SetIdentifierFile", "Begin", 11)
            If fileName Is Nothing OrElse fileName.Length < 1 Then
                Throw New ClientException("A file name is required to read identifiers from.")
            End If
            PrintDebugMessage("SetIdentifierFile", "fileName: " & fileName, 12)
            If fileName = "-" Then ' STDIN.
                Me.identifierFileReader = Console.In
            Else ' Data file.
                Me.identifierFileReader = New StreamReader(fileName)
            End If
            PrintDebugMessage("SetIdentifierFile", "End", 11)
        End Sub

        ' Read the next identifier from the identifier list file. The 
        ' identifier list file is set using SetIdentifierFile(fileName).
        Protected Function NextIdentifier() As String
            PrintDebugMessage("NextIdentifier", "Begin", 11)
            If Me.identifierFileReader Is Nothing Then
                Throw New ClientException("Identifier list file to read from not set.")
            End If
            Dim retVal As String = Nothing
            Dim line As String = Nothing
            line = Me.identifierFileReader.ReadLine()
            While line IsNot Nothing
                PrintProgressMessage(line, 1)
                If line.Contains(":") Then
                    retVal = line.Trim()
                    Exit While
                End If
                line = Me.identifierFileReader.ReadLine()
            End While
            PrintDebugMessage("NextIdentifier", "retVal: " & retVal, 12)
            PrintDebugMessage("NextIdentifier", "End", 11)
            Return retVal
        End Function

        ' Close the stream used to read from the identifier list file.
        Protected Sub CloseIdentifierFile()
            PrintDebugMessage("CloseIdentifierFile", "Begin", 11)
            If Me.identifierFileReader IsNot Nothing AndAlso Me.identifierFileReader IsNot Console.In Then
                Me.identifierFileReader.Close()
            End If
            Me.identifierFileReader = Nothing
            PrintDebugMessage("CloseIdentifierFile", "End", 11)
        End Sub

        ' Get the service connection. Has to be called before attempting to use any of the service operations.
        Protected MustOverride Sub ServiceProxyConnect()

        ' Get list of input parameter names from sevice.
        Public MustOverride Function GetParams() As String()

        ' Print a list of input parameter names.
        Protected Sub PrintParams()
            PrintDebugMessage("PrintParams", "Begin", 1)
            Dim paramNameList As String() = GetParams()
            If paramNameList Is Nothing Then
                Throw New ClientException("Null returned by web service.")
            End If
            For Each paramName As String In paramNameList
                Console.WriteLine(paramName)
            Next paramName
            PrintDebugMessage("PrintParams", "End", 1)
        End Sub

        ' Print a detailed description of a specified input parameter.
        Protected MustOverride Sub PrintParamDetail( paramName As String)

        ' Submit a job using the current client state.
        Public MustOverride Sub SubmitJob()

        ' Get the status of a submitted job.
        Public MustOverride Function GetStatus( jobId As String) As String

        ' Print the status of the current job.
        Public Sub PrintStatus()
            PrintDebugMessage("PrintStatus", "Begin", 1)
            If Me.JobId Is Nothing OrElse Me.JobId.Length < 1 Then
                Throw New ClientException("Job identifier is required to get the job status.")
            End If
            Dim status As String = GetStatus(JobId)
            Console.WriteLine(status)
            PrintDebugMessage("PrintStatus", "End", 1)
        End Sub

        ' Wait for a job to finish.
        Public Sub ClientPoll( jobId As String)
            PrintDebugMessage("ClientPoll", "Begin", 1)
            If jobId Is Nothing OrElse jobId.Length < 1 Then
                Throw New ClientException("Job identifier is required to poll job status.")
            End If
            PrintDebugMessage("ClientPoll", "jobId: " & jobId, 2)
            Dim checkInterval As Integer = 1000
            Dim status As String = "PENDING"
            ' Check status and wait if not finished
            While status = "RUNNING" OrElse status = "PENDING"
                status = GetStatus(jobId)
                PrintProgressMessage(status, 1)
                If status = "RUNNING" OrElse status = "PENDING" Then
                    ' Wait before polling again.
                    PrintDebugMessage("clientPoll", "checkInterval: " & checkInterval, 2)
                    System.Threading.Thread.Sleep(checkInterval)
                    checkInterval *= 2
                    If checkInterval > MaxCheckInterval Then
                        checkInterval = MaxCheckInterval
                    End If
                End If
            End While
            PrintDebugMessage("ClientPoll", "End", 1)
        End Sub

        ' Print a list of the available result types for the current job.
        Public MustOverride Sub PrintResultTypes()

        ' Get results for a job, of the specified type.
        Public MustOverride Function GetResult( jobId As String,  format As String) As Byte()

        ' Get results for a job, of the specified type.
        Public MustOverride Sub GetResults( jobId As String,  outformat As String,  outFileBase As String)

        ' Get results for a job using the current format and output file.
        Public Sub GetResults( jobId As String)
            PrintDebugMessage("GetResults", "Begin", 1)
            If jobId Is Nothing OrElse jobId.Length < 1 Then
                Throw New ClientException("Job identifier is required to get results.")
            End If
            GetResults(jobId, OutFormat, OutFile)
            PrintDebugMessage("GetResults", "End", 1)
        End Sub

        ' Get results for the current job.
        Public Sub GetResults()
            PrintDebugMessage("GetResults", "Begin", 1)
            If Me.JobId Is Nothing OrElse Me.JobId.Length < 1 Then
                Throw New ClientException("Job identifier is required to get results.")
            End If
            GetResults(JobId, OutFormat, OutFile)
            PrintDebugMessage("GetResults", "End", 1)
        End Sub

        Protected Overridable Overloads Sub Dispose(disposing As Boolean)
            If disposing Then
                Me.sequenceFileReader.Close()
                Me.identifierFileReader.Close()
            End If
        End Sub 'Dispose

        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub 'Dispose

    End Class
End Namespace
