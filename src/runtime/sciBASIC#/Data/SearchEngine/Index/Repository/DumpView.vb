Imports System.IO
Imports System.Text

Public Module DumpView

    ReadOnly DataPadLength As Integer = Long.MinValue.ToString.Length

    Public Sub IndexDumpView(dbFile As String, out As StreamWriter)
        Using index As New BinaryDataReader(dbFile.Open(FileMode.Open, doClear:=False), Encoding.ASCII)
            ' magic
            Call out.Write(index.ReadString(length:=9).PadRight(DataPadLength))
            Call out.Write("| ")

            For c As Integer = Asc(" "c) To Asc("~"c)
                Call out.Write(" ")
                Call out.Write(Chr(c))
            Next

            Call out.WriteLine()

            For i As Integer = 0 To Integer.MaxValue
                ' data value
                Call out.Write(index.ReadInt64.ToString.PadLeft(DataPadLength))
                Call out.Write("| ")

                ' characters offset
                For c As Integer = Asc(" "c) To Asc("~"c)
                    Call out.Write(" ")
                    Call out.Write(index.ReadInt32)
                Next

                ' ZERO terminated flag
                Call out.Write(" ")
                Call out.Write(index.ReadInt32)

                If index.EndOfStream Then
                    Exit For
                Else
                    Call out.WriteLine()
                    Call out.Flush()
                End If
            Next

            Call out.Flush()
        End Using
    End Sub
End Module
