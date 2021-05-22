'#Region "Microsoft.VisualBasic::cd1ef7f6eb41570e9d5b0121f604e11e, DataMySql\kb_UniProtKB\UniprotKBEngine.vb"

'' Author:
'' 
''       asuka (amethyst.asuka@gcmodeller.org)
''       xie (genetics@smrucc.org)
''       xieguigang (xie.guigang@live.com)
'' 
'' Copyright (c) 2018 GPL3 Licensed
'' 
'' 
'' GNU GENERAL PUBLIC LICENSE (GPL3)
'' 
'' 
'' This program is free software: you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation, either version 3 of the License, or
'' (at your option) any later version.
'' 
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
'' 
'' You should have received a copy of the GNU General Public License
'' along with this program. If not, see <http://www.gnu.org/licenses/>.



'' /********************************************************************************/

'' Summaries:

''     Module UniprotKBEngine
'' 
''         Properties: DbName
'' 
''         Function: GetGOTable, GetHashCode, GetKOTable
'' 
'' 
'' /********************************************************************************/

'#End Region

'Imports System.Runtime.CompilerServices
'Imports Oracle.LinuxCompatibility.MySQL
'Imports Oracle.LinuxCompatibility.MySQL.Expressions
'Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema
'Imports mysqli = Oracle.LinuxCompatibility.MySQL.MySqli

Imports Oracle.LinuxCompatibility.MySQL.Reflection.Schema

Namespace kb_UniProtKB

    ''' <summary>
    ''' 使用Uniprot的mysql知识库进行蛋白注释的获取的引擎
    ''' </summary>
    Public Module UniprotKBEngine

        ''' <summary>
        ''' ``kb_uniprotkb``
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property DbName As String = New Table(GetType(mysql.hash_table)).Database

        '        ''' <summary>
        '        ''' 获取得到哈希码，然后应用于快读的查询其他的蛋白数据
        '        ''' </summary>
        '        ''' <param name="id"></param>
        '        ''' <param name="mysql"></param>
        '        ''' <param name="chunkSize%"></param>
        '        ''' <returns></returns>
        '        <Extension>
        '        Public Function GetHashCode(id As IEnumerable(Of String), mysql As mysqli, Optional chunkSize% = 3000) As Dictionary(Of String, Long)
        '            Dim model As New Table(Of mysql.hash_table)(mysql)
        '            Dim targets$() = id _
        '                .Split(chunkSize) _
        '                .Select(Function(chunk) chunk.JoinBy(", ")) _
        '                .ToArray
        '            Dim hashCode As New Dictionary(Of String, Long)

        '            For Each chunk As String In targets
        '                Dim buffer As mysql.hash_table() = model _
        '                    .Where($"{NameOf(kb_UniProtKB.mysql.hash_table.uniprot_id)} IN ( {chunk} )") _
        '                    .SelectALL

        '                For Each uniprotID As mysql.hash_table In buffer
        '                    With uniprotID
        '                        Call hashCode.Add(.uniprot_id, .hash_code)
        '                    End With
        '                Next
        '            Next

        '            Return hashCode
        '        End Function

        '        <Extension>
        '        Public Function GetKOTable(id As IEnumerable(Of String), mysql As mysqli, Optional chunkSize% = 3000) As Dictionary(Of String, String)
        '            Dim hashcodes = id.GetHashCode(mysql, chunkSize)
        '            Dim model As New Table(Of mysql.protein_ko)(mysql)
        '            Dim targets = hashcodes.Values _
        '                .Split(chunkSize) _
        '                .Select(Function(chunk) chunk.JoinBy(", ")) _
        '                .ToArray
        '            Dim KO As New Dictionary(Of String, String)

        '            For Each chunk As String In targets
        '                Dim buffer As mysql.protein_ko() = model _
        '                    .Where($"{NameOf(kb_UniProtKB.mysql.protein_ko.hash_code)} IN ( {chunk} )") _
        '                    .SelectALL

        '                For Each map In buffer
        '                    With map
        '                        KO(.uniprot_id) = .KO
        '                    End With
        '                Next
        '            Next

        '            Return KO
        '        End Function

        '        Public Function GetGOTable(id As IEnumerable(Of String), mysql As mysqli, Optional chunkSize% = 3000) As Dictionary(Of String, String())
        '            Dim hashcodes = id.GetHashCode(mysql, chunkSize)
        '            Dim model As New Table(Of mysql.protein_go)(mysql)
        '            Dim targets = hashcodes.Values _
        '                .Split(chunkSize) _
        '                .Select(Function(chunk) chunk.JoinBy(", ")) _
        '                .ToArray
        '            Dim GO As New Dictionary(Of String, List(Of String))

        '            For Each chunk As String In targets
        '                Dim buffer As mysql.protein_go() = model _
        '                    .Where($"{NameOf(kb_UniProtKB.mysql.protein_go.hash_code)} IN ( {chunk} )") _
        '                    .SelectALL

        '                For Each map In buffer
        '                    With map
        '                        If Not GO.ContainsKey(.uniprot_id) Then
        '                            Call GO.Add(.uniprot_id, New List(Of String))
        '                        End If

        '                        Call GO(.uniprot_id).Add(.GO_term)
        '                    End With
        '                Next
        '            Next

        '            Return GO.ToDictionary(
        '                Function(x) x.Key,
        '                Function(list) list.Value.ToArray)
        '        End Function
    End Module
End Namespace
