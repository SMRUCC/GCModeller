Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

''' <summary>
''' 这个仅支持代谢反应过程，即<see cref="PathwayMap.KEGGReaction"/>集合不能够为空
''' </summary>
Public Module PathwayMapVisualize

    ''' <summary>
    ''' 这个仅支持代谢反应过程，即<see cref="PathwayMap.KEGGReaction"/>集合不能够为空
    ''' </summary>
    ''' <param name="ref"></param>
    ''' <param name="reaction"></param>
    ''' <param name="compounds"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildModel(ref As PathwayMap, reaction As IRepositoryRead(Of String, Reaction), compounds As IRepositoryRead(Of String, Compound)) As Network
        If ref.KEGGReaction.IsNullOrEmpty Then
            Return Nothing
        End If


    End Function
End Module
