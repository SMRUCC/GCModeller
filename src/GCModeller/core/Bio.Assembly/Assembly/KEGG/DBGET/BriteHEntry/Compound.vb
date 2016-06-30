#Region "Microsoft.VisualBasic::d64f8f48461039a2584362f7c67df449, ..\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\Compound.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' Compounds with Biological Roles
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
            Dim CompoundList As New List(Of Compound)

            Select Case Model.Degree

                Case "C"c
                    For Each [Class] As BriteHText In Model.CategoryItems

                        If [Class].CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each Category As BriteHText In [Class].CategoryItems
                            If Category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If

                            CompoundList += From htext As BriteHText
                                            In Category.CategoryItems
                                            Select New Compound With {
                                                .Class = [Class].ClassLabel,
                                                .Category = Category.ClassLabel,
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

                        For Each Category As BriteHText In [Class].CategoryItems
                            If Category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If
                            For Each SubCategory As BriteHText In Category.CategoryItems
                                If SubCategory.CategoryItems.IsNullOrEmpty Then
                                    Continue For
                                End If

                                CompoundList += From item As BriteHText
                                                In SubCategory.CategoryItems
                                                Select New Compound With {
                                                    .Class = [Class].ClassLabel,
                                                    .Category = Category.ClassLabel,
                                                    .SubCategory = SubCategory.ClassLabel,
                                                    .Entry = New KeyValuePair With {
                                                        .Key = item.EntryId,
                                                        .Value = item.Description
                                                    }
                                                }
                            Next
                        Next
                    Next

                Case "E"c
                    For Each [Class] As BriteHText In Model.CategoryItems
                        If [Class].CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If
                        For Each Category As BriteHText In [Class].CategoryItems
                            If Category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If
                            For Each SubCategory As BriteHText In Category.CategoryItems
                                If SubCategory.CategoryItems.IsNullOrEmpty Then
                                    Continue For
                                End If
                                For Each OrderItem In SubCategory.CategoryItems
                                    If OrderItem.CategoryItems.IsNullOrEmpty Then
                                        Continue For
                                    End If

                                    CompoundList += From item
                                                    In OrderItem.CategoryItems
                                                    Select New Compound With {
                                                        .Class = [Class].ClassLabel,
                                                        .Category = Category.ClassLabel,
                                                        .SubCategory = SubCategory.ClassLabel,
                                                        .Order = OrderItem.ClassLabel,
                                                        .Entry = New KeyValuePair With {
                                                            .Key = item.EntryId,
                                                            .Value = item.Description
                                                        }
                                                    }

                                Next
                            Next
                        Next
                    Next
            End Select

            Return CompoundList.ToArray
        End Function

        ''' <summary>
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
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DownloadFromResource(EXPORT As String, Optional DirectoryOrganized As Boolean = True) As Integer
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

            For Each BriteEntry In Resource

                For Each Entry As Compound In BriteEntry.Value
                    Dim EntryId As String = Entry.Entry.Key
                    Dim SaveToDir As String = If(DirectoryOrganized, String.Join("/", EXPORT, BriteEntry.Key, BriteHText.NormalizePath(Entry.Class), BriteHText.NormalizePath(Entry.Category), BriteHText.NormalizePath(Entry.SubCategory)), EXPORT)
                    Dim XmlFile As String = String.Format("{0}/{1}.xml", SaveToDir, EntryId)

                    If FileIO.FileSystem.FileExists(XmlFile) Then
                        If FileIO.FileSystem.GetFileInfo(XmlFile).Length > 0 Then
                            Continue For
                        End If
                    End If

                    If EntryId.First = "G"c Then
                        Dim [Module] = KEGG.DBGET.bGetObject.Glycan.Download(EntryId)

                        If [Module] Is Nothing Then
                            Call Console.WriteLine("[{0}] is not exists in the kegg!", Entry.ToString)
                            Continue For
                        End If

                        Call [Module].GetXml.SaveTo(XmlFile)
                    Else
                        Dim [Module] = KEGG.DBGET.bGetObject.Compound.Download(EntryId)

                        If [Module] Is Nothing Then
                            Call Console.WriteLine("[{0}] is not exists in the kegg!", Entry.ToString)
                            Continue For
                        End If

                        Call [Module].GetXml.SaveTo(XmlFile)
                    End If
                Next
            Next

            Return 0
        End Function

        Public Shared Function DownloadCompounds(Export As String, BriefFile As String, Optional DirectoryOrganized As Boolean = True) As Integer
            Dim BriefEntries = LoadFile(BriefFile)

            For Each Entry As Compound In BriefEntries
                Dim EntryId As String = Entry.Entry.Key
                Dim SaveToDir As String = If(DirectoryOrganized, String.Join("/", Export, BriteHText.NormalizePath(Entry.Class), BriteHText.NormalizePath(Entry.Category), BriteHText.NormalizePath(Entry.SubCategory)), Export)
                Dim XmlFile As String = String.Format("{0}/{1}.xml", SaveToDir, EntryId)

                If FileIO.FileSystem.FileExists(XmlFile) Then
                    If FileIO.FileSystem.GetFileInfo(XmlFile).Length > 0 Then
                        Continue For
                    End If
                End If

                Dim [Module] = KEGG.DBGET.bGetObject.Compound.Download(EntryId)

                If [Module] Is Nothing Then
                    Call Console.WriteLine("[{0}] is not exists in the kegg!", Entry.ToString)
                    Continue For
                End If

                Call [Module].GetXml.SaveTo(XmlFile)
            Next

            Return 0
        End Function

        Public Shared Function LoadFile(path As String) As Compound()
            Dim Model = BriteHText.Load(FileIO.FileSystem.ReadAllText(path))
            Return Build(Model)
        End Function
    End Class
End Namespace
