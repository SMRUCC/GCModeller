Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions




Public Class Program
    Public Shared Sub Main(args As String())
        Dim data As ClusterEntity() = Enumerable.Range(0, 100).[Select](Function(x) New ClusterEntity With {.uid = x.ToString, .entityVector = New Double() {randf.randf(0, 99999)}}).ToArray()
        Dim result As List(Of Classify) = CMeans.CMeans(10, data)

        For Each item In result
            Console.WriteLine($"===== {item.Id} (Count:{item.Values.Count}) =====")

            For Each item2 In item.Values.OrderBy(Function(x) x.entityVector.Average())
                Console.WriteLine(String.Join(", ", item2))
            Next
        Next

        Console.ReadKey()
    End Sub
End Class