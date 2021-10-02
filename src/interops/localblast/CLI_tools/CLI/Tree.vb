#Region "Microsoft.VisualBasic::151a78f8c2305adc1c0c30df4ca4837d, localblast\CLI_tools\CLI\Tree.vb"

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
    '     Function: ExportFasta, IdentitiesMAT
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("--Export.Fasta")>
    <Usage("--Export.Fasta /hits <query-hits.csv> /query <query.fasta> /subject <subject.fasta>")>
    Public Function ExportFasta(args As CommandLine) As Integer
        Dim hist = args("/hits").LoadCsv(Of BBH.BestHit)
        Dim query = New FastaFile(args("/query")).ToDictionary(Function(x) x.Title.Split.First)
        Dim subject = New FastaFile(args("/subject")).ToDictionary(Function(x) x.Title.Split.First)
        Dim AllLocus As String() = hist.Select(Function(x) x.QueryName).Join(hist.Select(Function(x) x.HitName)).Distinct.ToArray
        Dim GetFasta = LinqAPI.MakeList(Of FastaSeq) <= From id As String In AllLocus Where query.ContainsKey(id) Select query(id)

        GetFasta += From id As String
                    In AllLocus
                    Where subject.ContainsKey(id)
                    Select subject(id)

        Dim out As String = args("/hits").TrimSuffix & ".fasta"
        Return New FastaFile(GetFasta).Save(out).CLICode
    End Function

    <ExportAPI("/Identities.Matrix")>
    <Usage("/Identities.Matrix /hit <sbh/bbh.csv> [/out <out.csv> /cut 0.65]")>
    Public Function IdentitiesMAT(args As CommandLine) As Integer
        Dim hit As String = args("/hit")
        Dim cut As Double = args.GetValue("/cut", 0.65)
        Dim out As String = args.GetValue("/out", hit.TrimSuffix & $"_cut={cut}.csv")
        Dim hits = hit.LoadCsv(Of BBHIndex)
        Dim Grep As TextGrepMethod = TextGrepScriptEngine _
            .Compile("tokens ' ' first") _
            .PipelinePointer

        For Each x As BBHIndex In hits
            x.QueryName = Grep(x.QueryName)
            x.HitName = Grep(x.HitName)
        Next

        Dim Groups = (From x As BBHIndex
                      In hits.AsParallel
                      Select x
                      Group x By x.QueryName Into Group) _
                           .ToDictionary(Function(x) x.QueryName,
                                         Function(x) (From n As BBHIndex
                                                      In x.Group
                                                      Select n
                                                      Group n By n.HitName Into Group) _
                                                           .ToDictionary(Function(xx) xx.HitName,
                                                                         Function(xx) xx.Group.First))
        Dim allKeys As String() = Groups.Keys.ToArray
        Dim MAT As File = New File + "locus".Join(allKeys)

        For Each query In Groups
            Dim row As New RowObject(query.Key)
            Dim hash As Dictionary(Of String, BBHIndex) = query.Value

            For Each key As String In allKeys
                If hash.ContainsKey(key) Then
                    row += CStr(hash(key).identities)
                Else
                    row += "0"
                End If
            Next

            MAT += row
        Next

        Return MAT > out
    End Function
End Module
