#Region "Microsoft.VisualBasic::f0bb9c6cb74603beaabbbaa2cb737a5a, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\EnzymaticReaction.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

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
            Dim Model = BriteHText.Load(My.Resources.br08201)
            Return Build(Model)
        End Function

        Private Shared Function Build(Model As BriteHText) As EnzymaticReaction()
            Dim out As New List(Of EnzymaticReaction)

            For Each [class] As BriteHText In Model.CategoryItems
                For Each category As BriteHText In [class].CategoryItems
                    For Each SubCategory As BriteHText In category.CategoryItems

                        If SubCategory.CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each EC As BriteHText In SubCategory.CategoryItems
                            If Not EC.CategoryItems.IsNullOrEmpty Then
                                out += __rxns(EC, [class], category, SubCategory)
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回成功下载的代谢途径的数目</returns>
        ''' <remarks></remarks>
        Public Shared Function DownloadReactions(EXPORT$, Optional briefFile$ = "", Optional directoryOrganized As Boolean = True, Optional [overrides] As Boolean = False) As Integer
            Dim defs As EnzymaticReaction() = If(
                String.IsNullOrEmpty(briefFile),
                LoadFromResource(),
                LoadFile(briefFile))

            For Each rn As EnzymaticReaction In defs
                Dim rnId As String = rn.Entry.Key
                Dim saveDIR As String = If(directoryOrganized, __getDIR(EXPORT, rn), EXPORT)
                Dim xmlFile As String = String.Format("{0}/{1}.xml", saveDIR, rnId)

                If xmlFile.FileLength > 0 Then
                    If Not [overrides] Then
                        Continue For
                    End If
                End If

                Dim rMod As bGetObject.Reaction = bGetObject.Reaction.Download(rnId)

                If rMod Is Nothing Then
                    Call $"[{rn.ToString}] is not exists in the kegg!".__DEBUG_ECHO
                    Call FileIO.FileSystem.WriteAllText(App.HOME & "/DownloadsFailures.log", rn.ToString & vbCrLf, append:=True)
                    Continue For
                End If

                Call rMod.GetXml.SaveTo(xmlFile)
            Next

            Return 0
        End Function

        Private Shared Function __getDIR(outDIR As String, entry As EnzymaticReaction) As String
            Dim [class] As String = __trimInner(entry.Class)
            Dim cat As String = __trimInner(entry.Category)
            Dim subCat As String = __trimInner(entry.SubCategory)

            Return String.Join("/", outDIR, [class], cat, subCat)
        End Function

        Private Shared Function __trimInner(s As String) As String
            Return If(s.Length > 56, Mid(s, 1, 56) & "~", s).Split("\/:*".ToCharArray).JoinBy(" ")
        End Function
    End Class
End Namespace
