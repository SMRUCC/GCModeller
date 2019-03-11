#Region "Microsoft.VisualBasic::65546bc7f4c0d7cf7b3a38e8f3a030d7, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\EnzymaticReaction.vb"

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

    '     Class EnzymaticReaction
    ' 
    '         Properties: [Class], Category, EC, Entry, SubCategory
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __getDIR, __rxns, __source, __trimInner, Build
    '                   DownloadReactions, LoadFile, LoadFromResource, ToString
    ' 
    '         Sub: __downloadInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' Extract data from the <see cref="htext"/> entry model
    ''' </summary>
    Public Class EnzymaticReaction

        ''' <summary>
        ''' level: D
        ''' </summary>
        ''' <returns></returns>
        Public Property EC As String
        ''' <summary>
        ''' level: A
        ''' </summary>
        ''' <returns></returns>
        Public Property [Class] As String
        ''' <summary>
        ''' level: B
        ''' </summary>
        ''' <returns></returns>
        Public Property Category As String
        ''' <summary>
        ''' level: C
        ''' </summary>
        ''' <returns></returns>
        Public Property SubCategory As String
        Public Property Entry As KeyValuePair

        Public Shared Function LoadFromResource() As EnzymaticReaction()
            Dim model As BriteHText = BriteHText.Load(My.Resources.br08201)
            Return Build(model)
        End Function

        Private Shared Function Build(Model As BriteHText) As EnzymaticReaction()
            Dim out As New List(Of EnzymaticReaction)

            For Each [class] As BriteHText In Model.CategoryItems
                For Each category As BriteHText In [class].CategoryItems
                    For Each subCategory As BriteHText In category.CategoryItems

                        If subCategory.CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each EC As BriteHText In subCategory.CategoryItems
                            If Not EC.CategoryItems.IsNullOrEmpty Then
                                out += __rxns(EC, [class], category, subCategory)
                            End If
                        Next
                    Next
                Next
            Next

            Return out.ToArray
        End Function

        Private Shared Function __rxns(EC As BriteHText, [class] As BriteHText, category As BriteHText, subCat As BriteHText) As EnzymaticReaction()
            Dim LQuery = LinqAPI.Exec(Of EnzymaticReaction) <=
 _
                From rxn As BriteHText
                In EC.CategoryItems
                Let erxn As EnzymaticReaction = New EnzymaticReaction With {
                    .EC = EC.ClassLabel,
                    .Category = category.ClassLabel,
                    .Class = [class].ClassLabel,
                    .SubCategory = subCat.ClassLabel,
                    .Entry = New KeyValuePair With {
                        .Key = rxn.EntryId,
                        .Value = rxn.Description
                    }
                }
                Select erxn

            Return LQuery
        End Function

        Public Shared Function LoadFile(path As String) As EnzymaticReaction()
            Return Build(BriteHText.Load(FileIO.FileSystem.ReadAllText(path)))
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}: {1}] {2}", String.Join("/", [Class], Category, SubCategory), EC, Entry.ToString)
        End Function

        Private Shared Function __source(path$) As EnzymaticReaction()
            If Not path.FileLength > 0 Then
                Return LoadFromResource()
            Else
                Return LoadFile(path)
            End If
        End Function

        ''' <summary>
        ''' 函数返回下载失败的列表
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回下载失败的代谢反应过程的编号列表</returns>
        ''' <remarks></remarks>
        Public Shared Function DownloadReactions(EXPORT$, Optional briefFile$ = "", Optional directoryOrganized As Boolean = True, Optional [overrides] As Boolean = False) As String()
            Dim sources As EnzymaticReaction() = __source(briefFile)
            Dim failures As New List(Of String)

            Using progress As New ProgressBar("Download KEGG Reactions...", 1, CLS:=True)
                Dim tick As New ProgressProvider(sources.Length)
                Dim ETA$
                Dim __tick = Sub()
                                 ETA$ = tick _
                                    .ETA(progress.ElapsedMilliseconds) _
                                    .FormatTime
                                 Call progress.SetProgress(tick.StepProgress, "ETA=" & ETA)
                             End Sub

                For Each r As EnzymaticReaction In sources
                    Call __downloadInternal(
                        r, EXPORT, directoryOrganized,
                        [overrides],
                        failures,
                        __tick)
                Next
            End Using

            Return failures
        End Function

        Shared ReadOnly sleepTime% = 2000

        Shared Sub New()
            With App.GetVariable("sleep")
                If Not .StringEmpty Then
                    sleepTime = Val(.ByRef)

                    If sleepTime <= 0 Then
                        sleepTime = 2000
                    End If
                End If
            End With
        End Sub

        Private Shared Sub __downloadInternal(r As EnzymaticReaction,
                                              EXPORT$,
                                              directoryOrganized As Boolean,
                                              [overrides] As Boolean,
                                              failures As List(Of String),
                                              tick As Action)
            Dim rnID As String = r.Entry.Key
            Dim saveDIR As String = If(directoryOrganized, __getDIR(EXPORT, r), EXPORT)
            Dim xmlFile As String = String.Format("{0}/{1}.xml", saveDIR, rnID)

            If Not [overrides] AndAlso xmlFile.FileLength > 0 Then
                GoTo EXIT_LOOP
            End If

            Dim reaction As bGetObject.Reaction = ReactionWebAPI.Download(rnID)

            If reaction Is Nothing Then
                failures += rnID
            Else
                Call reaction.GetXml.SaveTo(xmlFile)
                Call Thread.Sleep(sleepTime)
            End If
EXIT_LOOP:
            Call tick()
        End Sub

        Private Shared Function __getDIR(outDIR As String, entry As EnzymaticReaction) As String
            Dim [class] As String = __trimInner(entry.Class)
            Dim cat As String = __trimInner(entry.Category)
            Dim subCat As String = __trimInner(entry.SubCategory)
            Dim ec As String = __trimInner(entry.EC)

            Return String.Join("/", outDIR, [class], cat, subCat, ec)
        End Function

        Private Shared Function __trimInner(s As String) As String
            Return If(s.Length > 56, Mid(s, 1, 56) & "~", s).Split("\/:*".ToCharArray).JoinBy(" ")
        End Function
    End Class
End Namespace
