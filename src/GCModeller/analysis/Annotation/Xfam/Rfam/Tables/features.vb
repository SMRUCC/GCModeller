#Region "Microsoft.VisualBasic::96f1ff6fd84b049cda86fdfc3a134ed3, ..\GCModeller\analysis\Annotation\Xfam\Rfam\Tables\features.vb"

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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("features")>
Public Class features: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("rfamseq_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property rfamseq_acc As String
    <DatabaseField("database_id"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property database_id As String
    <DatabaseField("primary_id"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property primary_id As String
    <DatabaseField("secondary_id"), DataType(MySqlDbType.VarChar, "255")> Public Property secondary_id As String
    <DatabaseField("feat_orient"), NotNull, DataType(MySqlDbType.Int64, "3")> Public Property feat_orient As Long
    <DatabaseField("feat_start"), NotNull, DataType(MySqlDbType.Int64, "19")> Public Property feat_start As Long
    <DatabaseField("feat_end"), NotNull, DataType(MySqlDbType.Int64, "19")> Public Property feat_end As Long
    <DatabaseField("quaternary_id"), DataType(MySqlDbType.VarChar, "150")> Public Property quaternary_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `features` (`rfamseq_acc`, `database_id`, `primary_id`, `secondary_id`, `feat_orient`, `feat_start`, `feat_end`, `quaternary_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `features` WHERE `rfamseq_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `features` SET `rfamseq_acc`='{0}', `database_id`='{1}', `primary_id`='{2}', `secondary_id`='{3}', `feat_orient`='{4}', `feat_start`='{5}', `feat_end`='{6}', `quaternary_id`='{7}' WHERE `rfamseq_acc` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfamseq_acc)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfamseq_acc, database_id, primary_id, secondary_id, feat_orient, feat_start, feat_end, quaternary_id, rfamseq_acc)
    End Function
#End Region
End Class


End Namespace

