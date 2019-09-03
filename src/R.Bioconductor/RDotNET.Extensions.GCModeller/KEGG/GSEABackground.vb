Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports SMRUCC.genomics.Assembly.KEGG
Imports VisualBasicNet = Microsoft.VisualBasic.Language.Runtime

Public Module GSEABackground

    ''' <summary>
    ''' 从标准参考图数据中建立通用的富集分析的背景模型
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function SaveBackgroundRda(maps As IEnumerable(Of DBGET.bGetObject.PathwayMap), rdafile$) As Boolean
        Dim backgroundSize%
        Dim terms As New List(Of String)
        Dim background As New var("GSEA.KEGGCompounds", "list()")
        Dim pathways As New var("list()")

        With New VisualBasicNet
            For Each map In maps
                pathways(map.EntryId) = base.list(
                    !id = map.EntryId,
                    !name = map.name,
                    !compounds = map.KEGGCompound _
                        .Select(Function(c) c.name) _
                        .DoCall(Function(list)
                                    Return base.c(list, stringVector:=True)
                                End Function),
                    !ko = map.KEGGOrthology _
                        .EntityList _
                        .DoCall(Function(list)
                                    Return base.c(list, stringVector:=True)
                                End Function)
                )
            Next
        End With

        With background
            !size = backgroundSize
            !pathways = pathways
        End With

        Return base.save({background}, file:=rdafile)
    End Function
End Module
