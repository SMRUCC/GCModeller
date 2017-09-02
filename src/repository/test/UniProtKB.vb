Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.Repository.kb_UniProtKB

Module UniProtKB

    Sub Main()
        Call UniprotXML _
            .EnumerateEntries("G:\GCModeller-repo\uniprot-all.xml\uniprot-id-Q9Y478+OR+id-P54619+OR+id-Q5VST6+OR+id-Q7Z5R6+OR+id-Q9NRW3+--.xml") _
            .DumpMySQL("./test.sql")
    End Sub
End Module
