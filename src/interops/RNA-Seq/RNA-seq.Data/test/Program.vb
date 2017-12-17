Imports SMRUCC.genomics.SequenceModel.FQ

Module Program

    Sub Main()
        Dim q = FastQ.GetQualityOrder("@"c)
        q = FastQ.GetQualityOrder("5"c)

        Call Stream _
            .ReadAllLines("F:\2017-12-6-16s_test\test\T1-1_combined_R1.fastq") _
            .TrimLowQuality _
            .TrimShortReads(200) _
            .WriteFastQ("F:\2017-12-6-16s_test\test\T1-1_combined_R1.trim.fastq")

        Call Stream _
            .ReadAllLines("F:\2017-12-6-16s_test\test\T1-1_combined_R2.fastq") _
            .TrimLowQuality _
            .TrimShortReads(200) _
            .WriteFastQ("F:\2017-12-6-16s_test\test\T1-1_combined_R2.trim.fastq")

        Pause()
    End Sub
End Module
