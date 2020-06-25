Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text

Namespace Ptf

    ''' <summary>
    ''' the GCModeller protein annotation tabular format file.
    ''' </summary>
    Public Class PtfFile : Implements ISaveHandle

        Public Property proteins As ProteinAnnotation()

        Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Using output As New StreamWriter(path.Open(doClear:=True), encoding) With {
                .NewLine = ASCII.LF
            }
                Call Document.writeTabular(Me, output)
            End Using

            Return True
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace