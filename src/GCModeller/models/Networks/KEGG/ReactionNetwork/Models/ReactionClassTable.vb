#Region "Microsoft.VisualBasic::ace4a7983048a7b49af23affc7f50008, models\Networks\KEGG\ReactionNetwork\Models\ReactionClassTable.vb"

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

'     Class ReactionClassTable
' 
'         Properties: [from], [to], category, define, rId
' 
'         Function: CreateIndexKey, IndexTable
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace ReactionNetwork

    Public Class ReactionClassTable

        ''' <summary>
        ''' entry id of current reaction class
        ''' </summary>
        ''' <returns></returns>
        Public Property rId As String
        ''' <summary>
        ''' the reaction class
        ''' </summary>
        ''' <returns></returns>
        Public Property category As String
        ''' <summary>
        ''' a list of kegg reaction id which are belongs to
        ''' current reaction class data model.
        ''' </summary>
        ''' <returns></returns>
        Public Property fluxId As String()
        ''' <summary>
        ''' compound id
        ''' </summary>
        ''' <returns></returns>
        Public Property [from] As String
        ''' <summary>
        ''' compound id
        ''' </summary>
        ''' <returns></returns>
        Public Property [to] As String
        ''' <summary>
        ''' reaction class definition
        ''' </summary>
        ''' <returns></returns>
        Public Property define As String

        Public Overrides Function ToString() As String
            Return define
        End Function

        ''' <summary>
        ''' create a compound index 
        ''' </summary>
        ''' <param name="table"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <see cref="from"/> and <see cref="[to]"/>
        ''' </remarks>
        Public Shared Function IndexTable(table As IEnumerable(Of ReactionClassTable)) As Index(Of String)
            Return table _
                .Select(Function(r) CreateIndexKey(r.from, r.to)) _
                .GroupBy(Function(key) key) _
                .Keys _
                .Indexing
        End Function

        ''' <summary>
        ''' create a reaction index
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks><see cref="fluxId"/></remarks>
        Public Shared Function ReactionIndex(table As IEnumerable(Of ReactionClassTable)) As Dictionary(Of String, ReactionClassTable())
            Return table _
                .Select(Function(a)
                            Return a.fluxId.Select(Function(rid) (rid, cls:=a))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(a) a.rid) _
                .ToDictionary(Function(a) a.Key,
                              Function(g)
                                  Return g _
                                     .Select(Function(a) a.cls) _
                                     .GroupBy(Function(cls) cls.rId) _
                                     .Select(Function(a) a.First) _
                                     .ToArray
                              End Function)
        End Function

        Public Shared Function CreateIndexKey(a As String, b As String) As String
            Return {a, b}.OrderBy(Function(s) s).JoinBy("+")
        End Function

    End Class
End Namespace
