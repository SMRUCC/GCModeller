#Region "Microsoft.VisualBasic::c1cbbfae663841aa6c6789a24e3b20da, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\Pathways.vb"

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

    '   Total Lines: 124
    '    Code Lines: 84
    ' Comment Lines: 16
    '   Blank Lines: 24
    '     File Size: 5.02 KB


    '     Module Pathways
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AllEntries, Downloads
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace Assembly.KEGG.DBGET.LinkDB

    ''' <summary>
    ''' LinkDB Search for KEGG pathways
    ''' </summary>
    Public Module Pathways

        ReadOnly sleep% = 2500

        Sub New()
            With App.GetVariable("/sleep")
                If Not .StringEmpty Then
                    sleep = Val(.ByRef)
                End If

                If sleep <= 0 Then
                    sleep = 2500
                End If
            End With
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="code">The species code in KEGG database</param>
        ''' <returns></returns>
        Public Function AllEntries(code$, Optional cache$ = "./.kegg/pathways/", Optional offline As Boolean = False) As ListEntry()
            Static handlers As New Dictionary(Of String, PathwayEntryQuery)

            Dim query As PathwayEntryQuery = handlers.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New PathwayEntryQuery(cache, interval:=sleep, offline:=offline)
                           End Function)

            Return query.Query(Of ListEntry())(code, ".html")
        End Function

        ''' <summary>
        ''' 下载某一个物种所注释的代谢途径的数据
        ''' </summary>
        ''' <param name="sp"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns></returns>
        Public Function Downloads(sp$,
                                  Optional EXPORT$ = "./LinkDB-pathways/",
                                  Optional cache$ = "./.kegg/pathways/",
                                  Optional offline As Boolean = False,
                                  Optional ignoresDbKeyNotFound As Boolean = False) As String()

            Dim entries As New List(Of ListEntry)
            Dim briteTable As Dictionary(Of String, BriteHEntry.Pathway) = BriteHEntry.Pathway.LoadDictionary
            Dim progress As New ProgressBar("KEGG LinkDB Downloads KEGG Pathways....", 1, CLS:=True)
            Dim failures As New List(Of String)

            ' VBDebugger.Mute = True

            Dim all As ListEntry() = AllEntries(sp, cache, offline:=offline).ToArray
            Dim url$
            Dim i As i32 = 1
            Dim hitCache As Boolean = False

            Static handlers As New Dictionary(Of String, PathwayMapDownloader)

            Dim query As PathwayMapDownloader = handlers.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New PathwayMapDownloader(cache, interval:=sleep, offline:=offline)
                           End Function)

            For Each entry As ListEntry In all
                Dim imageUrl = String.Format("http://www.genome.jp/kegg/pathway/{0}/{1}.png", sp, entry.entryId)
                Dim img As String = EXPORT & $"/maps/{entry.entryId}.png"
                Dim bCode As String = Regex.Match(entry.entryId, "\d+").Value
                Dim xml$
                Dim data As Pathway = query.Query(Of Pathway)(entry, ".html", hitCache)

                If briteTable.ContainsKey(bCode) Then
                    xml = $"{EXPORT}/{briteTable(bCode).GetPathCategory}/{entry.entryId}.Xml"
                ElseIf ignoresDbKeyNotFound Then
                    Continue For
                Else
                    Throw New Exception($"Pathway Map class information for '{entry.entryId}' not found! Please consider update GCModeller to latest version...")
                End If

                If img.FileLength < 1024 Then
                    Call imageUrl.DownloadFile(img)
                End If

                If data Is Nothing Then
                    failures += entry.entryId
                Else
                    entries += entry
                    url = $"http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+path:{entry.entryId}"
                    data.genes = url.LinkDbEntries(cacheRoot:=$"{cache}/linkdb/", offline:=offline).ToArray

                    Call data.SaveAsXml(xml)
                End If

                If Not hitCache Then
                    Call Thread.Sleep(sleep)
                End If

EXIT_LOOP:      Call progress.SetProgress(++i / all.Length * 100, entry.GetJson)
            Next

            ' VBDebugger.Mute = False

            Call progress.Dispose()
            Call entries.GetJson.SaveTo(EXPORT & $"/{sp}.json")

            Return failures
        End Function
    End Module
End Namespace
