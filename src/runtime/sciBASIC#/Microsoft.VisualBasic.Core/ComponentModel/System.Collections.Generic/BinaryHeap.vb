Imports Microsoft.VisualBasic.Language

Namespace ComponentModel.Collection

    Public Class BinaryHeap

        Public content As New List(Of Object)
        Public scoreFunction As Func(Of Object, Double)

        Public ReadOnly Property size As Integer
            Get
                Return content.Count
            End Get
        End Property

        Sub New(scoreFunction As Func(Of Object, Double))
            Me.scoreFunction = scoreFunction
        End Sub

        Public Sub push(element)
            content.Add(element)
            bubbleUp(content.Count - 1)
        End Sub

        Public Function pop()
            Dim result = content(Scan0)
            Dim [end] = content.Pop

            If content > 0 Then
                content(Scan0) = [end]
                sinkDown(Scan0)
            End If

            Return result
        End Function

        Public Function peek() As Object
            Return content(Scan0)
        End Function

        Public Sub remove(node)
            Dim len = content.Count

            For i As Integer = 0 To len - 1
                If content(i) Is node Then
                    Dim [end] = content.Pop

                    If i <> len - 1 Then
                        content(i) = [end]

                        If scoreFunction([end]) < scoreFunction(node) Then
                            bubbleUp(i)
                        Else
                            sinkDown(i)
                        End If
                    End If

                    Return
                End If
            Next

            Throw New Exception("Node not found.")
        End Sub

        Private Sub bubbleUp(n As Integer)
            Dim element = content(n)

            Do While n > 0
                Dim parentN = Math.Floor((n + 1) / 2) - 1,
            parent = content(parentN)

                If scoreFunction(element) < scoreFunction(parent) Then
                    content(parentN) = element
                    content(n) = parent
                    n = parentN
                Else
                    Exit Do
                End If
            Loop
        End Sub

        Private Sub sinkDown(n As Integer)
            Dim length = content.Count,
           element = content(n),
           elemScore = scoreFunction(element)
            Dim child1Score As Double

            Do While True
                Dim child2N = (n + 1) * 2, child1N = child2N - 1
                Dim swap = Nothing

                If (child1N < length) Then
                    ' Look it up And compute its score.
                    Dim child1 = content(child1N)
                    child1Score = scoreFunction(child1)
                    ' If the score Is less than our element's, we need to swap.
                    If child1Score < elemScore Then
                        swap = child1N
                    End If
                End If

                If child2N < length Then
                    Dim child2 = content(child2N),
              child2Score = scoreFunction(child2)
                    If (child2Score < If(swap Is Nothing, elemScore, child1Score)) Then
                        swap = child2N
                    End If
                End If

                If Not swap Is Nothing Then
                    content(n) = content(swap)
                    content(swap) = element
                    n = swap
                Else
                    Exit Do
                End If
            Loop
        End Sub
    End Class
End Namespace