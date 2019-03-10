#Region "Microsoft.VisualBasic::33003fdca740e8144ac5c6f6fd3edce2, CLI_tools\S.M.A.R.T\CLI\UniProt.vb"

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
    '     Function: UniProt2Pfam, UniProtXmlDomains
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports Protein = SMRUCC.genomics.Assembly.Uniprot.XML.entry

Partial Module CLI

    <ExportAPI("/UniProt.domains")>
    <Usage("/UniProt.domains /in <uniprot.Xml> [/map <maps.tab/tsv> /out <proteins.csv>]")>
    <Description("Export the protein structure domain annotation table from UniProt database dump.")>
    Public Function UniProtXmlDomains(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.domains.csv"
        Dim proteins As PfamString()

        With args <= "/map"
            If .FileExists Then
                Dim mappings = Retrieve_IDmapping.MappingReader(.ByRef)

                With UniProtXML.LoadDictionary([in])
                    proteins = mappings _
                        .Where(Function(ID)
                                   Return ID.Value.Any(Function(s) .ContainsKey(s))
                               End Function) _
                        .Select(Function(map)
                                    Dim ID As String = map.Key
                                    Dim uniprotID$ = map.Value _
                                        .Where(Function(acc) .ContainsKey(acc)) _
                                        .First
                                    Dim protein As PfamString = .ByRef(uniprotID).UniProt2Pfam.First
                                    protein.ProteinId = ID
                                    Return protein
                                End Function) _
                        .ToArray
                End With
            Else
                proteins = UniProtXML _
                    .EnumerateEntries(path:=[in]) _
                    .Select(AddressOf UniProt2Pfam) _
                    .IteratesALL _
                    .ToArray
            End If
        End With

        Return proteins _
            .SaveTo(out) _
            .CLICode
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Iterator Function UniProt2Pfam(protein As Protein) As IEnumerable(Of PfamString)
        For Each ID As String In protein.accessions
            Yield New PfamString With {
                .ProteinId = ID,
                .Description = protein.proteinFullName,
                .Length = protein.sequence.length,
                .Domains = protein.features _
                    .SafeQuery _
                    .Where(Function(f) f.type = "domain") _
                    .Select(Function(feature) feature.description) _
                    .Distinct _
                    .ToArray,
                .PfamString = protein.features _
                    .SafeQuery _
                    .Where(Function(f) f.type = "domain") _
                    .OrderBy(Function(feature)
                                 Return feature.location.begin.position
                             End Function) _
                    .Select(Function(feature)
                                Return $"{feature.description}({feature.location.begin.position}|{feature.location.end.position})"
                            End Function) _
                    .ToArray
            }
        Next
    End Function
End Module
