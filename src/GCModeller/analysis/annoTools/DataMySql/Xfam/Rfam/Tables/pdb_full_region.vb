#Region "Microsoft.VisualBasic::37e2489f46cb16efc24ab7c4ff4a8b5f, ..\GCModeller\analysis\annoTools\DataMySql\Xfam\Rfam\Tables\pdb_full_region.vb"

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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_full_region")>
Public Class pdb_full_region: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfam_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "7")> Public Property rfam_acc As String
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("chain"), DataType(MySqlDbType.VarChar, "4")> Public Property chain As String
    <DatabaseField("pdb_start"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property pdb_start As Long
    <DatabaseField("pdb_end"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property pdb_end As Long
    <DatabaseField("bit_score"), NotNull, DataType(MySqlDbType.Double)> Public Property bit_score As Double
    <DatabaseField("evalue_score"), NotNull, DataType(MySqlDbType.VarChar, "15")> Public Property evalue_score As String
    <DatabaseField("cm_start"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property cm_start As Long
    <DatabaseField("cm_end"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property cm_end As Long
    <DatabaseField("hex_colour"), DataType(MySqlDbType.VarChar, "6")> Public Property hex_colour As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_full_region` (`rfam_acc`, `pdb_id`, `chain`, `pdb_start`, `pdb_end`, `bit_score`, `evalue_score`, `cm_start`, `cm_end`, `hex_colour`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_full_region` WHERE `rfam_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_full_region` SET `rfam_acc`='{0}', `pdb_id`='{1}', `chain`='{2}', `pdb_start`='{3}', `pdb_end`='{4}', `bit_score`='{5}', `evalue_score`='{6}', `cm_start`='{7}', `cm_end`='{8}', `hex_colour`='{9}' WHERE `rfam_acc` = '{10}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfam_acc)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfam_acc, pdb_id, chain, pdb_start, pdb_end, bit_score, evalue_score, cm_start, cm_end, hex_colour, rfam_acc)
    End Function
#End Region
End Class


End Namespace
