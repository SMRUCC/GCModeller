#Region "Microsoft.VisualBasic::959483555f906ed084e03c2b347c4485, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\Compound.vb"

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

Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' Compounds with Biological Roles.(在这里面包含有KEGG compounds的下载API)
    ''' </summary>
    ''' <remarks>
    ''' Compounds
    ''' 
    '''  br08001  Compounds with biological roles
    '''  br08002  Lipids
    '''  br08003  Phytochemical compounds
    '''  br08005  Bioactive peptides
    '''  br08006  Endocrine disrupting compounds
    '''  br08007  Pesticides
    '''  br08008  Carcinogens
    '''  br08009  Natural toxins
    '''  br08010  Target-based classification of compounds
    ''' </remarks>
    Public Class Compound

        Public Property [Class] As String
        Public Property Category As String
        Public Property SubCategory As String
        Public Property Order As String
        Public Property Entry As KeyValuePair

        Private Shared Function Build(Model As BriteHText) As Compound()
            Dim list As New List(Of Compound)

            Select Case Model.Degree

                Case "C"c
                    For Each [Class] As BriteHText In Model.CategoryItems

                        If [Class].CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each category As BriteHText In [Class].CategoryItems
                            If category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If

                            list += From htext As BriteHText
                                    In category.CategoryItems
                                    Select New Compound With {
                                        .Class = [Class].ClassLabel,
                                        .Category = category.ClassLabel,
                                        .Entry = New KeyValuePair With {
                                            .Key = htext.EntryId,
                                            .Value = htext.Description
                                        }
                                    }
                        Next
                    Next

                Case "D"c
                    For Each [Class] As BriteHText In Model.CategoryItems

                        If [Class].CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each category As BriteHText In [Class].CategoryItems
                            If category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If
                            For Each subCategory As BriteHText In category.CategoryItems
                                If subCategory.CategoryItems.IsNullOrEmpty Then
                                    Continue For
                                End If

                                list += From br As BriteHText
                                        In subCategory.CategoryItems
                                        Select New Compound With {
                                            .Class = [Class].ClassLabel,
                                            .Category = category.ClassLabel,
                                            .SubCategory = subCategory.ClassLabel,
                                            .Entry = New KeyValuePair With {
                                                .Key = br.EntryId,
                                                .Value = br.Description
                                            }
                                        }
                            Next
                        Next
                    Next

                Case "E"c
                    For Each [class] As BriteHText In Model.CategoryItems
                        If [class].CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If
                        For Each category As BriteHText In [class].CategoryItems
                            If category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If
                            For Each subCategory As BriteHText In category.CategoryItems
                                If subCategory.CategoryItems.IsNullOrEmpty Then
                                    Continue For
                                End If
                                For Each order As BriteHText In subCategory.CategoryItems
                                    If order.CategoryItems.IsNullOrEmpty Then
                                        Continue For
                                    End If

                                    list += From br As BriteHText
                                            In order.CategoryItems
                                            Select New Compound With {
                                                .Class = [class].ClassLabel,
                                                .Category = category.ClassLabel,
                                                .SubCategory = subCategory.ClassLabel,
                                                .Order = order.ClassLabel,
                                                .Entry = New KeyValuePair With {
                                                    .Key = br.EntryId,
                                                    .Value = br.Description
                                                }
                                            }

                                Next
                            Next
                        Next
                    Next
            End Select

            Return list.ToArray
        End Function

        ''' <summary>
        ''' 请注意，这个函数只能够下载包含有分类信息的化合物，假若代谢物还没有分类信息的话，则无法利用这个函数进行下载
        ''' 
        '''  br08001  Compounds with biological roles
        '''  br08002  Lipids
        '''  br08003  Phytochemical compounds
        '''  br08005  Bioactive peptides
        '''  br08006  Endocrine disrupting compounds
        '''  br08007  Pesticides
        '''  br08008  Carcinogens
        '''  br08009  Natural toxins
        '''  br08010  Target-based classification of compounds
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <param name="DirectoryOrganized"></param>
        ''' <param name="forceUpdate">是否需要API对已经存在的数据进行强制更新？</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DownloadFromResource(EXPORT$,
                                                    Optional DirectoryOrganized As Boolean = True,
                                                    Optional forceUpdate As Boolean = False) As String()
            Dim Resource = {
                New KeyValuePair(Of String, Compound())("Compounds with biological roles", Build(BriteHText.Load(My.Resources.br08001))),
                New KeyValuePair(Of String, Compound())("Lipids", Build(BriteHText.Load(My.Resources.br08002))),
                New KeyValuePair(Of String, Compound())("Phytochemical compounds", Build(BriteHText.Load(My.Resources.br08003))),
                New KeyValuePair(Of String, Compound())("Bioactive peptides", Build(BriteHText.Load(My.Resources.br08005))),
                New KeyValuePair(Of String, Compound())("Endocrine disrupting compounds", Build(BriteHText.Load(My.Resources.br08006))),
                New KeyValuePair(Of String, Compound())("Pesticides", Build(BriteHText.Load(My.Resources.br08007))),
                New KeyValuePair(Of String, Compound())("Carcinogens", Build(BriteHText.Load(My.Resources.br08008))),
                New KeyValuePair(Of String, Compound())("Natural toxins", Build(BriteHText.Load(My.Resources.br08009))),
                New KeyValuePair(Of String, Compound())("Target-based classification of compounds", Build(BriteHText.Load(My.Resources.br08010)))
            }
            Dim failures As New List(Of String)

            For Each briteEntry In Resource
                With briteEntry
                    Call __downloadsInternal(.Key, .Value, failures, EXPORT, DirectoryOrganized, forceUpdate)
                End With
            Next

            Return failures
        End Function

        Private Shared Sub __downloadsInternal(key$, briteEntry As Compound(), ByRef failures As List(Of String), EXPORT$, DirectoryOrganized As Boolean, forceUpdate As Boolean)
            Dim progress As New ProgressBar("Downloads " & key, cls:=True)
            Dim tick As New ProgressProvider(briteEntry.Length)

            ' 2017-3-12
            ' 有些entry的编号是空值？？？
            Dim keys = briteEntry.Where(
                Function(ID)
                    Return (Not ID Is Nothing) AndAlso
                        (Not ID.Entry Is Nothing) AndAlso
                        (Not ID.Entry.Key.StringEmpty)
                End Function)

            For Each entry As Compound In keys
                Dim EntryId As String = entry.Entry.Key
                Dim saveDIR As String = entry.BuildPath(EXPORT, DirectoryOrganized, [class]:=key)
                Dim xml As String = String.Format("{0}/{1}.xml", saveDIR, EntryId)

                If Not forceUpdate AndAlso xml.FileExists(True) Then
                    Continue For
                End If

                If EntryId.First = "G"c Then
                    Dim gl As Glycan = Glycan.Download(EntryId)

                    If gl Is Nothing Then
                        Call $"[{entry.ToString}] is not exists in the kegg!".Warning
                        failures += EntryId
                    Else
                        Call gl.GetXml.SaveTo(xml)
                    End If
                Else
                    Dim cpd As bGetObject.Compound = MetabolitesDBGet.DownloadCompound(EntryId)

                    If cpd Is Nothing Then
                        Call $"[{entry.ToString}] is not exists in the kegg!".Warning
                        failures += EntryId
                    Else
                        Call cpd.GetXml.SaveTo(xml)
                    End If
                End If

                Dim ETA$ = $"ETA={tick.ETA(progress.ElapsedMilliseconds)}"
                Call Thread.Sleep(1000)
                Call progress.SetProgress(tick.StepProgress, detail:=ETA)
            Next

            Call progress.Dispose()
        End Sub

        Public Function BuildPath(EXPORT$, directoryOrganized As Boolean, Optional class$ = "") As String
            With Me
                If directoryOrganized Then
                    Dim t As New List(Of String) From {
                        EXPORT,
                        BriteHText.NormalizePath(.Class),
                        BriteHText.NormalizePath(.Category),
                        BriteHText.NormalizePath(.SubCategory)
                    }

                    If Not [class].StringEmpty Then
                        Call t.Insert(index:=1, item:=[class])
                    End If

                    Return String.Join("/", t)
                Else
                    Return EXPORT
                End If
            End With
        End Function

        ''' <summary>
        ''' 函数返回失败的编号列表
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <param name="BriefFile"></param>
        ''' <param name="DirectoryOrganized"></param>
        ''' <returns></returns>
        Public Shared Function DownloadCompounds(EXPORT$, briefFile$,
                                                 Optional DirectoryOrganized As Boolean = True,
                                                 Optional forceUpdate As Boolean = False) As String()

            Dim BriefEntries As Compound() = LoadFile(briefFile)
            Dim failures As New List(Of String)

            For Each entry As Compound In BriefEntries
                Dim EntryId As String = entry.Entry.Key
                Dim saveDIR As String = entry.BuildPath(EXPORT, DirectoryOrganized)
                Dim xml As String = String.Format("{0}/{1}.xml", saveDIR, EntryId)

                If Not forceUpdate AndAlso xml.FileExists(True) Then
                    Continue For
                End If

                Dim cpd As bGetObject.Compound = MetabolitesDBGet.DownloadCompound(EntryId)

                If cpd Is Nothing Then
                    Call $"[{entry.ToString}] is not exists in the kegg!".Warning
                    failures += EntryId
                Else
                    Call cpd.GetXml.SaveTo(xml)
                End If
            Next

            Return failures
        End Function

        Public Shared Function LoadFile(path As String) As Compound()
            Dim Model = BriteHText.Load(FileIO.FileSystem.ReadAllText(path))
            Return Build(Model)
        End Function
    End Class
End Namespace
