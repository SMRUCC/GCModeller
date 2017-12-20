Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString

Partial Module CLI

    <ExportAPI("/UniProt.domains")>
    <Usage("/UniProt.domains /in <uniprot.Xml> [/out <proteins.csv>]")>
    <Description("Export the protein structure domain annotation table from UniProt database dump.")>
    Public Function UniProtXmlDomains(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.domains.csv"
        Dim proteins = UniProtXML _
            .EnumerateEntries(path:=[in]) _
            .Select(AddressOf UniProt2Pfam) _
            .IteratesALL _
            .ToArray

        Return proteins _
            .SaveTo(out) _
            .CLICode
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Iterator Function UniProt2Pfam(protein As entry) As IEnumerable(Of PfamString)
        For Each ID As String In protein.accessions
            Yield New PfamString With {
                .ProteinId = protein.accessions.JoinBy("; "),
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