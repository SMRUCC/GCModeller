Imports System.Text
Imports RDotNet.Extensions.VisualBasic

Namespace bnlearn

    Public Class BnlearnModelling(Of TNodeType As INetworkNodeValue) : Inherits bnlearn

        Protected TempData As String, NetworkData As TNodeType()

        Sub New(NetworkValues As TNodeType())
            NetworkData = NetworkValues
            TempData = BuildDataFile()
        End Sub

        Protected Overridable Function BuildDataFile() As String
            Dim Length As Integer = NetworkData.First.NodeValueVectors.Count - 1
            Dim ChunkBuffer As StringBuilder() = (From n As Integer In NetworkData.First.NodeValueVectors Select New StringBuilder(NetworkData.Count * 2)).ToArray
            Dim CsvBuilder As StringBuilder = New StringBuilder(1024)

            For Each item As TNodeType In NetworkData
                Dim DataChunk = item.NodeValueVectors

                For i As Integer = 0 To ChunkBuffer.Count - 1
                    Call ChunkBuffer(i).Append(Chr(DataChunk(i)) & ",")
                Next
                Call CsvBuilder.Append(item.Identifier & ",")
            Next
            Call CsvBuilder.Remove(CsvBuilder.Length - 1, 1)
            Call CsvBuilder.AppendLine()

            For Each line As StringBuilder In ChunkBuffer
                Call line.Remove(line.Length - 1, 1)
                Call CsvBuilder.AppendLine(line.ToString)
            Next

            TempData = String.Format("{0}/{1}.csv", Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), New Random().NextDouble * 10 ^ 10)
            Call FileIO.FileSystem.WriteAllText(TempData, CsvBuilder.ToString, False)
            Return TempData
        End Function

        Protected Overrides Function __R_script() As String
            Dim scriptBuilder As StringBuilder = New StringBuilder(4096)
            Call scriptBuilder.AppendLine("library(bnlearn)")
            Call scriptBuilder.AppendLine(createNetwork(NetworkData))
            Call scriptBuilder.AppendLine(String.Format("dip_data <- read.csv(""{0}"");", TempData.Replace("\", "/")))
            Call scriptBuilder.AppendLine("dip_network <- iamb(dip_data);")
            Call scriptBuilder.AppendLine("dip_network_model <- bn.fit(dip_network, dip_data, method=""bayes"");")
            Call scriptBuilder.AppendLine("dip_network_model")

            Return scriptBuilder.ToString
        End Function

        ''' <summary>
        ''' 创建一个空网络，并构建好初始的网络结构
        ''' </summary>
        ''' <param name="NetworkData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function createNetwork(NetworkData As TNodeType()) As String
            Dim vecBuilder As StringBuilder = New StringBuilder(4096)
            For Each Token As String In (From node In NetworkData Select node.Identifier).ToArray
                Call vecBuilder.Append(String.Format("""{0}"", ", Token))
            Next
            Call vecBuilder.Remove(vecBuilder.Length - 2, 2)

            Dim scriptBuilder As StringBuilder = New StringBuilder(String.Format("dip_network <- empty.graph(c({0}));" & vbCrLf, vecBuilder.ToString), 1024 * 128)
            Call vecBuilder.Clear()
            For Each Node As TNodeType In NetworkData
                For Each connectedNode As String In Node.ConnectedToNodes
                    Call vecBuilder.Append(String.Format("""{0}"", ""{1}"", ", Node.Identifier, connectedNode))
                Next
            Next
            Call vecBuilder.Remove(vecBuilder.Length - 2, 2)
            Call scriptBuilder.AppendLine(String.Format("arc.set <- matrix(c({0}), ncol=2, byrow=TRUE, dimnames=list(NULL, c(""from"", ""to"")));", vecBuilder.ToString))
            Call scriptBuilder.AppendLine("arcs(dip_network) <- arc.set")

            Return scriptBuilder.ToString
        End Function

        Public Shared Function Convert(Sequence As Char()) As Integer()
            Dim LQuery = (From c As Char In Sequence Select Asc(c)).ToArray
            Return LQuery
        End Function
    End Class
End Namespace