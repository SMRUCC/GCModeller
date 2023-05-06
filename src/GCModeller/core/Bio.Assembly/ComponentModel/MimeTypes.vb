Namespace ComponentModel

    Public Module MimeTypes

        ''' <summary>
        ''' FASTQ format is a text-based format for storing both a biological sequence 
        ''' (usually nucleotide sequence) and its corresponding quality scores. Both the
        ''' sequence letter and quality score are each encoded with a single ASCII 
        ''' character for brevity. It was originally developed at the Wellcome Trust 
        ''' Sanger Institute to bundle a FASTA formatted sequence and its quality data, 
        ''' but has recently become the de facto standard for storing the output of 
        ''' high-throughput sequencing instruments such as the Illumina Genome Analyzer.
        ''' </summary>
        Public Const FastQ As String = "text/plain, chemical/seq-na-fastq"

    End Module
End Namespace