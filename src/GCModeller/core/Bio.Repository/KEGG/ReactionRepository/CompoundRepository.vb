#Region "Microsoft.VisualBasic::9ab44c0667d5027ea63883bc7430bed9, ..\GCModeller\core\Bio.Repository\KEGG\ReactionRepository\CompoundRepository.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Data.KEGG.Metabolism.RepositoryExtensions

Public Class CompoundRepository : Implements IRepositoryRead(Of String, CompoundIndex)

    Public Property Compounds As CompoundIndex()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return compoundTable.Values.ToArray
        End Get
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Set(value As CompoundIndex())
            compoundTable = value.ToDictionary(Function(c) c.ID)
        End Set
    End Property

    Dim compoundTable As Dictionary(Of String, CompoundIndex)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, CompoundIndex).Exists
        Return compoundTable.ContainsKey(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByKey(key As String) As CompoundIndex Implements IRepositoryRead(Of String, CompoundIndex).GetByKey
        If compoundTable.ContainsKey(key) Then
            Return compoundTable(key)
        Else
            Return Nothing
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetWhere(clause As Func(Of CompoundIndex, Boolean)) As IReadOnlyDictionary(Of String, CompoundIndex) Implements IRepositoryRead(Of String, CompoundIndex).GetWhere
        Return compoundTable.Values.Where(clause).ToDictionary(Function(c) c.ID)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAll() As IReadOnlyDictionary(Of String, CompoundIndex) Implements IRepositoryRead(Of String, CompoundIndex).GetAll
        Return New Dictionary(Of String, CompoundIndex)(compoundTable)
    End Function

    Public Shared Function ScanModels(directory As String, Optional ignoreGlycan As Boolean = True) As CompoundRepository
        Dim table As New Dictionary(Of String, CompoundIndex)

        For Each xml As String In ls - l - r - "*.Xml" <= directory
            Dim compound As Compound

            If xml.BaseName.First = "G"c Then
                If ignoreGlycan Then
                    Continue For
                Else
                    compound = xml.LoadXml(Of Glycan).ToCompound
                End If
            Else
                compound = xml.LoadXml(Of Compound)()
            End If

            If Not table.ContainsKey(compound.Entry) Then
                Dim index As New CompoundIndex With {
                    .Entity = compound,
                    .ID = compound.Entry,
                    .DbTerms = New OrthologyTerms With {
                        .Terms = New List(Of [Property]) From {
                            {TermKeys.KEGG, compound.Entry}
                        }
                    }
                }

                Call table.Add(index.ID, index)
            End If
        Next

        Return New CompoundRepository With {
            .compoundTable = table
        }
    End Function
End Class

Public Class CompoundIndex
    Implements IKeyIndex(Of Compound)
    Implements INamedValue

    <XmlAttribute>
    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property Entity As Compound Implements IKeyIndex(Of Compound).Entity

    <XmlIgnore>
    Public ReadOnly Property Index As Index(Of String) Implements IKeyIndex(Of Compound).Index

    Dim terms As OrthologyTerms

    Public Property DbTerms As OrthologyTerms
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return terms
        End Get
        Set(value As OrthologyTerms)
            terms = value
            _Index = terms _
                .Terms _
                .Select(Function(term) $"{term.name}:{term.value}") _
                .Indexing
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return ID
    End Function
End Class
