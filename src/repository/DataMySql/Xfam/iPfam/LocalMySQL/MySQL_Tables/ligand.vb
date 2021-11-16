#Region "Microsoft.VisualBasic::81f28274d7628f54991c762536bee247, DataMySql\Xfam\iPfam\LocalMySQL\MySQL_Tables\ligand.vb"

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

    ' Class ligand
    ' 
    '     Properties: atom_end, atom_start, auto_ligand, chain, ligand_id
    '                 pdb_id, residue
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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("ligand")>
Public Class ligand: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_ligand"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "8")> Public Property auto_ligand As Long
    <DatabaseField("pdb_id"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("ligand_id"), NotNull, DataType(MySqlDbType.VarChar, "3")> Public Property ligand_id As String
    <DatabaseField("chain"), NotNull, DataType(MySqlDbType.VarChar, "4")> Public Property chain As String
    <DatabaseField("residue"), DataType(MySqlDbType.Int64, "11")> Public Property residue As Long
    <DatabaseField("atom_start"), DataType(MySqlDbType.Int64, "11")> Public Property atom_start As Long
    <DatabaseField("atom_end"), DataType(MySqlDbType.Int64, "11")> Public Property atom_end As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `ligand` (`pdb_id`, `ligand_id`, `chain`, `residue`, `atom_start`, `atom_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `ligand` (`pdb_id`, `ligand_id`, `chain`, `residue`, `atom_start`, `atom_end`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `ligand` WHERE `auto_ligand` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `ligand` SET `auto_ligand`='{0}', `pdb_id`='{1}', `ligand_id`='{2}', `chain`='{3}', `residue`='{4}', `atom_start`='{5}', `atom_end`='{6}' WHERE `auto_ligand` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_ligand)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdb_id, ligand_id, chain, residue, atom_start, atom_end)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdb_id, ligand_id, chain, residue, atom_start, atom_end)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_ligand, pdb_id, ligand_id, chain, residue, atom_start, atom_end, auto_ligand)
    End Function
#End Region
End Class


End Namespace
