Imports System.IO
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.PICRUSt

Module Program

    Dim dbfile As String = "P:\ko_13_5_precalculated.PICRUSt"

    Sub Main(args As String())
        ' Call testWrite()
        Call testRead()
    End Sub

    Sub testRead()
        Using file As New MetaBinaryReader(dbfile.Open(FileMode.Open, doClear:=False, [readOnly]:=True))

        End Using
    End Sub

    Sub testWrite()
        Dim gg = otu_taxonomy.Load("P:\gg_13_8_99.gg.tax")

        Using file As Stream = dbfile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
            writer = MetaBinaryWriter.CreateWriter(gg, file)

            Dim raw = "P:\ko_13_5_precalculated.tab".Open(FileMode.Open, doClear:=False, [readOnly]:=True)

            Call writer.ImportsComputes(raw)

        End Using
    End Sub
End Module
