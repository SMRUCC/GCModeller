Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Namespace LDM

    ''' <summary>
    ''' blast网络
    ''' </summary>
    Public Class BLAST : Implements ISaveHandle

        Public Property Proteins As Protein()
        Public Property BlastHits As Hit()

        Public Function Save(Optional DIR As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            Try
                Call Proteins.SaveTo(DIR & "/Proteins.Csv")
                Call BlastHits.SaveTo(DIR & "/Hits.Csv")

                Return True
            Catch ex As Exception
                Call App.LogException(New Exception(DIR, ex))
            End Try

            Return False
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.GetEncodings)
        End Function
    End Class
End Namespace