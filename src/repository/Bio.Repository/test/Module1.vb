Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Module Module1

    Sub Main()
        Call readTest()
    End Sub

    Sub writeTest()
        Dim list = CompoundRepository.ScanRepository("E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd").ToArray

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo".Open
            Call KEGGCompoundPack.WriteKeggDb(list, file)
        End Using
    End Sub

    Sub readTest()
        Dim list As Compound()

        Using file = "E:\biodeep\biodeepdb_v3\KEGG\KEGG_cpd.repo".Open(IO.FileMode.Open, doClear:=False, [readOnly]:=True)
            list = KEGGCompoundPack.ReadKeggDb(file)
        End Using

        Pause()
    End Sub
End Module
