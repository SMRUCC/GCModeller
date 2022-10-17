Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans

Namespace ExpressionPattern

    Public Module Writer

        Public Function WriteExpressionPattern(pattern As ExpressionPattern, file As Stream) As Boolean
            Using bin As New BinaryDataWriter(file) With {
                .ByteOrder = ByteOrder.BigEndian
            }
                Call bin.Write(pattern.dim)
                Call bin.Write(pattern.sampleNames.Length)
                Call pattern.sampleNames.ForEach(Sub(name, i) Call bin.Write(name, BinaryStringFormat.ZeroTerminated))

                Call bin.Write(pattern.centers.Length)

                For Each center In pattern.centers
                    Dim buf = WriteCenter(center)

                    Call bin.Write(buf.Length)
                    Call bin.Write(buf)
                Next

                ' save patterns
                Call bin.Write(pattern.Patterns.Length)

                For Each data As FuzzyCMeansEntity In pattern.Patterns
                    Dim buf As Byte() = Classify.GetBuffer(data)

                    Call bin.Write(buf.Length)
                    Call bin.Write(buf)
                Next

                Call bin.Flush()
            End Using

            Return True
        End Function

        Private Function WriteCenter(center As Classify) As Byte()
            Using ms As New MemoryStream, bin As New BinaryDataWriter(ms) With {
                .ByteOrder = ByteOrder.BigEndian
            }
                Call bin.Write(center.Id)
                Call bin.Write(center.center.Length)
                Call bin.Write(center.center)
                Call bin.Write(center.members.Count)

                For Each data As FuzzyCMeansEntity In center.members
                    Dim buf As Byte() = Classify.GetBuffer(data)

                    Call bin.Write(buf.Length)
                    Call bin.Write(buf)
                Next

                Call bin.Flush()

                Return ms.ToArray
            End Using
        End Function
    End Module
End Namespace