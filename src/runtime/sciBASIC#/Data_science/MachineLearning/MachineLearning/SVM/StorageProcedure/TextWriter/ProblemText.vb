
Imports System.IO
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace SVM

    Module ProblemText

        ''' <summary>
        ''' Reads a problem from a stream.
        ''' </summary>
        ''' <param name="stream">Stream to read from</param>
        ''' <returns>The problem</returns>
        Public Function Read(stream As Stream) As Problem
            Start()
            Dim input As StreamReader = New StreamReader(stream)
            Dim vy As List(Of Double) = New List(Of Double)()
            Dim vx As List(Of Node()) = New List(Of Node())()
            Dim max_index = 0

            While input.Peek() > -1
                Dim parts As String() = input.ReadLine().Trim().Split()
                vy.Add(Double.Parse(parts(0)))
                Dim m = parts.Length - 1
                Dim x = New Node(m - 1) {}

                For j = 0 To m - 1
                    x(j) = New Node()
                    Dim nodeParts = parts(j + 1).Split(":"c)
                    x(j).Index = Integer.Parse(nodeParts(0))
                    x(j).Value = Double.Parse(nodeParts(1))
                Next

                If m > 0 Then max_index = stdNum.Max(max_index, x(m - 1).Index)
                vx.Add(x)
            End While

            [Stop]()
            Return New Problem(vy.ToArray(), vx.ToArray(), max_index)
        End Function

        ''' <summary>
        ''' Writes a problem to a stream.
        ''' </summary>
        ''' <param name="stream">The stream to write the problem to.</param>
        ''' <param name="problem">The problem to write.</param>
        Public Sub Write(stream As Stream, problem As Problem)
            Start()
            Dim output As StreamWriter = New StreamWriter(stream)

            For i = 0 To problem.Count - 1
                output.Write(problem.Y(i))

                For j = 0 To problem.X(i).Length - 1
                    output.Write(" {0}:{1:0.000000}", problem.X(i)(j).Index, problem.X(i)(j).Value)
                Next

                output.Write(ASCII.LF)
            Next

            output.Flush()
            [Stop]()
        End Sub

        ''' <summary>
        ''' Reads a Problem from a file.
        ''' </summary>
        ''' <param name="filename">The file to read from.</param>
        ''' <returns>the Probem</returns>
        Public Function Read(filename As String) As Problem
            Dim input = File.OpenRead(filename)

            Try
                Return Read(input)
            Finally
                input.Close()
            End Try
        End Function

        ''' <summary>
        ''' Writes a problem to a file.   This will overwrite any previous data in the file.
        ''' </summary>
        ''' <param name="filename">The file to write to</param>
        ''' <param name="problem">The problem to write</param>
        Public Sub Write(filename As String, problem As Problem)
            Dim output = File.Open(filename, FileMode.Create)

            Try
                Write(output, problem)
            Finally
                output.Close()
            End Try
        End Sub
    End Module
End Namespace