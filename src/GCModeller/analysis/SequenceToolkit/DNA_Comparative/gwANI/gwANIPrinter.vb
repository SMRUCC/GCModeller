Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO

Namespace gwANI

    Public Module gwANIPrinter

        <Extension>
        Public Sub print(dataset As DataSet(), out As TextWriter)
            Dim sequence_names$() = dataset.Keys

            Call print_header(out, sequence_names)
            Call print_matrix(out, dataset, sequence_names)
        End Sub

        ''' <summary>
        ''' 打印出列标题
        ''' </summary>
        Private Sub print_header(out As TextWriter, sequence_names$())
            Dim number_of_samples% = sequence_names.Length

            For i As Integer = 0 To number_of_samples - 1
                Call out.Write(vbTab & "{0}", sequence_names(i))
            Next

            Call out.WriteLine()
        End Sub

        Private Sub print_matrix(out As TextWriter, dataset As DataSet(), sequence_names$())
            Dim number_of_samples% = sequence_names.Length
            Dim similarity_percentage As Double()
            Dim id$

            For i As Integer = 0 To number_of_samples - 1
                similarity_percentage = dataset(i)(sequence_names)
                id = sequence_names(i)

                For j As Integer = 0 To number_of_samples - 1
                    If similarity_percentage(j) < 0 Then
                        out.Write(vbTab & "-")
                    Else
                        out.Write(vbTab & "{0:f}", similarity_percentage(j))
                    End If
                Next

                out.Write(vbLf)
            Next
        End Sub
    End Module
End Namespace