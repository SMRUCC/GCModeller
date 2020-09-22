Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace CMeans
    Public Class Program
        Public Shared Sub Main(args As String())
            Dim data As List(Of Double()) = Enumerable.Range(0, 100).[Select](Function(x) New Double() {randf.randf(0, 99999)}).ToList()
            Dim result As List(Of Classify) = CMeans(10, data)

            For Each item In result
                Console.WriteLine($"===== {item.Id} (Count:{item.Values.Count}) =====")

                For Each item2 In item.Values.OrderBy(Function(x) x.Average())
                    Console.WriteLine(String.Join(", ", item2))
                Next
            Next

            Console.ReadKey()
        End Sub

        Public Shared Function CMeans(classCount As Integer, Values As List(Of Double())) As List(Of Classify)
            Return CMeans(classCount, 2, 0.001, Values)
        End Function

        Public Shared Function CMeans(classCount As Integer, m As Double, Values As List(Of Double())) As List(Of Classify)
            Return CMeans(classCount, m, 0.001, Values)
        End Function

        Private Shared Iterator Function GetRandomMatrix(classCount As Integer, nsamples As Integer) As IEnumerable(Of Double())
            For Each x As Double In Enumerable.Range(0, nsamples)
                Dim c = New Double(classCount - 1) {}

                For i = 0 To c.Length - 1
                    c(i) = randf.randf(0, 1 - c.Sum())
                    If i = c.Length - 1 Then c(i) = 1 - c.Sum()
                Next

                Yield c
            Next
        End Function

        Public Shared Function CMeans(classCount As Integer, m As Double, diff As Double, Values As List(Of Double())) As List(Of Classify)
            Dim u As Double()() = GetRandomMatrix(classCount, nsamples:=Values.Count).ToArray()
            Dim _j As Double = -1
            Dim centers As List(Of Double())

            While True
                centers = GetCenters(classCount, m, u, Values)
                Dim j_new As Double = J(m, u, centers, Values)

                If _j <> -1 AndAlso Math.Abs(j_new - _j) < diff Then Exit While
                _j = j_new

                For i As Integer = 0 To u.Length - 1
                    Dim index As Integer = i

                    For j As Integer = 0 To u(i).Length - 1
                        Dim jIndex As Integer = j

                        u(i)(j) = 1 / Enumerable.Select(Enumerable.Range(CInt(0), CInt(classCount)), CType(Function(x) Math.Pow(Math.Sqrt(Dist(CType(Values(CInt(index)), Double()), CType(centers(CInt(jIndex)), Double()))) / Math.Sqrt(Dist(CType(Values(CInt(index)), Double()), CType(centers(CInt(x)), Double()))), 2 / (m - 1)), Func(Of Integer, Double))).Sum()

                        If Double.IsNaN(u(i)(j)) Then
                            u(i)(j) = 1
                        End If
                    Next
                Next
            End While

            Dim result As List(Of Classify) = Enumerable.Range(0, classCount) _
                .[Select](Function(x, i)
                              Return New Classify() With {
                                  .Id = i
                              }
                          End Function) _
                .ToList()

            For i = 0 To Values.Count - 1
                Dim index = Array.IndexOf(u(i), u(i).Max())
                result(index).Values.Add(Values(i))
            Next

            Return result
        End Function

        Public Shared Function GetCenters(classCount As Integer, m As Double, u As Double()(), Values As List(Of Double())) As List(Of Double())
            Return Enumerable.Range(0, classCount).[Select](Function(i) Enumerable.Range(0, Enumerable.First(Values).Length).[Select](Function(x) Enumerable.Select(Enumerable.Range(CInt(0), CInt(Values.Count)), CType(Function(j) Math.Pow(CDbl(u(CInt(j))(CInt(i))), m) * Values(CInt(j))(CInt(x)), Func(Of Integer, Double))).Sum() / Enumerable.Select(Of Integer, Global.System.[Double])(Enumerable.Range(CInt(0), CInt(Values.Count)), CType(Function(j) Math.Pow(CDbl(u(CInt(j))(CInt(i))), m), Func(Of Integer, Double))).Sum()).ToArray()).ToList()
        End Function

        Public Shared Function J(m As Double, u As Double()(), centers As List(Of Double()), values As List(Of Double())) As Double
            Return Enumerable.Select(centers, CType(Function(x, i) Enumerable.Sum(Enumerable.Select(values, CType(Function(y, j1) Math.Pow(CDbl(u(CInt(j1))(CInt(i))), m) * Dist(CType(y, Double()), CType(x, Double())), Func(Of Double(), Integer, Double)))), Func(Of Double(), Integer, Double))).Sum()
        End Function

        Public Shared Function Dist(value As Double(), center As Double()) As Double
            Return Enumerable.Select(value, CType(Function(x, i) Math.Pow(x - center(CInt(i)), CDbl(2)), Func(Of Double, Integer, Double))).Sum()
        End Function
    End Class
End Namespace
