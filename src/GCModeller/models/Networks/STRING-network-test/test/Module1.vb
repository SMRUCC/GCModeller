Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Model.Network.STRING
Imports Microsoft.VisualBasic.Data.visualize.Network

Module Module1

    Sub Main()
        Dim edges = InteractExports.ImportsTsv("D:\GCModeller\src\GCModeller\models\Networks\STRING-network-test\string_interactions.tsv")
        Dim nodes = "D:\GCModeller\src\GCModeller\models\Networks\STRING-network-test\string_network_coordinates.txt".LoadTsv(Of Coordinates)

        Call GraphModel _
            .CreateGraph(edges, nodes) _
            .DrawImage(canvasSize:="2000,2000", hideDisconnectedNode:=True) _
            .Save("D:\GCModeller\src\GCModeller\models\Networks\STRING-network-test\string_interactions.png")

        Pause()
    End Sub
End Module
