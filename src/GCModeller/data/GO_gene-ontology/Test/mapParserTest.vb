Imports GO_Annotation

Module mapParserTest
    Sub Main()
        Dim maps = toGO.Parse2GO("P:\2019_bio_videos\20190826_go_annotation\pfam2go.txt").ToArray

        Pause()
    End Sub
End Module
