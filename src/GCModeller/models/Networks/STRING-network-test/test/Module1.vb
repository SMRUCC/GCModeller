Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Data.STRING

Module Module1

    Sub Main()
        Dim edges = InteractExports.ImportsTsv("D:\GCModeller\src\GCModeller\models\Networks\STRING-network-test\string_interactions.tsv")
        Dim nodes = "D:\GCModeller\src\GCModeller\models\Networks\STRING-network-test\string_network_coordinates.txt".LoadTsv(Of Coordinates)


        Pause()
    End Sub
End Module
