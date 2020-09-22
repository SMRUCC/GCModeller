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

                If i = c.Length - 1 Then
                    c(i) = 1 - c.Sum()
                End If
            Next

            Yield c
        Next
    End Function

    Public Function CMeans(classCount As Integer, m As Double, diff As Double, Values As ClusterEntity(), Optional parallel As Boolean = True) As Classify()
        Dim u As Double()() = GetRandomMatrix(classCount, nsamples:=Values.Length).ToArray()
        Dim _j As Double = -1
        Dim centers As Double()()
        Dim width As Integer = Values(0).Length
        Dim j_new As Double
        Dim diffValue As Double

        While True
            centers = GetCenters(classCount, m, u, Values, width).ToArray
            j_new = J(m, u, centers, Values)
            diffValue = Math.Abs(j_new - _j)

            If _j <> -1 AndAlso diffValue < diff Then
                Exit While
            Else
                Call $"diff: |{j_new} - {_j}| = {diffValue}".__DEBUG_ECHO
            End If

            _j = j_new

            If parallel Then
                Call u.updateMembershipParallel(Values, centers, classCount, m)
            Else
                Call u.updateMembership(Values, centers, classCount, m)
            End If
        End While

        Return Values.PopulateClusters(classCount, u)
    End Function

    <Extension>
    Private Sub updateMembershipParallel(ByRef u As Double()(), Values As ClusterEntity(), centers As Double()(), classCount As Integer, m As Double)
        u = Enumerable.Range(0, u.Length) _
            .AsParallel _
            .Select(Function(i)
                        Dim result As Double() = centers.scanRow(
                            index:=i,
                            Values:=Values,
                            classCount:=classCount,
                            m:=m
                        )
                        Dim pack = (i, result)

                        Return pack
                    End Function) _
            .OrderBy(Function(pack) pack.i) _
            .Select(Function(a) a.result) _
            .ToArray
    End Sub

    <Extension>
    Private Function scanRow(centers As Double()(), index As Integer, Values As ClusterEntity(), classCount As Integer, m As Double) As Double()
        Dim ui As Double() = New Double(classCount - 1) {}

        For j As Integer = 0 To classCount - 1
            Dim jIndex As Integer = j
            Dim sumAll As Double = Aggregate x As Integer
                                   In Enumerable.Range(0, classCount)
                                   Let a As Double = Math.Sqrt(Dist(Values(index), centers(jIndex))) / Math.Sqrt(Dist(Values(index), centers(x)))
                                   Let val As Double = a ^ (2 / (m - 1))
                                   Into Sum(val)
            ui(j) = 1 / sumAll

            If Double.IsNaN(ui(j)) Then
                ui(j) = 1
            End If
        Next

        Return ui
    End Function

    <Extension>
    Private Sub updateMembership(u As Double()(), Values As ClusterEntity(), centers As Double()(), classCount As Integer, m As Double)
        For i As Integer = 0 To u.Length - 1
            Dim index As Integer = i

            For j As Integer = 0 To u(i).Length - 1
                Dim jIndex As Integer = j
                Dim sumAll As Double = Aggregate x As Integer
                                       In Enumerable.Range(0, classCount)
                                       Let a As Double = Math.Sqrt(Dist(Values(index), centers(jIndex))) / Math.Sqrt(Dist(Values(index), centers(x)))
                                       Let val As Double = a ^ (2 / (m - 1))
                                       Into Sum(val)
                u(i)(j) = 1 / sumAll

                If Double.IsNaN(u(i)(j)) Then
                    u(i)(j) = 1
                End If
            Next
        Next
    End Sub

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

    ''' <summary>
    ''' 在这里面只会计算结果值，并不会修改数据
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="center"></param>
    ''' <returns></returns>
    Private Function Dist(value As ClusterEntity, center As Double()) As Double
        Return value.entityVector.Select(Function(x, i) (x - center(i)) ^ 2).Sum()
    End Function
End Module
