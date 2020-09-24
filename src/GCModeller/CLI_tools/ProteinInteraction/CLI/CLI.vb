#Region "Microsoft.VisualBasic::daf4d521ccbbcbb22c7a584b0704165d, CLI_tools\ProteinInteraction\CLI\CLI.vb"

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
    '     Function: BioGridIdTypes, BioGRIDSelects, TCSParser
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.BioGRID
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.SwissTCS
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.MiST2
Imports SMRUCC.genomics.Data.SABIORK

<Package("Protein.Interactions.Tools", Category:=APICategories.CLI_MAN,
                  Description:="Tools for analysis the protein interaction relationship.",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Url:="http://gcmodeller.org")>
Public Module CLI

    <ExportAPI("--interact.TCS", Usage:="--interact.TCS /door <door.opr> /MiST2 <mist2.xml> /swiss <tcs.csv.DIR> /out <out.DIR>")>
    Public Function TCSParser(args As CommandLine) As Integer
        Dim MiST2 = args("/mist2").LoadXml(Of MiST2)  ' 主要是从这个模块之中获取TCS的基因定义
        Dim DOOR As DOOR = DOOR_API.Load(args("/door"))
        Dim cTkDIR As String = args("/swiss")
        Dim outDIR As String = args.GetValue("/out", App.CurrentDirectory)
        Dim CrossTalks = FileIO.FileSystem.GetFiles(cTkDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.csv") _
            .Select(Function(csv) csv.LoadCsv(Of CrossTalks)).Unlist

        For Each rep As Replicon In MiST2.MajorModules

            Dim lstHisk As String() = rep.TwoComponent.get_HisKinase
            Dim lstRR As String() = rep.TwoComponent.GetRR

            For Each HisK As String In lstHisk
                Dim lstChunk As New List(Of CrossTalks)

                For Each RR As String In lstRR

                    Dim p As Double = CrossTalks.CrossTalk(HisK, RR)

                    If DOOR.SameOperon(HisK, RR) Then  ' 同源的？？？
                        If Not p > 0 Then
                            p = 1
                        End If

                        Call lstChunk.Add(New CrossTalks With {.Kinase = HisK, .Regulator = RR, .Probability = p})
                    Else
                        If p > 0 Then
                            Call lstChunk.Add(New CrossTalks With {.Kinase = HisK, .Regulator = RR, .Probability = p})
                        End If
                    End If
                Next

                If Not lstChunk.IsNullOrEmpty Then
                    Call lstChunk.SaveTo(outDIR & $"/{HisK}.csv")
                End If
            Next
        Next

        Return 0
    End Function

    <ExportAPI("/BioGRID.selects",
               Usage:="/BioGRID.selects /in <in.DIR/*.Csv> /key <GeneId> /links <BioGRID-links.mitab.txt> [/out <out.DIR/*.Csv>]")>
    <Group(CLIGroupping.BioGridTools)>
    Public Function BioGRIDSelects(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim links As String = args("/links")
        Dim key As String = args.GetValue("/key", "GeneId")

        If [in].FileExists Then
            Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".BioGRID.Csv")
            Dim result As EntityObject() = BioGRID.API.Selects(
                [in].LoadCsv(Of EntityObject)(maps:={
                    {key, NameOf(EntityObject.ID)}
                }),
                BioGRID.LoadAllmiTab(links)).ToArray

            Return result.SaveTo(out).CLICode
        Else
            Dim net As ALLmitab() = BioGRID.LoadAllmiTab(links).ToArray
            Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".BioGRID_selects/")

            For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
                Dim result As EntityObject() = file.LoadCsv(Of EntityObject)(maps:={
                    {key, NameOf(EntityObject.ID)}
                })
                Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"

                result = BioGRID.API.Selects(result, net).ToArray
                result.SaveTo(out)
            Next
        End If

        Return 0
    End Function

    <ExportAPI("/BioGRID.Id.types", Usage:="/BioGRID.Id.types /in <BIOGRID-IDENTIFIERS.tsv> [/out <out.txt>]")>
    <Group(CLIGroupping.BioGridTools)>
    Public Function BioGridIdTypes(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Types.txt")
        Dim source As IEnumerable(Of IDENTIFIERS) = LoadIdentifiers([in])
        Dim types As String() = source.AllIdentifierTypes
        Return types.SaveTo(out).CLICode
    End Function

    '<ExportAPI("/sabiork.kinetics")>
    '<Usage("/sabiork.kinetics [/output <dir>]")>
    'Public Function sabiorkKinetics(args As CommandLine) As Integer
    '    Dim output As String = args("/output") Or "./"
    '    Dim idlist As String() = KEGG.DBGET.BriteHEntry.htext.br08201.Hierarchical.EnumerateEntries.Select(Function(r) r.entryID).Distinct.ToArray

    '    Call idlist.QueryUsing_KEGGId(output).GetJson.SaveTo($"{output}/failures.json")

    '    Return 0
    'End Function
End Module
