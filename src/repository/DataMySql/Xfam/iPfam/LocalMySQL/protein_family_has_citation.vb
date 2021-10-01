#Region "Microsoft.VisualBasic::93cdd93786747eea145b8249529f34d5, DataMySql\Xfam\iPfam\LocalMySQL\protein_family_has_citation.vb"

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

    ' Class protein_family_has_citation
    ' 
    '     Properties: auto_prot_fam, order_added, pmid
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace iPfam.LocalMySQL

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein_family_has_citation")>
Public Class protein_family_has_citation: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_prot_fam"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property auto_prot_fam As Long
    <DatabaseField("pmid"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property pmid As Long
    <DatabaseField("order_added"), NotNull, DataType(MySqlDbType.Int64, "4")> Public Property order_added As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein_family_has_citation` (`auto_prot_fam`, `pmid`, `order_added`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein_family_has_citation` (`auto_prot_fam`, `pmid`, `order_added`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein_family_has_citation` WHERE `auto_prot_fam` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein_family_has_citation` SET `auto_prot_fam`='{0}', `pmid`='{1}', `order_added`='{2}' WHERE `auto_prot_fam` = '{3}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_prot_fam)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, auto_prot_fam, pmid, order_added)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, auto_prot_fam, pmid, order_added)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_prot_fam, pmid, order_added, auto_prot_fam)
    End Function
#End Region
End Class


End Namespace
