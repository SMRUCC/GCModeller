#Region "Microsoft.VisualBasic::46de3c9ccf949a9f0fad949aebfc3a9e, data\RCSB PDB\PDB\PDB.vb"

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

'   Total Lines: 115
'    Code Lines: 85 (73.91%)
' Comment Lines: 13 (11.30%)
'    - Xml Docs: 92.31%
' 
'   Blank Lines: 17 (14.78%)
'     File Size: 4.38 KB


' Class PDB
' 
'     Properties: AminoAcidSequenceData, AtomStructures, Author, Compound, Experiment
'                 Header, Journal, Keywords, MaxSpace, MinSpace
'                 Remark, Sequence, Source, Title
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: (+2 Overloads) Load
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords

Public Class PDB

    Public Const REGEX_HEAD As String = "[A-Z]+\s+(\d+)?\s"

    Public Property Header As Header
    Public Property Title As Title
    Public Property Compound As Compound
    Public Property Source As Source
    Public Property Keywords As Keywords.Keywords
    Public Property Experiment As ExperimentData
    Public Property Author As Author
    Public Property Journal As Journal
    Public Property Remark As Remark
    Public Property Sequence As Sequence
    Public Property AtomStructures As Atom
        Get
            Return _atomStructuresData
        End Get
        Set(value As Atom)
            _atomStructuresData = value
            _AASeqLoader = New Microsoft.VisualBasic.ComponentModel.LazyLoader(Of AminoAcid(), Atom)(value, AddressOf AminoAcid.SequenceGenerator)
        End Set
    End Property

    Dim _atomStructuresData As Atom
    Dim _AASeqLoader As Microsoft.VisualBasic.ComponentModel.LazyLoader(Of AminoAcid(), Atom)

    Public ReadOnly Property MaxSpace As Keywords.Point3D
        Get
            Dim XLQuery = (From Atom In AtomStructures.Atoms Select Atom.Location.X).ToArray.Max
            Dim YLQuery = (From Atom In AtomStructures.Atoms Select Atom.Location.Y).ToArray.Max
            Dim ZLQuery = (From Atom In AtomStructures.Atoms Select Atom.Location.Z).ToArray.Max
            Return New Point3D With {.X = XLQuery, .Y = YLQuery, .Z = ZLQuery}
        End Get
    End Property

    Public ReadOnly Property MinSpace As Keywords.Point3D
        Get
            Dim XLQuery = (From Atom In AtomStructures.Atoms Select Atom.Location.X).ToArray.Min
            Dim YLQuery = (From Atom In AtomStructures.Atoms Select Atom.Location.Y).ToArray.Min
            Dim ZLQuery = (From Atom In AtomStructures.Atoms Select Atom.Location.Z).ToArray.Min
            Return New Point3D With {.X = XLQuery, .Y = YLQuery, .Z = ZLQuery}
        End Get
    End Property

    ''' <summary>
    ''' 已经经过了排序操作了的
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AminoAcidSequenceData As AminoAcid()
        Get
            Return _AASeqLoader.Value
        End Get
    End Property

    Protected Friend Sub New()
    End Sub

    ''' <summary>
    ''' 加载一个蛋白质的三维空间结构的数据文件
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Load(path As String) As PDB
        Return Load(path.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
    End Function

    Public Shared Function Load(s As Stream) As PDB
        Dim pdb As New PDB
        Dim last As Keyword = Nothing

        For Each line As String In s.ReadAllLines
            Dim data = line.GetTagValue(trim:=True)

            If Not last Is Nothing Then
                If data.Name <> last.Keyword Then
                    last.Flush()
                    last = Nothing
                End If
            End If

            Select Case data.Name
                Case Keyword.KEYWORD_HEADER : pdb.Header = Header.Parse(data.Value)
                Case Keyword.KEYWORD_TITLE : pdb.Title = Title.Append(last, data.Value)
                Case Keyword.KEYWORD_COMPND : pdb.Compound = Compound.Append(last, data.Value)
                Case Keyword.KEYWORD_SOURCE : pdb.Source = Source.Append(last, data.Value)
                Case Keyword.KEYWORD_KEYWDS : pdb.Keywords = RCSB.PDB.Keywords.Keywords.Parse(data.Value)
                Case Keyword.KEYWORD_EXPDTA : pdb.Experiment = ExperimentData.Parse(data.Value)
                Case Keyword.KEYWORD_AUTHOR : pdb.Author = Author.Parse(data.Value)

                Case Else
                    Throw New NotImplementedException(data.Name)
            End Select
        Next

        If Not last Is Nothing Then
            Call last.Flush()
        End If

        Return pdb
    End Function

    Public Overloads Shared Widening Operator CType(path As String) As PDB
        Return PDB.Load(path)
    End Operator
End Class
