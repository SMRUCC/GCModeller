#Region "Microsoft.VisualBasic::079cca54281b84db1a2b33288ee48873, DataMySql\Xfam\iPfam\LocalMySQL\MySQL_Tables\pdb_protein_region_lig_int.vb"

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

    ' Class pdb_protein_region_lig_int
    ' 
    '     Properties: auto_ligand, auto_reg_lig_int, pdb_id, quality_control, region_id
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
''' domain ligand interactions
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pdb_protein_region_lig_int")>
Public Class pdb_protein_region_lig_int: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("auto_reg_lig_int"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property auto_reg_lig_int As Long
    <DatabaseField("pdb_id"), DataType(MySqlDbType.VarChar, "4")> Public Property pdb_id As String
    <DatabaseField("region_id"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property region_id As Long
    <DatabaseField("auto_ligand"), NotNull, DataType(MySqlDbType.Int64, "8")> Public Property auto_ligand As Long
    <DatabaseField("quality_control"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property quality_control As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `pdb_protein_region_lig_int` (`pdb_id`, `region_id`, `auto_ligand`, `quality_control`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `pdb_protein_region_lig_int` (`pdb_id`, `region_id`, `auto_ligand`, `quality_control`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `pdb_protein_region_lig_int` WHERE `auto_reg_lig_int` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `pdb_protein_region_lig_int` SET `auto_reg_lig_int`='{0}', `pdb_id`='{1}', `region_id`='{2}', `auto_ligand`='{3}', `quality_control`='{4}' WHERE `auto_reg_lig_int` = '{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, auto_reg_lig_int)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pdb_id, region_id, auto_ligand, quality_control)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pdb_id, region_id, auto_ligand, quality_control)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, auto_reg_lig_int, pdb_id, region_id, auto_ligand, quality_control, auto_reg_lig_int)
    End Function
#End Region
End Class


End Namespace
