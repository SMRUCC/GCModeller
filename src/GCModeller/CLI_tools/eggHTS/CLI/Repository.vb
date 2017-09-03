Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.mysql
Imports SMRUCC.genomics.Data.Repository.kb_UniProtKB
Imports SMRUCC.genomics.foundation.OBO_Foundry

Partial Module CLI

    <ExportAPI("/Imports.Go.obo.mysql")>
    <Description("")>
    <Usage("/Imports.Go.obo.mysql /in <go.obo> [/out <out.sql>]")>
    <Group(CLIGroups.Repository_CLI)>
    Public Function DumpGOAsMySQL(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".kb_go.sql")

        Return New OBOFile([in]) _
            .DumpMySQL(saveSQL:=out) _
            .CLICode
    End Function

    <ExportAPI("/Imports.Uniprot.Xml")>
    <Usage("/Imports.Uniprot.Xml /in <uniprot.xml> [/out <out.sql>]")>
    <Group(CLIGroups.Repository_CLI)>
    Public Function DumpUniprot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".kb_go.sql")
        Dim proteins = UniProtXML.EnumerateEntries(path:=[in])

        Return proteins _
            .DumpMySQL(savedSQL:=out) _
            .CLICode
    End Function
End Module
