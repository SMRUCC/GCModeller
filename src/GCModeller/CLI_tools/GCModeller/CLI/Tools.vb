#Region "Microsoft.VisualBasic::55085980b6a525b4ff1b76b4cae2d625, ..\CLI_tools\GCModeller\CLI\Tools.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data.Repository.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Strip.Null.Columns", Usage:="/Strip.Null.Columns /in <table.csv> [/out <out.csv>]")>
    Public Function StripNullColumns(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out As String = args.GetValue("/out", [in])
        Dim file As File = File.Load([in])
        Dim columns = file.Columns.Where(Function(c As String()) Not c.Skip(1).All(AddressOf StringEmpty)).JoinColumns
        Return file.Save(out).CLICode
    End Function

    <ExportAPI("/Located.AppData")>
    <Group(CLIGrouping.GCModellerAppTools)>
    Public Function LocatedAppData(args As CommandLine) As Integer
        Call System.Diagnostics.Process.Start(App.ProductSharedDIR.ParentPath)
        Return 0
    End Function

    ''' <summary>
    ''' 列举出所有的GCModeller的CLI工具命令
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--ls", Info:="Listing all of the available GCModeller CLI tools commands.")>
    <Group(CLIGrouping.GCModellerAppTools)>
    Public Function List(args As CommandLine) As Integer
        Dim execs As IEnumerable(Of String) =
            ls - l - wildcards("*.exe") <= App.HOME

        Dim types = LinqAPI.Exec(Of NamedValue(Of PackageAttribute)) <=
 _
            From exe As String
            In execs
            Let def As Type = GetCLIMod(exe)
            Where Not def Is Nothing
            Select New NamedValue(Of PackageAttribute) With {
                .Name = exe.BaseName,
                .Value = GetEntry(def)
            }

        Dim exeMAX As Integer = (From x In types Select Len(x.Name)).Max + 5

        Call Console.WriteLine("All of the available GCModeller commands were listed below.")
        Call Console.WriteLine()
        Call Console.WriteLine("For getting the available function in the GCModeller program, ")
        Call Console.WriteLine("try typing:    <command> ?")
        Call Console.WriteLine("For getting the manual document in the GCModeller program,")
        Call Console.WriteLine("try typing:    <command> man")
        Call Console.WriteLine(vbCrLf)
        Call Console.WriteLine("Listed {0} available GCModeller commands:", types.Length)
        Call Console.WriteLine()

        For Each x In types
            Dim exePrint As String = " " & x.Name & New String(" "c, exeMAX - Len(x.Name))
            Dim lines$() = Paragraph _
                .Split(x.Value.Description, 60) _
                .ToArray

            Console.WriteLine("{0}{1}", exePrint, lines.FirstOrDefault)

            If lines.Length > 1 Then
                Dim indent As New String(" "c, exeMAX + 1)

                For Each line$ In lines.Skip(1)
                    Console.WriteLine("{0}{1}", indent, line$)
                Next
            End If
        Next

        Return 0
    End Function

    <ExportAPI("/init.manuals", Usage:="/init.manuals")>
    <Group(CLIGrouping.GCModellerAppTools)>
    Public Function InitManuals(args As CommandLine) As Integer
        Dim execs As IEnumerable(Of String) = ls - l - wildcards("*.exe") <= App.HOME
        Dim tools As String() =
            LinqAPI.Exec(Of String) <= From exe As String
                                       In execs
                                       Let def As Type = GetCLIMod(exe)
                                       Where Not def Is Nothing
                                       Select exe
        For Each app As String In tools
            Call New IORedirectFile(app, "man --file").Run()

            Dim path As String = Microsoft.VisualBasic.App.HOME & "/" & app.BaseName & ".md"
            If Not path.FileExists Then
                Continue For
            End If

            Dim md As String = path.ReadAllText
            md = $"---
title: {app.BaseName}
tags: [maunal, tools]
date: {Now.ToString}
---
" & md
            Call md.SaveTo(path)
        Next

        Dim sb As New StringBuilder

        Call sb.AppendLine($"---
title: GCModeller CLI Tools
tags: [manual, tools]
date: {Now.ToString}
---")
        Call sb.AppendLine()
        Call sb.AppendLine("All of the available GCModeller CLI tools are listed below:")
        Call sb.AppendLine()
        Call sb.AppendLine()

        For Each app As String In tools
            Call sb.AppendLine($"+ [{app.BaseName}](./{app.BaseName}.html)")
        Next

        Call sb.SaveTo(App.HOME & "/index.md")

        Return 0
    End Function

    <ExportAPI("/Search.Fasta",
               Usage:="/Search.Fasta /in <fasta.fasta/DIR> /query <query_arguments.csv> [/out <out_DIR>]")>
    <Argument("/query", AcceptTypes:={GetType(QueryArgument)})>
    Public Function SearchFasta(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim query As String = args("/query")
        Dim out As String = args.GetValue("/out", query.TrimSuffix & "-" & [in].BaseName & "/")
        Dim arguments = query.LoadCsv(Of QueryArgument)

        Return StreamIterator.SeqSource([in],, debug:=True) _
            .BatchSearch(arguments.Select(
                Function(x) New NamedValue(Of String) With {
                    .Name = x.Name,
                    .Value = x.Expression
                }), out)
    End Function
End Module
