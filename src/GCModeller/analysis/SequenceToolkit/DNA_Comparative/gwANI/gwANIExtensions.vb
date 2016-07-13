Imports System.IO
Imports System.Text

Namespace gwANI

    Public Module gwANIExtensions

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="[in]">必须要是经过多序列比对对齐了的序列</param>
        ''' <param name="out"></param>
        ''' <param name="fast"></param>
        Public Sub Evaluate([in] As String, out As String, Optional fast As Boolean = True)
            Call "".SaveTo(out, Encoding.ASCII)

            Dim file As New FileStream(out, FileMode.OpenOrCreate)
            Dim write As New StreamWriter(file)

            If fast Then
                Call gwANI.fast_calculate_gwani([in], write)
            Else
                Call gwANI.calculate_and_output_gwani([in], write)
            End If
        End Sub
    End Module
End Namespace