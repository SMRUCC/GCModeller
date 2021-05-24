#Region "Microsoft.VisualBasic::6021ecb5238c698252dcd8657cdab0db, CLI_tools\GCModeller\CLI\Repository.vb"

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
    '     Function: CopyFiles, Install_NCBI_nt, InstallCDD, InstallCOGs, InstallGenbank
    '               MatchHits, ntRepositoryExports, NtScaner, UniqueTitle
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Assembly.NCBI.CDD
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Repository.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Install.genbank", Usage:="/Install.genbank /imports <all_genbanks.DIR> [/refresh]")>
    <Group(CLIGrouping.RepositoryTools)>
    Public Function InstallGenbank(args As CommandLine) As Integer
        Dim [in] As String = args - "/imports"
        Return Installer.Install([in], args.GetBoolean("/refresh")).CLICode
    End Function

    '<ExportAPI("-imports", Usage:="-imports <genbank_file/genbank_directory>")>
    'Public Function [Imports](argvs As CommandLine) As Integer
    '    Return RQL.API.ImportsGBK(argvs.Parameters.First).CLICode
    'End Function

    <ExportAPI("--install-COGs",
               Info:="Install the COGs database into the GCModeller database.",
               Usage:="--install-COGs /COGs <Dir.COGs>")>
    <Group(CLIGrouping.RepositoryTools)>
    Public Function InstallCOGs(args As CommandLine) As Integer
        Dim COGsDir As String = args("/COGs")
        Dim protFasta As String = FileIO.FileSystem.GetFiles(COGsDir,
                                                             FileIO.SearchOption.SearchAllSubDirectories,
                                                             "*.fasta", "*.fa", "*.fsa").FirstOrDefault
        If String.IsNullOrEmpty(protFasta) Then
            Call $"Could not found any fasta file from directory {COGsDir}".__DEBUG_ECHO
            Return -1
        Else
            Call $"{protFasta.ToFileURL} was found!".__DEBUG_ECHO
        End If

        If String.IsNullOrEmpty(GCModeller.FileSystem.RepositoryRoot) Then
            Settings.SettingsFile.RepositoryRoot = App.HOME & "/Repository/"
        End If

        COGsDir = GCModeller.FileSystem.RepositoryRoot & "/COGs/bbh/"
        Call $"COGs fasta database will be installed at location {COGsDir}".__DEBUG_ECHO
        Return SMRUCC.genomics.Assembly.NCBI.COG.COGs.SaveRelease(protFasta, COGsDir).CLICode
    End Function

    <ExportAPI("--install-CDD", Usage:="--install-CDD /cdd <cdd.DIR>")>
    <Group(CLIGrouping.RepositoryTools)>
    Public Function InstallCDD(args As CommandLine) As Integer
        Dim Repository As String = GCModeller.FileSystem.RepositoryRoot
        Repository &= "/CDD/"
        Dim buildFrom As String = args("/cdd")
        Call DbFile.BuildDb(buildFrom, Repository)
        Return True
    End Function

    '<ExportAPI("--install.ncbi_nt", Usage:="--install.ncbi_nt /nt <nt.fasta/DIR> [/EXPORT <DATA_dir>]")>
    '<Group(CLIGrouping.RepositoryTools)>
    'Public Function Install_NCBI_nt(args As CommandLine) As Integer
    '    Dim nt As String = args("/nt")
    '    Dim EXPORT$ = args.GetValue(
    '        "/EXPORT",
    '        If(nt.FileExists, nt.TrimSuffix, nt.TrimDIR) & "-$DATA/")
    '    Dim mysql As MySqli = Nothing

    '    Call mysql.[Imports](nt, EXPORT, False)

    '    Return 0
    'End Function

    <ExportAPI("/nt.repository.query", Usage:="/nt.repository.query /query <arguments.csv> /DATA <DATA_dir> [/out <out_DIR>]")>
    <ArgumentAttribute("/query", AcceptTypes:={GetType(QueryArgument)})>
    <Group(CLIGrouping.RepositoryTools)>
    Public Function ntRepositoryExports(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim DATA As String = args("/DATA")
        Dim out As String = args.GetValue("/out", query.TrimSuffix & "-" & DATA.BaseName & "-EXPORT/")
        Dim repo As New QueryEngine

        Call repo.ScanSeqDatabase(DATA)

        For Each x In query.LoadCsv(Of QueryArgument)
            Dim path$ = $"{out}/{x.Name.NormalizePathString}.fasta"

            Call path.__DEBUG_ECHO

            Using fasta As StreamWriter = path.OpenWriter(Encodings.ASCII)
                For Each result As FastaSeq In repo.Search(query:=x.Expression$)
                    Call fasta.WriteLine(result.GenerateDocument(120))
                    Call Console.Write(".")
                Next
            End Using
        Next

        Return 0
    End Function

    <ExportAPI("/nt.scan",
               Usage:="/nt.scan /query <expression.csv> /DATA <nt.DIR> [/break 60 /out <out_DIR>]")>
    <Group(CLIGrouping.RepositoryTools)>
    Public Function NtScaner(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim DATA As String = args("/DATA")
        Dim out As String = args.GetValue("/out", query.TrimSuffix & "_" & DATA.BaseName & ".csv")
        Dim expressions = query.LoadCsv(Of QueryArgument)
        Dim exp = expressions _
            .Select(Function(x) New NamedValue(Of Expression) With {
                .Name = x.Name,
                .Value = x.Expression.Build
            }).ToDictionary
        Dim break% = args.GetValue("/break", 60)

        Call QueryEngine.ScanDatabase(
            DATA,
            query:=exp,
            EXPORT:=out,
            lineBreak:=break)

        Return 0
    End Function

    <ExportAPI("/Name.match.hits", Usage:="/Name.match.hits /in <list.csv> /titles <*.txt/DIR> [/out <out.csv>]")>
    <Group(CLIGrouping.RepositoryTools)>
    Public Function MatchHits(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim titles As String = args("/titles")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & titles.BaseName & ".csv")
        Dim file As IO.File = IO.File.Load([in])
        Dim list As New List(Of String)

        If titles.FileExists Then
            list += titles.IterateAllLines
        Else
            For Each path$ In ls - l - r - wildcards("*.txt") <= titles
                list += path.IterateAllLines
            Next
        End If

        For Each line In file
            Dim name$ = line.First
            Dim count = list _
                .Where(Function(s) InStr(s, name, CompareMethod.Text) > 0) _
                .Count
            line(1) = CStr(count)
        Next

        Return file.Save(out, Encodings.UTF8).CLICode
    End Function

    <ExportAPI("/Data.Copy", Usage:="/Data.Copy /imports <DIR> [/ext <*.*> /copy2 <CopyTo> /overrides]")>
    Public Function CopyFiles(args As CommandLine) As Integer
        Dim importsData$ = args("/imports")
        Dim ext As String = args.GetValue("/ext", "*.*")
        Dim copy2 As String = args("/copy2") Or (importsData.TrimDIR & "-copy2/")
        Dim [overrides] As Boolean = args.GetBoolean("/overrides")

        For Each file$ In ls - l - r - ext <= importsData
            Dim path$ = file$

            If [overrides] Then
                path = copy2 & $"/{path.ParentDirName}-{path.FileName}"
            Else
                path = copy2 & "/" & path.FileName
            End If

            If Not SafeCopyTo(file, path) Then
                Call file.PrintException
            End If
        Next

        Return 0
    End Function

    <ExportAPI("/title.uniques", Usage:="/title.uniques /in <*.txt/DIR> [/simple /tokens 3 /n -1 /out <out.csv>]")>
    <Group(CLIGrouping.RepositoryTools)>
    Public Function UniqueTitle(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String =
            args.GetValue("/out", [in].ParentPath & "/" & [in].BaseName & ".unique_titles.txt")
        Dim list As New List(Of String)
        Dim tokens As Integer = args.GetValue("/tokens", 3)
        Dim simple As Boolean = args.GetBoolean("/simple")

        If [in].FileExists Then
            list += [in].ReadAllLines.Select(Function(s) s.GetTagValue(vbTab).Value)
        Else
            For Each file$ In ls - l - r - wildcards("*.txt") <= [in]
                list += file$ _
                    .ReadAllLines _
                    .Select(Function(s) s.GetTagValue(vbTab).Value)
                Call file.__DEBUG_ECHO
            Next
        End If

        Dim words As New Dictionary(Of String, Integer)
        Dim word As New Value(Of String)
        Dim data As New List(Of ClusterEntity)

        Call words.Add(" ", 0)

        For i As Integer = 0 To list.Count - 1
            Dim s$ = Regex.Replace(list(i), "\s{2,}", " ")

            Do While Not String.IsNullOrEmpty(
                word = Regex.Match(s, "^\d+\S*\s", RegexOptions.Multiline).Value)
                s = s.Replace(+word, "")
            Loop

            Do While Not String.IsNullOrEmpty(
                word = Regex.Match(s, "^\S+?:\s", RegexOptions.Multiline).Value)
                s = s.Replace(+word, "")
            Loop

            Do While Not String.IsNullOrEmpty(
                word = Regex.Match(s, "^\S+?\d+\s", RegexOptions.Multiline).Value)
                s = s.Replace(+word, "")
            Loop

            Do While Not String.IsNullOrEmpty(
                word = Regex.Match(s, "^\[\S+\]\s", RegexOptions.Multiline).Value)
                s = s.Replace(+word, "")
            Loop

            If InStr(s, "(") = 1 OrElse InStr(s, "{") = 1 OrElse InStr(s, "[") = 1 Then
                s = Mid(s, 2)
            End If

            Dim t$() = s.Split.Take(tokens).ToArray
            Dim p As New List(Of Double)

            For Each str As String In t$
                If Not words.ContainsKey(word = str.ToLower) Then
                    Call words.Add(+word, words.Count)
                End If

                p += CDbl(words(+word))
            Next

            If p.Count < tokens Then
                p += (tokens - p.Count) _
                    .Sequence _
                    .Select(Function(o) 0R)
            End If

            data += New ClusterEntity With {
                .uid = s$,
                .entityVector = p
            }
        Next

        Call (From x In data Select x.entityVector.Length Distinct) _
            .ToArray _
            .GetJson _
            .__DEBUG_ECHO

        Dim int2Words = words _
            .ToDictionary(Function(x) x.Value,
                          Function(x) x.Key)

        If simple Then
            Dim LQuery = From x
                         In data
                         Select x.uid,
                             u = String.Join("-", x.entityVector.Select(Function(o) CStr(o)))
                         Group By u Into Group

            'Return LQuery.ToArray(
            '    Function(x) New NamedValue(Of String()) With {
            '        .Name = x.u _
            '            .Split("-"c) _
            '            .Select(Function(o) int2Words(CInt(o))) _
            '            .JoinBy(" "),
            '        .x = x.Group _
            '            .Select(Function(o) o.uid)
            '    }).GetJson _
            '      .SaveTo(out) _
            '      .CLICode
            Return LQuery.Select(
                Function(x) x.u _
                    .Split("-"c) _
                    .Select(Function(o) int2Words(CInt(o))) _
                    .JoinBy(" ")
                ).OrderBy(Function(s) s) _
                .FlushAllLines(out) _
                .CLICode
        End If

        Dim n As Integer = args.GetValue("/n", data.Count * 0.1)
        Dim cl As ClusterCollection(Of ClusterEntity) = data.ClusterDataSet(n)
        Dim output As New List(Of NamedValue(Of String()))

        For Each cluster As KMeansCluster(Of ClusterEntity) In cl
            Dim common$() = cluster _
                .ClusterMean _
                .Select(Function(x) int2Words(CInt(x)))

            output += New NamedValue(Of String()) With {
                .Name = String.Join(" ", common),
                .Value = cluster.Select(Function(x) x.uid)
            }
        Next

        Return output.GetJson.SaveTo(out).CLICode
    End Function
End Module
