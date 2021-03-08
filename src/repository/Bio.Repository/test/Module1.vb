Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Module Module1

    Sub Main()
        Dim list = CompoundRepository.ScanRepository("E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd").ToArray

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo".Open
            Call KEGGCompoundPack.WriteKeggDb(list, file)
        End Using
    End Sub
End Module
