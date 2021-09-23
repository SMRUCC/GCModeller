﻿#Region "Microsoft.VisualBasic::3213a6ff520a8989da2eaf1266233ed1, core\Bio.Assembly\ComponentModel\Annotation\Profiles\CatalogProfiles.vb"

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

'     Class CatalogProfiles
' 
'         Properties: catalogs, Keys, MaxValue, TotalTerms
' 
'         Constructor: (+4 Overloads) Sub New
'         Function: delete, GetProfiles, haveCategory, OrderByValues, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    Public Class CatalogProfiles

        Public Property catalogs As New Dictionary(Of String, CatalogProfile)

        Default Public ReadOnly Property GetCatalogValue(name As String) As CatalogProfile
            Get
                Return catalogs.TryGetValue(name)
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String)
            Get
                Return catalogs.Keys
            End Get
        End Property

        Public ReadOnly Property TotalTerms As Integer
            Get
                Return Aggregate cat As CatalogProfile In catalogs.Values Into Sum(cat.profile.Count)
            End Get
        End Property

        Public ReadOnly Property MaxValue As Double
            Get
                Return Aggregate cat As CatalogProfile
                       In catalogs.Values
                       Let v As Double = If(cat.profile.Count = 0, 0, cat.profile.Max(Function(n) n.Value))
                       Into Max(v)
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(data As Dictionary(Of String, NamedValue(Of Double)()))
            For Each cat As KeyValuePair(Of String, NamedValue(Of Double)()) In data
                catalogs(cat.Key) = New CatalogProfile(DirectCast(cat.Value, IEnumerable(Of NamedValue(Of Double))))
            Next
        End Sub

        Sub New(data As Dictionary(Of String, NamedValue(Of Integer)()))
            For Each cat As KeyValuePair(Of String, NamedValue(Of Integer)()) In data
                catalogs(cat.Key) = New CatalogProfile(cat.Value)
            Next
        End Sub

        Sub New(copy As CatalogProfiles)
            For Each cat As KeyValuePair(Of String, CatalogProfile) In copy.catalogs
                catalogs(cat.Key) = New CatalogProfile(cat.Value)
            Next
        End Sub

        Public Function OrderByValues() As CatalogProfiles
            Return New CatalogProfiles With {
                .catalogs = catalogs _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Value.OrderByValues
                                  End Function)
            }
        End Function

        Public Function Take(topN As Integer) As CatalogProfiles
            Return New CatalogProfiles With {
                .catalogs = catalogs _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Value.Take(topN)
                                  End Function)
            }
        End Function

        ''' <summary>
        ''' removes item from <see cref="catalogs"/> via a given key 
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function delete(name As String) As CatalogProfiles
            If catalogs.ContainsKey(name) Then
                Call catalogs.Remove(name)
            End If

            Return Me
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function haveCategory(name As String) As Boolean
            Return catalogs.ContainsKey(name)
        End Function

        Public Iterator Function GetProfiles() As IEnumerable(Of NamedValue(Of CatalogProfile))
            For Each catalog In catalogs
                Yield New NamedValue(Of CatalogProfile) With {
                    .Name = catalog.Key,
                    .Value = catalog.Value
                }
            Next
        End Function

        Public Overrides Function ToString() As String
            Return catalogs.Keys.GetJson
        End Function

    End Class
End Namespace
