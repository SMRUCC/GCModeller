#Region "Microsoft.VisualBasic::35f0f57f8f8f22871f4084321843d3c6, core\Bio.Assembly\Assembly\ELIXIR\UniProt\XML\GetID.vb"

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

'     Module GetIDs
' 
' 
'         Enum IDTypes
' 
'             Accession, EMBL, KEGG, LocusTag, ORF
'             RefSeq
' 
' 
' 
'  
' 
'     Function: EnumerateParsers, (+2 Overloads) GetID, IdMapping, ParseType
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.Uniprot.XML

    Public Module GetIDs

        Public Enum IDTypes As Integer
            NA = -1

            ''' <summary>
            ''' Uniprot accession ID
            ''' </summary>
            Accession
            ORF
            LocusTag
            RefSeq
            KEGG
            EMBL
        End Enum

        ''' <summary>
        ''' 名字是小写的
        ''' </summary>
        Dim parser As New MapsHelper(Of IDTypes)(map:=EnumParser(Of IDTypes)(), [default]:=IDTypes.Accession)

        Public Iterator Function EnumerateParsers() As IEnumerable(Of Map(Of IDTypes, Func(Of entry, String)))
            For Each type As IDTypes In [Enums](Of IDTypes)().Where(Function(t) t <> IDTypes.NA)
                Yield New Map(Of IDTypes, Func(Of entry, String)) With {
                    .Key = type,
                    .Maps = .Key.GetID()
                }
            Next
        End Function

        Public Function ParseType(type$) As IDTypes
            Return parser(LCase(type))
        End Function

        <Extension>
        Public Function GetID(type As IDTypes) As Func(Of entry, String)
            Select Case type
                Case IDTypes.Accession
                    Return Function(prot As entry)
                               Return DirectCast(prot, INamedValue).Key
                           End Function
                Case IDTypes.EMBL
                    Return Function(prot As entry)
                               If prot.xrefs.ContainsKey(NameOf(IDTypes.EMBL)) Then
                                   Return prot.xrefs(NameOf(IDTypes.EMBL)) _
                                       .First _
                                       .properties _
                                       .Where(Function(p) p.type = "protein sequence ID") _
                                       .FirstOrDefault?.value
                               Else
                                   Return Nothing
                               End If
                           End Function
                Case IDTypes.KEGG
                    Return Function(prot As entry)
                               If prot.xrefs.ContainsKey(NameOf(IDTypes.KEGG)) Then
                                   Return prot.xrefs(NameOf(IDTypes.KEGG)).FirstOrDefault?.id
                               Else
                                   Return Nothing
                               End If
                           End Function
                Case IDTypes.LocusTag
                    Return Function(prot As entry)
                               If prot.gene Is Nothing Then
                                   Return Nothing
                               Else
                                   Return prot _
                                       .gene("ordered locus") _
                                       .DefaultFirst
                               End If
                           End Function
                Case IDTypes.ORF
                    Return Function(prot As entry)
                               Return prot.gene?.ORF?.FirstOrDefault
                           End Function
                Case IDTypes.RefSeq
                    Return Function(prot As entry)
                               If prot.xrefs.ContainsKey(NameOf(IDTypes.RefSeq)) Then
                                   Return prot.xrefs(NameOf(IDTypes.RefSeq)).FirstOrDefault?.id
                               Else
                                   Return Nothing
                               End If
                           End Function
                Case Else
                    Throw New NotImplementedException(type.ToString)
            End Select
        End Function

        <Extension>
        Public Function GetID(type$) As Func(Of entry, String)
            Return parser(LCase(type)).GetID
        End Function

        <Extension>
        Private Function mapAnyIDs(entryList As IEnumerable(Of entry)) As Dictionary(Of String, String)
            Dim index As New Dictionary(Of String, String)

            ' map any to uniprot id
            For Each entry As entry In entryList
                Dim unifyId As String = entry.accessions(Scan0)

                For Each id As String In entry.EnumerateAllIDs.Select(Function(i) i.xrefID)
                    index(id) = unifyId
                Next
            Next

            Return index
        End Function

        <Extension>
        Private Function mapTargetIDs(entryList As IEnumerable(Of entry), target As String) As Dictionary(Of String, String)
            Dim pattern As String = Nothing
            Dim index As New Dictionary(Of String, String)

            If target.IndexOf(":"c) > -1 Then
                With target.GetTagValue(":", trim:=True)
                    target = .Name
                    pattern = .Value
                End With
            End If

            ' map any to target database
            For Each entry As entry In entryList
                Dim unifyId As String
                Dim allEntry = entry.EnumerateAllIDs.ToArray

                If Not pattern Is Nothing Then
                    unifyId = allEntry _
                        .Where(Function(ref) ref.Database = target) _
                        .Where(Function(ref) ref.xrefID.IsPattern(pattern)) _
                        .FirstOrDefault _
                        .xrefID
                Else
                    unifyId = allEntry _
                        .Where(Function(ref) ref.Database = target) _
                        .DefaultFirst _
                        .xrefID
                End If

                If Not unifyId.StringEmpty Then
                    For Each id As String In allEntry.Select(Function(i) i.xrefID)
                        index(id) = unifyId
                    Next
                End If
            Next

            Return index
        End Function

        <Extension>
        Public Function IdMapping(entryList As IEnumerable(Of entry), Optional target As String = Nothing) As Func(Of String, String)
            Dim index As Dictionary(Of String, String)

            If Not target.StringEmpty Then
                index = entryList.mapTargetIDs(target)
            Else
                index = entryList.mapAnyIDs
            End If

            Return Function(anyId) index.TryGetValue(anyId, [default]:=anyId)
        End Function
    End Module
End Namespace
