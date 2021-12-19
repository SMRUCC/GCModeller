Imports System
Imports System.IO
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.PICRUSt

Module Program
    Sub Main(args As String())
        Dim gg = otu_taxonomy.Load("P:\gg_13_8_99.gg.tax")

        Using file As Stream = "P:\ko_13_5_precalculated.PICRUSt".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
            writer = MetaBinaryWriter.CreateWriter(gg, file)

            Dim raw = "P:\ko_13_5_precalculated.tab".Open(FileMode.Open, doClear:=False, [readOnly]:=True)

            Call writer.ImportsComputes(raw)

        End Using
    End Sub
End Module
