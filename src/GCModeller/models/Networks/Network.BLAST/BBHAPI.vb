#Region "Microsoft.VisualBasic::a1ee291cdae10dbf4bb6e39929454ba1, Network.BLAST\BBHAPI.vb"

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

    ' Module BBHAPI
    ' 
    ' 
    '     Enum BHTypes
    ' 
    '         BBH, SBH
    ' 
    ' 
    ' 
    '     Class BBH
    ' 
    '         Properties: Direction
    ' 
    '     Class ___bbhThread
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __partionThreadsBBH, __toSBH, InvokeGet
    ' 
    '  
    ' 
    '     Function: BBHHits
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Public Module BBHAPI

    Public Enum BHTypes
        SBH
        BBH
    End Enum

    Public Class BBH : Inherits BiDirectionalBesthit

        Public Property Direction As BHTypes

    End Class

    <Extension> Private Function __toSBH(sbh As BestHit) As BBH
        Return New BBH With {
            .Direction = BHTypes.SBH,
            .HitName = sbh.HitName,
            .forward = sbh.identities,
            .reverse = sbh.identities,
            .Length = sbh.query_length,
            .Positive = sbh.Positive,
            .QueryName = sbh.QueryName
        }
    End Function

    Private Function __partionThreadsBBH(partition As List(Of BestHit), threadPool As ___bbhThread) As BBH()
        Dim sw As Stopwatch = Stopwatch.StartNew

        Call "Thread started!".__DEBUG_ECHO

        Do While partition.Count > 0
            Dim hit As BestHit = partition(Scan0) ' query <-> hit
            Call partition.RemoveAt(Scan0)

            If threadPool.BBHHash.ContainsKey(hit.BBH_ID) Then  ' 由于是并行化的，所以假设其他的线程已经创建了的话则直接下一条记录
                Continue Do
            End If

            If Not threadPool.SBHHash.ContainsKey(hit.HitName) Then   ' hit <-> query   
                Dim sbh = hit.__toSBH
                Call threadPool.BBHHash.Add(sbh.BBH_ID, sbh)

                Continue Do   ' 
            End If

            Dim hits = threadPool.SBHHash(hit.HitName)
            If Not hits.ContainsKey(hit.QueryName) Then
                Dim sbh = hit.__toSBH
                Call threadPool.BBHHash.Add(sbh.BBH_ID, sbh)

                Continue Do
            End If

            Dim querys = hits(hit.QueryName)
            Dim queryPaired = querys.First

            Call querys.RemoveAt(Scan0)
            If querys.Count = 0 Then
                Call hits.Remove(hit.QueryName)

                If hits.Count = 0 Then
                    Call threadPool.SBHHash.Remove(hit.HitName)
                End If
            End If

            Dim bbhHit As New BBH With {
                .HitName = hit.HitName,
                .QueryName = hit.QueryName,
                .forward = hit.identities,
                .reverse = queryPaired.identities,
                .Length = hit.query_length,
                .Positive = (hit.Positive + queryPaired.Positive) / 2,
                .Direction = BHTypes.BBH
            }

            If Not threadPool.BBHHash.ContainsKey(bbhHit.BBH_ID) Then
                Call threadPool.BBHHash.Add(bbhHit.BBH_ID, bbhHit)
            End If

            Call partition.Remove(queryPaired)
        Loop

        Call $"Thread completed! .....{sw.ElapsedMilliseconds / 1000}sec".__DEBUG_ECHO

        Return New BBH() {}
    End Function

    Private Class ___bbhThread

        Public ReadOnly BBHHash As Dictionary(Of String, BBH)
        Public ReadOnly SBHHash As Dictionary(Of String, Dictionary(Of String, List(Of BestHit)))

        Sub New(sbh As Generic.IEnumerable(Of BestHit))
            BBHHash = New Dictionary(Of String, BBH)
            SBHHash = (From x As BestHit In sbh
                       Select x
                       Group x By x.QueryName Into Group) _
                            .ToDictionary(Function(x) x.QueryName,
                                          Function(x) (From xx As BestHit In x.Group
                                                       Select xx
                                                       Group xx By xx.HitName Into Group) _
                                                            .ToDictionary(Function(xx) xx.HitName,
                                                                          Function(xx) xx.Group.AsList))

            Call "Dictionary hash table created done!".__DEBUG_ECHO
            partitions = sbh.Split(102400).Select(Function(x) x.AsList).ToArray
            Call $"{partitions.Length} partitions...".__DEBUG_ECHO
        End Sub

        Dim partitions As List(Of BestHit)()

        Public Function InvokeGet() As BBH()
            Using busy As New CBusyIndicator(start:=True)
                Dim threads = (From x As List(Of BestHit)
                               In partitions.AsParallel
                               Let handle As Func(Of BBH()) = Function() BBHAPI.__partionThreadsBBH(x, Me)
                               Select New AsyncHandle(Of BBH())(handle).GetValue).ToArray
                Return BBHHash.Values.ToArray
            End Using
        End Function
    End Class

    ''' <summary>
    ''' 由于在这里并不知道基因组的情况，所以在这里只是简单的判断是否双向比对上了
    ''' </summary>
    ''' <param name="sbh"></param>
    ''' <returns></returns>
    Public Function BBHHits(sbh As IEnumerable(Of BestHit)) As BBH()
        Dim threadPool As New ___bbhThread(sbh)
        Return threadPool.InvokeGet
    End Function
End Module
