Imports SMRUCC.genomics.SequenceModel.FQ

Module Program

    Sub Main()
        Dim q = FastQ.GetQualityOrder("@"c)

        Call Stream _
            .ReadAllLines("F:\2017-12-6-16s_test\test\T1-1_combined_R1.fastq") _
            .TrimLowQuality _
            .TrimShortReads(100) _
            .WriteFastQ("F:\2017-12-6-16s_test\test\T1-1_combined_R1.trim.fastq")


        Pause()
    End Sub
End Module
