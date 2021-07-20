Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace PathwayMaps

    ''' <summary>
    ''' assign of the pathway category to compounds/KO by 
    ''' max graph evaluation
    ''' </summary>
    Public Class MaxGraphAsignment

        ReadOnly maps As Map()

        Sub New(maps As IEnumerable(Of Map))
            Me.maps = maps.ToArray
        End Sub

        Public Function AssignMapClass(compounds As IEnumerable(Of String), KO As IEnumerable(Of String)) As IEnumerable(Of NamedValue(Of String))

        End Function

    End Class
End Namespace