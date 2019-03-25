Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Assembly

Partial Module CLI

    <ExportAPI("/iGEM.select.parts")>
    <Usage("/iGEM.select.parts /list <id.list.txt> /allparts <ALL_parts.fasta> [/out <table.xls>]")>
    <Description("Select iGEM part sequence by given id list.")>
    <Group(Program.iGEMTools)>
    Public Function SelectParts(args As CommandLine) As Integer
        Dim in$ = args <= "/list"
        Dim allparts = iGEM.PartSeq.Parse(args <= "/allparts").ToDictionary(Function(p) p.PartName)
        Dim out$ = args("out") Or $"{[in].TrimSuffix}.iGEM_parts.xls"
        Dim idList = [in].IterateAllLines _
            .Select(Function(line)
                        Return Strings.Trim(line) _
                            .StringSplit("[\s,\t]+") _
                            .FirstOrDefault
                    End Function) _
            .Where(Function(id) Not id.StringEmpty) _
            .ToArray

        If [in].ExtensionSuffix = "csv" Then
            ' skip header
            idList = idList.Skip(1).ToArray
        End If

        Dim subset = idList _
            .Select(Function(id)
                        If allparts.ContainsKey(id) Then
                            Return allparts(id)
                        Else
                            ' empty line for sequence not found
                            Return New iGEM.PartSeq With {
                                .PartName = id
                            }
                        End If
                    End Function) _
            .ToArray

        Return subset.SaveTo(out, tsv:=True).CLICode
    End Function
End Module