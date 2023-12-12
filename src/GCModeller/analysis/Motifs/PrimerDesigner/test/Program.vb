Imports SMRUCC.genomics.Analysis.PrimerDesigner.Restriction_enzyme
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Module Program

    Const test_demo As String = "\GCModeller\src\GCModeller\visualize\data\addgene-plasmid-100854-sequence-189713.gbk"

    Sub Main(args As String())
        Call SlicerTest()
    End Sub

    Sub searchTest()
        Dim list = WikiLoader.PullAll.ToArray
        Dim plasmid As FastaSeq = GBFF.File.Load(test_demo).Origin.ToFasta
        Dim nt_scan As New Scanner(plasmid)
        Dim result As New List(Of SimpleSegment)

        For Each enzyme As Enzyme In list
            Dim motif = enzyme.TranslateRegular()
            Dim sites = motif.Scan(nt_scan)

            Call result.AddRange(sites)
        Next

        Pause()
    End Sub

    Sub SlicerTest()
        Dim plasmid As FastaSeq = GBFF.File.Load(test_demo).Origin.ToFasta
        Dim list = WikiLoader.PullAll.Shuffles.Take(5).ToArray
        Dim simulation As New Slicer(plasmid, list)
        Dim result = simulation.GetSegments.ToArray

        Pause()
    End Sub
End Module
