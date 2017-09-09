#Region "Microsoft.VisualBasic::d5788607995b20ac2096fda7c7e46b0b, ..\repository\DataMySql\kb_UniProtKB\ShellScriptAPI.vb"

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

Imports System.Runtime.CompilerServices
Imports Oracle.LinuxCompatibility.MySQL
Imports mysqli = Oracle.LinuxCompatibility.MySQL.MySQL

Namespace kb_UniProtKB

    ''' <summary>
    ''' 使用Uniprot的mysql知识库进行蛋白注释的获取的引擎
    ''' </summary>
    Public Module UniprotKBEngine

        ''' <summary>
        ''' 获取得到哈希码，然后应用于快读的查询其他的蛋白数据
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="mysql"></param>
        ''' <param name="chunkSize%"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetHashCode(id As IEnumerable(Of String), mysql As mysqli, Optional chunkSize% = 3000) As Dictionary(Of String, Long)
            Dim model As New Table(Of mysql.hash_table)(mysql)
            Dim targets$() = id _
                .Split(chunkSize) _
                .Select(Function(chunk) chunk.JoinBy(", ")) _
                .ToArray
            Dim hashCode As New Dictionary(Of String, Long)

            For Each chunk As String In targets
                Dim buffer As mysql.hash_table() = model _
                    .Where($"{NameOf(kb_UniProtKB.mysql.hash_table.uniprot_id)} IN ( {chunk} )") _
                    .SelectALL

                For Each uniprotID As mysql.hash_table In buffer
                    With uniprotID
                        Call hashCode.Add(.uniprot_id, .hash_code)
                    End With
                Next
            Next

            Return hashCode
        End Function

        <Extension>
        Public Function GetKOTable(id As IEnumerable(Of String), mysql As mysqli, Optional chunkSize% = 3000) As Dictionary(Of String, String)
            Dim list = id.ToArray
            Dim hashcodes = list.GetHashCode(mysql, chunkSize)
            Dim model As New Table(Of mysql.protein_ko)(mysql)
            Dim targets = hashcodes.Values _
                .Split(chunkSize) _
                .Select(Function(chunk) chunk.JoinBy(", ")) _
                .ToArray
            Dim KO As New Dictionary(Of String, String)

            For Each chunk As String In targets
                Dim buffer As mysql.protein_ko() = model _
                    .Where($"{NameOf(kb_UniProtKB.mysql.protein_ko.hash_code)} IN ( {chunk} )") _
                    .SelectALL

                For Each map In buffer
                    With map
                        KO(.uniprot_id) = .KO
                    End With
                Next
            Next

            Return KO
        End Function
    End Module
End Namespace