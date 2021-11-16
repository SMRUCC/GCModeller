#Region "Microsoft.VisualBasic::44b41150c14c45f9a5ed39d4f91aa80e, DataMySql\Xfam\iPfam\LocalMySQL\pdb_protein_atom_int.vb"

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

    ' Class pdb_protein_atom_int
    ' 
    '     Properties: atom_a, atom_b, auto_res_int, pdb_id
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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_protein_atom_int")>
Public Class pdb_protein_atom_int: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_res_int"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property auto_res_int As Long
    <DatabaseField("pdb_id"), DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("atom_a"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property atom_a As Long
    <DatabaseField("atom_b"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property atom_b As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_protein_atom_int` (`auto_res_int`, `pdb_id`, `atom_a`, `atom_b`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_protein_atom_int` (`auto_res_int`, `pdb_id`, `atom_a`, `atom_b`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_protein_atom_int` WHERE `auto_res_int` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_protein_atom_int` SET `auto_res_int`='{0}', `pdb_id`='{1}', `atom_a`='{2}', `atom_b`='{3}' WHERE `auto_res_int` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_res_int)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, auto_res_int, pdb_id, atom_a, atom_b)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, auto_res_int, pdb_id, atom_a, atom_b)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_res_int, pdb_id, atom_a, atom_b, auto_res_int)
    End Function
#End Region
End Class


End Namespace
