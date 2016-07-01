#Region "Microsoft.VisualBasic::991401623f067a7dcffa9319d4f5dc66, ..\GCModeller\analysis\Annotation\Xfam\Rfam\Tables\motif_matches.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  Microsoft VisualBasic MYSQL

' SqlDump= 


' 

Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("motif_matches")>
Public Class motif_matches: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("motif_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property motif_acc As String
    <DatabaseField("rfam_acc"), NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("rfamseq_acc"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property rfamseq_acc As String
    <DatabaseField("rfamseq_start"), DataType(MySqlDbType.Int64, "19")> Public Property rfamseq_start As Long
    <DatabaseField("rfamseq_stop"), DataType(MySqlDbType.Int64, "19")> Public Property rfamseq_stop As Long
    <DatabaseField("query_start"), DataType(MySqlDbType.Int64, "11")> Public Property query_start As Long
    <DatabaseField("query_stop"), DataType(MySqlDbType.Int64, "11")> Public Property query_stop As Long
    <DatabaseField("motif_start"), DataType(MySqlDbType.Int64, "11")> Public Property motif_start As Long
    <DatabaseField("motif_stop"), DataType(MySqlDbType.Int64, "11")> Public Property motif_stop As Long
    <DatabaseField("e_value"), DataType(MySqlDbType.VarChar, "15")> Public Property e_value As String
    <DatabaseField("bit_score"), DataType(MySqlDbType.Double)> Public Property bit_score As Double
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `motif_matches` (`motif_acc`, `rfam_acc`, `rfamseq_acc`, `rfamseq_start`, `rfamseq_stop`, `query_start`, `query_stop`, `motif_start`, `motif_stop`, `e_value`, `bit_score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `motif_matches` WHERE `motif_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `motif_matches` SET `motif_acc`='{0}', `rfam_acc`='{1}', `rfamseq_acc`='{2}', `rfamseq_start`='{3}', `rfamseq_stop`='{4}', `query_start`='{5}', `query_stop`='{6}', `motif_start`='{7}', `motif_stop`='{8}', `e_value`='{9}', `bit_score`='{10}' WHERE `motif_acc` = '{11}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, motif_acc)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, motif_acc, rfam_acc, rfamseq_acc, rfamseq_start, rfamseq_stop, query_start, query_stop, motif_start, motif_stop, e_value, bit_score, motif_acc)
    End Function
#End Region
End Class


End Namespace

