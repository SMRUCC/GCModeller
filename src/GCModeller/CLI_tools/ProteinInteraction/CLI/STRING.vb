#Region "Microsoft.VisualBasic::11ee25245f6db3496c01c4734430c1fa, ..\GCModeller\CLI_tools\ProteinInteraction\CLI\STRING.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.BioGRID
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.SwissTCS
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.MiST2
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Data.STRING.SimpleCsv
Imports SMRUCC.genomics.Data.STRING.StringDB.Tsv

Partial Module CLI

    <ExportAPI("/STRING.selects",
               Usage:="/STRING.selects /in <in.DIR/*.Csv> /key <GeneId> /links <links.txt> /maps <maps_id.tsv> [/out <out.DIR/*.Csv>]")>
    <Group(CLIGroupping.STRING_tools)>
    Public Function STRINGSelects(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim links As String = args("/links")
        Dim maps As String = args("/maps")
        Dim key As String = args.GetValue("/key", "GeneId")
        Dim mapsKey As New Dictionary(Of String, String) From {
            {key, NameOf(EntityObject.ID)}
        }
        Dim mapNames As Dictionary(Of String, String) =
            entrez_gene_id_vs_string.BuildMapsFromFile(maps)

        If [in].FileExists Then
            Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".STRING.Csv")
            Dim result As EntityObject() = linksDetail.Selects(
                [in].LoadCsv(Of EntityObject)(maps:=mapsKey),
                linksDetail.LoadFile(links),
                mapNames).ToArray

            Return result.SaveTo(out).CLICode
        Else
            Dim net As linksDetail() = linksDetail.LoadFile(links).ToArray
            Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".STRING_selects/")

            For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
                Dim result As EntityObject() = file.LoadCsv(Of EntityObject)(maps:=mapsKey)
                Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"

                result = linksDetail.Selects(result, net, mapNames).ToArray
                result.SaveTo(out)
            Next
        End If

        Return 0
    End Function

    <ExportAPI("/STRING.Network",
               Usage:="/STRING.Network /id <uniprot_idMappings.tsv> /links <protein.actions-links.tsv> [/sub <idlist.txt> /attributes <dataset.csv> /id_field <ID> /all_links <protein.links.txt> /out <outDIR>]")>
    <Group(CLIGroupping.STRING_tools)>
    Public Function StringNetwork(args As CommandLine) As Integer
        Dim idTsv = args("/id")
        Dim links$ = args("/links")
        Dim alllinks As String = args("/all_links")
        Dim sublist As String = args("/sub")
        Dim out = args.GetValue(
            "/out",
            idTsv.TrimSuffix & "-" & links.BaseName & $"{If(sublist.FileExists, "-" & sublist.BaseName, "")}/")
        Dim maps = Uniprot.Web.SingleMappings(idTsv)
        Dim dataset = args("/attributes")

        If sublist.FileExists Then
            Dim list = sublist.ReadAllLines
            maps = list _
                .Where(Function(id) maps.ContainsKey(id)) _
                .ToDictionary(Function(k) k,
                              Function(s) maps(s))
        End If

        maps = maps.ReverseMaps(True)

        Dim net As FileStream.Network = If(
            alllinks.FileExists(True),
            maps.MatchNetwork(actions:=links, links:=alllinks),
            maps.MatchNetwork(actions:=links))

        If dataset.FileExists Then
            Dim idField$ = args("/id_field")

            With EntityObject.LoadDataSet(dataset).ToDictionary
                For Each node As Node In net.Nodes
                    Dim ID$

                    If String.IsNullOrEmpty(idField) Then
                        ID = node.ID
                    Else
                        ID = node(idField)
                    End If

                    If .ContainsKey(ID) Then

                        With .Item(ID)
                            For Each k In .Properties.Keys
                                node.Properties(k) = .Properties(k)
                            Next
                        End With
                    End If
                Next
            End With
        End If

        Return net.Save(out).CLICode
    End Function
End Module

