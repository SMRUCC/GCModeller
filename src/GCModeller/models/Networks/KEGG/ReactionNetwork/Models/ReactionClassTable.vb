#Region "Microsoft.VisualBasic::9daa26224cc1572ec4b04959dbdcd441, GCModeller\models\Networks\KEGG\ReactionNetwork\Models\ReactionClassTable.vb"

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
    '    Code Lines: 57
    ' Comment Lines: 46
    '   Blank Lines: 12
    '     File Size: 4.57 KB


    '     Class ReactionClassTable
    ' 
    '         Properties: category, define, fluxId, rId
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateIndexKey, IndexTable, ReactionIndex, (+2 Overloads) ScanRepository, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Namespace ReactionNetwork

    Public Class ReactionClassTable : Inherits ReactionCompoundTransform

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
        ''' reaction class definition
        ''' </summary>
        ''' <returns></returns>
        Public Property define As String

        Sub New()
        End Sub

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
        ''' <remarks>
        ''' <see cref="fluxId"/>
        ''' 
        ''' 因为在<see cref="ScanRepository(String)"/>加载数据的时候，由于<see cref="ReactionClass.reactantPairs"/>
        ''' 有多个记录值，所以在展开为二维表的时候会出现重复的<see cref="ReactionClassTable.rId"/>
        ''' 不可以去重，因为可能会丢失一部分的物质对信息
        ''' </remarks>
        Public Shared Function ReactionIndex(table As IEnumerable(Of ReactionClassTable)) As Dictionary(Of String, ReactionClassTable())
            Return table _
                .Select(Function(a)
                            Return a.fluxId.Select(Function(rid) (rid, cls:=a))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(a) a.rid) _
                .ToDictionary(Function(a) a.Key,
                              Function(g)
                                  Return g.Select(Function(a) a.cls).ToArray
                                  ' 20200927 see remarks
                                  '
                                  ' .GroupBy(Function(cls) cls.rId) _
                                  ' .Select(Function(a) a.First) _
                                  ' .ToArray
                              End Function)
        End Function

        Public Shared Iterator Function ScanRepository(repo As IEnumerable(Of ReactionClass)) As IEnumerable(Of ReactionClassTable)
            For Each model As ReactionClass In repo
                For Each transform As ReactionCompoundTransform In model.reactantPairs
                    Yield New ReactionClassTable With {
                        .category = model.category,
                        .define = model.definition,
                        .fluxId = model.reactions.Select(Function(r) r.name).ToArray,
                        .from = transform.from,
                        .[to] = transform.to,
                        .rId = model.entryId
                    }
                Next
            Next
        End Function

        ''' <summary>
        ''' read a foder of <see cref="ReactionClass"/>, and then convert to <see cref="ReactionClassTable"/>
        ''' </summary>
        ''' <param name="repo"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ScanRepository(repo As String) As IEnumerable(Of ReactionClassTable)
            Return ScanRepository(ReactionClass.ScanRepository(repo, loadsAll:=False))
        End Function

        Public Shared Function CreateIndexKey(a As String, b As String) As String
            Return {a, b}.OrderBy(Function(s) s).JoinBy("+")
        End Function

    End Class
End Namespace
