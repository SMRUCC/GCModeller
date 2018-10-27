Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language

Public Class PathwayRepository : Inherits XmlDataModel

    Public Property PathwayMaps As PathwayMap()

    Public Shared Function ScanModels(directory As String) As PathwayRepository
        Dim maps As New List(Of PathwayMap)

        For Each file As String In ls - l - r - "*.Xml" <= directory
            maps += file.LoadXml(Of PathwayMap)
        Next

        Return New PathwayRepository With {
            .PathwayMaps = maps
        }
    End Function
End Class
