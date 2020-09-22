Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' the cmeans algorithm module
''' </summary>
Public Module CMeans
    Public Function CMeans(classCount As Integer, Values As IEnumerable(Of ClusterEntity)) As Classify()
        Return CMeans(classCount, 2, 0.001, Values.ToArray)
    End Function

    Public Function CMeans(classCount As Integer, m As Double, Values As IEnumerable(Of ClusterEntity)) As Classify()
        Return CMeans(classCount, m, 0.001, Values.ToArray)
    End Function

    Private Iterator Function GetRandomMatrix(classCount As Integer, nsamples As Integer) As IEnumerable(Of Double())
        For Each x As Double In Enumerable.Range(0, nsamples)
            Dim c = New Double(classCount - 1) {}

            For i = 0 To c.Length - 1
                c(i) = randf.randf(0, 1 - c.Sum())
                If i = c.Length - 1 Then c(i) = 1 - c.Sum()
            Next

            Yield c
        Next
    End Function

    Public Function CMeans(classCount As Integer, m As Double, diff As Double, Values As ClusterEntity()) As Classify()
        Dim u As Double()() = GetRandomMatrix(classCount, nsamples:=Values.Length).ToArray()
        Dim _j As Double = -1
        Dim centers As Double()()
        Dim width As Integer = Values(0).Length

        While True
            centers = GetCenters(classCount, m, u, Values, width).ToArray
            Dim j_new As Double = J(m, u, centers, Values)

            If _j <> -1 AndAlso Math.Abs(j_new - _j) < diff Then Exit While
            _j = j_new

            For i As Integer = 0 To u.Length - 1
                Dim index As Integer = i

                For j As Integer = 0 To u(i).Length - 1
                    Dim jIndex As Integer = j

                    u(i)(j) = 1 / Enumerable.Range(0, classCount) _
                        .Select(Function(x)
                                    Return Math.Pow(Math.Sqrt(Dist(Values(index), centers(jIndex))) / Math.Sqrt(Dist(Values(index), centers(x))), 2 / (m - 1))
                                End Function) _
                        .Sum()

                    If Double.IsNaN(u(i)(j)) Then
                        u(i)(j) = 1
                    End If
                Next
            Next
        End While

        Return Values.PopulateClusters(classCount, u)
    End Function

    <Extension>
    Private Function PopulateClusters(values As ClusterEntity(), classCount As Integer, u As Double()()) As Classify()
        Dim result As Classify() = Enumerable.Range(0, classCount) _
          .[Select](Function(x, i)
                        Return New Classify() With {
                            .Id = i
                        }
                    End Function) _
          .ToArray
        Dim index As Integer
        Dim maxMembership As Double

        For i As Integer = 0 To values.Length - 1
            maxMembership = u(i).Max()
            index = Array.IndexOf(u(i), maxMembership)
            result(index).members.Add(values(i))
        Next

        Return result
    End Function

    Public Iterator Function GetCenters(classCount As Integer, m As Double, u As Double()(), Values As ClusterEntity(), width As Integer) As IEnumerable(Of Double())
        For Each i As Integer In Enumerable.Range(0, classCount)
            Yield Enumerable.Range(0, width) _
                              .[Select](Function(x)
                                            Dim sumAll = Aggregate j As Integer In Enumerable.Range(0, Values.Count)
                                                   Let val As Double = Math.Pow(u(j)(i), m) * Values(j)(x)
                                                   Into Sum(val)
                                            Dim b = Aggregate j As Integer In Enumerable.Range(0, Values.Count)
                                                        Let val As Double = Math.Pow(u(j)(i), m)
                                                           Into Sum(val)

                                            Return sumAll / b
                                        End Function) _
                              .ToArray()
        Next
    End Function

    Public Function J(m As Double, u As Double()(), centers As Double()(), values As ClusterEntity()) As Double
        Return centers.Select(Function(x, i)
                                  Return values.Select(Function(y, j1)
                                                           Return (u(j1)(i) ^ m) * Dist(y, x)
                                                       End Function).Sum()
                              End Function).Sum()
    End Function

    Private Function Dist(value As ClusterEntity, center As Double()) As Double
        Return value.entityVector.Select(Function(x, i) (x - center(i)) ^ 2).Sum()
    End Function
End Module
