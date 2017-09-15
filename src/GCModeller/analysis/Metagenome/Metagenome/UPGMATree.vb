Imports System.Math
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO

''' <summary>
''' https://github.com/graph1994/UPGMA-Tree-Building-Application/blob/master/UPGMATreeCreator.py
''' https://en.wikipedia.org/wiki/UPGMA
''' </summary>
Public Module UPGMATree

    Public Class taxa

        Public id%
        Public data As taxa()
        Public label$
        Public size%
        Public distance#

        Sub New(id%, data As taxa(), size%, distance#)
            Me.id = id
            Me.data = data
            Me.size = size
            Me.distance = distance
        End Sub

        Sub New(id%, data$, size%, distance#)
            Me.id = id
            Me.label = data
            Me.size = size
            Me.distance = distance
        End Sub
    End Class

    Function form_taxas(species As taxa()) As Dictionary(Of Integer, taxa)
        Dim taxas As New Dictionary(Of Integer, taxa)
        Dim ids = 1

        For Each item In species
            Dim x As New taxa(ids, {item}, 1, 0)
            taxas(x.id) = x
            ids = ids + 1
        Next

        Return taxas
    End Function

    Function find_min(dic%(), array As List(Of Double())) As (i%, j%, lowest#)
        Dim lowest# = Integer.MaxValue
        Dim iMin = 0
        Dim jMin = 0

        For Each i In dic
            For Each j In dic
                If j > i Then
                    Dim tmp = array(j - 1)(i - 1)

                    If tmp <= lowest Then
                        iMin = i
                        jMin = j
                        lowest = tmp
                    End If
                End If
            Next
        Next
        Return (iMin, jMin, lowest)
    End Function

    Function combine(dic_taxas As Dictionary(Of Integer, taxa), matrix As List(Of Double())) As taxa
        Do While dic_taxas.Count <> 1
            Dim x As (i%, j%, dij#) = find_min(dic_taxas.Keys.ToArray, matrix)
            Dim i = x.i
            Dim j = x.j
            Dim dij = x.dij
            Dim icluster = dic_taxas(i)
            Dim jcluster = dic_taxas(j)

            Dim u As New taxa(dic_taxas.Keys.Max + 1, {icluster, jcluster}, (icluster.size + jcluster.size), (dij))
            dic_taxas.Remove(i)
            dic_taxas.Remove(j)
            matrix.Add({})
            For Each l In NumericSequence.Range(0, u.id - 1)
                matrix(u.id - 1).Add(0)
            Next
            For Each l In dic_taxas.Keys
                Dim dil = matrix(Max(i, l) - 1)(Min(i, l) - 1)
                Dim djl = matrix(Max(j, l) - 1)(Min(j, l) - 1)
                Dim dul = (dil * icluster.size + djl * jcluster.size) / (icluster.size + jcluster.size)
                matrix(u.id - 1)(l - 1) = dul
            Next

            dic_taxas(u.id) = u
        Loop

        ' 循环的退出条件为字典之中只有一个值
        Return dic_taxas.Values.First
    End Function

    Function taxaPrint(tax As taxa, distance#)
        If tax.size > 1 Then
            println("(")
            taxaPrint(tax.data(0), tax.distance)
            println(",")
            taxaPrint(tax.data(1), tax.distance)
            println("," & tax.distance & ")")
        Else
            ' println(tax.data)
        End If
    End Function

    <Extension>
    Public Function BuildTree(data As IEnumerable(Of DataSet)) As taxa
        Dim array = data.ToArray
        Dim inputs = array.Select(Function(x) New taxa(0, x.ID, 0, 0)).ToArray
        Dim matrix = array.Matrix
        Dim table = form_taxas(inputs)
        Dim tree = combine(table, matrix.AsList)
        Return tree
    End Function
End Module
