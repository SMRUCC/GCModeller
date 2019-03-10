﻿#Region "Microsoft.VisualBasic::ada85ee8fdc3670a9e2636c9acec8a10, data\RegulonDatabase\RegulonDB\RegulonDB.vb"

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

    '     Class RegulonDB
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) __export, ExportSites
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq.Extensions
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace RegulonDB

    Public Class RegulonDB

        Dim DbReflector As MySqli

        Sub New(MySQL As ConnectionUri)
            DbReflector = New MySqli(MySQL)
        End Sub

        Public Function ExportSites() As FastaFile
            Dim Table = DbReflector.Query(Of Tables.site)("select * from site")
            Dim File As FastaFile = __export(Table.ToArray)
            Return File
        End Function

        Private Shared Function __export(table As Generic.IEnumerable(Of Tables.site)) As FastaFile
            Dim fasta = table.Select(Function(site) __export(site))
            Return CType(fasta, FastaFile)
        End Function

        Private Shared Function __export(site As Tables.site) As SequenceModel.FASTA.FastaSeq
            Dim attrs As String() = New String() {
                site.key_id_org,
                site.site_id,
                site.site_internal_comment,
                site.site_length,
                site.site_note,
                site.site_posleft,
                site.site_posright
            }
            Return New SMRUCC.genomics.SequenceModel.FASTA.FastaSeq With {
                .SequenceData = site.site_sequence,
                .Headers = attrs
            }
        End Function
    End Class
End Namespace
