Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST

Namespace Colors

    ''' <summary>
    ''' A helper for assign color of myva cog result
    ''' </summary>
    Friend Class MvyaColorProfile

        ReadOnly colors As Dictionary(Of String, String)
        ReadOnly defaultColor As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myvaCOGs"></param>
        ''' <param name="profiles">Color profiles</param>
        Sub New(myvaCOGs As MyvaCOG(), profiles As Dictionary(Of String, String), defaultColor As String)
            Me.colors = New Dictionary(Of String, String)
            Me.defaultColor = defaultColor

            For Each line As MyvaCOG In myvaCOGs
                If String.IsNullOrEmpty(line.COG) Then
                    Call colors.Add(line.QueryName, defaultColor)
                Else
                    Call colors.Add(line.QueryName, profiles(line.COG.ToUpper))
                End If
            Next
        End Sub

        Public Function GetColor(geneId As String) As String
            If Not String.IsNullOrEmpty(geneId) AndAlso colors.ContainsKey(geneId) Then
                Return colors(geneId)
            Else
                Return defaultColor
            End If
        End Function
    End Class
End Namespace