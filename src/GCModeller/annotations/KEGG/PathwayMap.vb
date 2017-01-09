Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Module PathwayMapVisualize

    <Extension>
    Public Function BuildModel(ref As PathwayMap, reaction As IRepositoryRead(Of String, Reaction)) As Network

    End Function
End Module
