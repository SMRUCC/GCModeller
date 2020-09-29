#Region "Microsoft.VisualBasic::e4e1466a991c83017b36a993abac46fc, httpd\Program\DebuggerToolkit.vb"

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

' Module CLI
' 
'     Function: [GET], POST, StressTest
'     Structure __test
' 
'         Function: Run
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel.Threads
Imports Microsoft.VisualBasic.Text

Partial Module CLI

    <ExportAPI("/GET")>
    <Description("Tools for http get request the content of a specific url.")>
    <Usage("/GET /url <url, /std_in> [/out <file/std_out>]")>
    <Argument("/url", False, CLITypes.File, PipelineTypes.std_in,
          AcceptTypes:={GetType(String)},
          Description:="The resource URL on the web.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
          AcceptTypes:={GetType(String)},
          Description:="The save location of your requested data file.")>
    Public Function [GET](args As CommandLine) As Integer
        Dim url As String = args.ReadInput("/url")

        VBDebugger.ForceSTDError = True

        Using out As StreamWriter = args.OpenStreamOutput("/out")
            Dim html As String = url.GET
            Call out.Write(html)
        End Using

        Return 0
    End Function

    <ExportAPI("/POST")>
    <Usage("/POST /url <url, /std_in> [[/args1 value1 /args2 value2, ...] /out <file/std_out>]")>
    Public Function POST(args As CommandLine) As Integer
        Dim url$ = args.ReadInput("/url")
        Dim argv = args.ParameterList _
            .Where(Function(tuple)
                       With tuple.Name.ToLower
                           Return .ByRef <> "/url" AndAlso .ByRef <> "/out"
                       End With
                   End Function) _
            .ToDictionary _
            .FlatTable _
            .NameValueCollection

        VBDebugger.ForceSTDError = True

        Call argv.ToDictionary.GetJson.__DEBUG_ECHO

        Using out As StreamWriter = args.OpenStreamOutput("/out", Encodings.UTF8WithoutBOM)
            Call out.WriteLine(url.POST(argv, contentEncoding:=Encodings.UTF8))
        End Using

        Return 0
    End Function

    <ExportAPI("/Stress.Testing")>
    <Description("Using Ctrl + C to stop the stress testing.")>
    <Usage("/Stress.Testing /url <target_url> [/out <out.txt>]")>
    Public Function StressTest(args As CommandLine) As Integer
        Dim url$ = args <= "/url"
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/" & url.NormalizePathString & ".txt")
        Dim test As Func(Of Integer, String) = AddressOf New __test With {
            .url = url
        }.Run

        Using result As StreamWriter = out.OpenWriter
            Do While True
                Dim pack%() = SeqRandom(10000)
                Dim returns = BatchTasks.BatchTask(pack, getTask:=test, numThreads:=1000, TimeInterval:=0)
                For Each line In returns
                    Call result.WriteLine(line)
                Next

                Call result.WriteLine()
                Call result.WriteLine("==========================================================")
                Call result.WriteLine()
                Call result.Flush()
            Loop
        End Using

        Return 0
    End Function

    Private Structure __test
        Dim url$

        Public Function Run(n%) As String
            Try
                Dim request$ = url & "?random=" & UrlEncode(StrUtils.RandomASCIIString(len:=n))
                Dim response& = Time(Sub() Call request.GET)
                Return {"len=" & n, $"response={response}ms"}.JoinBy(ASCII.TAB)
            Catch ex As Exception
                Return "error"
            End Try
        End Function
    End Structure
End Module
