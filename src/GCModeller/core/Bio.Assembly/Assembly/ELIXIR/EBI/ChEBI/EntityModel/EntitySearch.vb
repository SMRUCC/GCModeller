#Region "Microsoft.VisualBasic::74dd17ecd563b2bd16bb70718809a709, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\EntityModel\EntitySearch.vb"

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

    '   Total Lines: 112
    '    Code Lines: 74
    ' Comment Lines: 24
    '   Blank Lines: 14
    '     File Size: 4.44 KB


    '     Module EntitySearch
    ' 
    '         Function: EnumerateNames, NameEqualsAny, SearchByNameAndFormula, SearchByNameEqualsAny
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Levenshtein
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML

Namespace Assembly.ELIXIR.EBI.ChEBI

    Public Module EntitySearch

        ''' <summary>
        ''' 返回的结果之中的名称为chebi的数字主编号
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <param name="name$">模糊查找</param>
        ''' <param name="formula$">直接使用字符串相等</param>
        ''' <returns></returns>
        <Extension>
        Public Function SearchByNameAndFormula(chebi As Dictionary(Of Long, ChEBIEntity),
                                               name$,
                                               formula$,
                                               Optional parallel As Boolean = True) As IEnumerable(Of NamedCollection(Of DistResult))

            Dim source As IEnumerable(Of ChEBIEntity)
            Dim nameString As New LevenshteinString(name.ToLower)
            Dim matchFormula = From metabolite As ChEBIEntity
                               In chebi.Values
                               Where (Not metabolite.Formulae Is Nothing AndAlso
                                   metabolite.Formulae.data.TextEquals(formula))

            If parallel Then
                source = matchFormula.AsParallel
            Else
                source = matchFormula
            End If

            Dim LQuery = From metabolite As ChEBIEntity
                         In source
                         Let matches = metabolite _
                             .EnumerateNames _
                             .Select(Function(s) nameString Like LCase(s)) _
                             .Where(Function(m) Not m Is Nothing) _
                             .ToArray
                         Where Not matches.IsNullOrEmpty
                         Select New NamedCollection(Of DistResult) With {
                             .name = metabolite.chebiId.Split(":"c).Last,
                             .value = matches
                         }

            Return LQuery
        End Function

        ''' <summary>
        ''' 进行<see cref="ChEBIEntity.chebiAsciiName"/>和他的同义词的精确匹配
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <param name="name$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SearchByNameEqualsAny(chebi As Dictionary(Of Long, ChEBIEntity), name$) As ChEBIEntity
            For Each metabolite As ChEBIEntity In chebi.Values
                If metabolite.NameEqualsAny(name) Then
                    Return metabolite
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 枚举出这个chebi化合物之中的所有的化合物名称
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <returns></returns>
        <Extension> Public Function EnumerateNames(chebi As ChEBIEntity) As String()
            Dim list As New List(Of String)

            list += chebi.chebiAsciiName
            list += chebi.Synonyms.SafeQuery.Select(Function(s) s.data)
            list += chebi.IupacNames.SafeQuery.Select(Function(s) s.data)

            Return list
        End Function

        ''' <summary>
        ''' 进行不区分大小写的字符串精确匹配
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <param name="name$"></param>
        ''' <returns></returns>
        <Extension> Public Function NameEqualsAny(chebi As ChEBIEntity, name$) As Boolean
            If chebi.chebiAsciiName.TextEquals(name) Then
                Return True
            Else
                For Each s As Synonyms In chebi.Synonyms.SafeQuery
                    If s.data.TextEquals(name) Then
                        Return True
                    End If
                Next
                For Each s As Synonyms In chebi.IupacNames.SafeQuery
                    If s.data.TextEquals(name) Then
                        Return True
                    End If
                Next
            End If

            Return False
        End Function
    End Module
End Namespace
