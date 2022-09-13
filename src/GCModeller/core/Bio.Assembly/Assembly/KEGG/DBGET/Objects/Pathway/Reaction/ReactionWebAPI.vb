#Region "Microsoft.VisualBasic::dda6e6723c0092f1bba4d3fef35d97a9, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Reaction\ReactionWebAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 149
    '    Code Lines: 91
    ' Comment Lines: 35
    '   Blank Lines: 23
    '     File Size: 5.89 KB


    '     Module ReactionWebAPI
    ' 
    '         Function: Download, DownloadRelatedReactions, FetchTo, idFromInt32
    ' 
    '         Sub: DownloadAllReactions
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.WebQuery

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module ReactionWebAPI

        ''' <summary>
        ''' 使用ID来下载代谢过程的模型数据
        ''' </summary>
        ''' <param name="ID">编号格式为：``R\d+``，例如R00259</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Download(ID As String, Optional cache$ = "./reactions/", Optional sleepTime% = 3000) As Reaction
            Static handlers As New Dictionary(Of String, ReactionQuery)

            Return handlers.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New ReactionQuery(cache, sleepTime)
                           End Function) _
               .Query(Of Reaction)(ID, cacheType:=".html")
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="list"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回成功下载的对象的数目</returns>
        ''' <remarks></remarks>
        Public Function FetchTo(list As String(), EXPORT$) As String()
            Dim failures As New List(Of String)
            Dim r As Reaction

            For Each ID As String In list
                r = Download(ID, cache:=$"{EXPORT}/.reactions/")

                If r Is Nothing Then
                    failures += ID
                Else
                    Call r.GetXml.SaveTo($"{EXPORT}/{ID}.xml")
                End If
            Next

            Return failures.ToArray
        End Function

        ''' <summary>
        ''' 当前的KEGG数据库之中的代谢反应数量
        ''' 
        ''' > https://www.kegg.jp/kegg/docs/statistics.html
        ''' </summary>
        Const MaxReactionCount As Integer = 11271

        ''' <summary>
        ''' Download all of the reaction that related to the given set of compounds.
        ''' 
        ''' 函数返回下载失败的列表
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Sub DownloadAllReactions(EXPORT$, Optional cache$ = "./.reactions/")
            Using progress As New ProgressBar("Download all KEGG reactions...", 1, CLS:=True)
                Dim tick As New ProgressProvider(progress, MaxReactionCount)
                Dim ETA$
                Dim doTick = Sub(cpdName As String)
                                 ETA$ = tick.ETA().FormatTime

                                 Call progress.SetProgress(tick.StepProgress, $"{cpdName}, ETA=" & ETA)
                             End Sub
                Dim count As Integer = 0

                For i As Integer = 0 To 99999
                    Dim reactionID As String = idFromInt32(i)

                    With Download(reactionID, cache:=cache)
                        If .IsNothing Then

                        Else
                            Call .GetXml _
                                 .SaveTo($"{EXPORT}/{reactionID.Last}/{reactionID}.Xml")
                            Call doTick(.CommonNames.FirstOrDefault)

                            count += 1
                        End If
                    End With

                    If count >= MaxReactionCount Then
                        Exit For
                    End If
                Next
            End Using
        End Sub

        Private Function idFromInt32(index As Integer) As String
            Return $"R{index.FormatZero("00000")}"
        End Function

        ''' <summary>
        ''' Download all of the reaction that related to the given set of compounds.
        ''' 
        ''' 函数返回下载失败的列表
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回下载失败的代谢反应过程的编号列表</returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function DownloadRelatedReactions(compounds As IEnumerable(Of Compound), EXPORT$, Optional cache$ = "./.reactions/") As String()
            Dim failures As New List(Of String)
            Dim compoundArray = compounds.ToArray

            Using progress As New ProgressBar("Download compounds related KEGG reactions...", 1, CLS:=True)
                Dim tick As New ProgressProvider(progress, compoundArray.Length)
                Dim ETA$
                Dim doTick = Sub(cpdName As String)
                                 ETA$ = tick.ETA().FormatTime

                                 Call progress.SetProgress(tick.StepProgress, $"{cpdName}, ETA=" & ETA)
                             End Sub

                For Each compound As Compound In compoundArray
                    For Each reactionID As String In compound.reactionId.SafeQuery
                        With Download(reactionID, cache:=cache)
                            If .IsNothing Then
                                failures += reactionID
                            Else
                                Call .GetXml _
                                     .SaveTo($"{EXPORT}/{reactionID.Last}/{reactionID}.Xml")
                            End If
                        End With
                    Next

                    Call doTick(compound.commonNames.FirstOrDefault)
                Next
            End Using

            Return failures
        End Function
    End Module
End Namespace
