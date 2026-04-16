#Region "Microsoft.VisualBasic::c860ba48555d5ff5cf8138f3a68a2841, WebCloud\SMRUCC.WebCloud.DataCenter\Task\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: Restore
    ' 
    '         Sub: Cleanup
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.DataCenter.mysql
Imports mysqliEnd = Oracle.LinuxCompatibility.MySQL.MySqli

Namespace Platform

    Public Module Extensions

        ''' <summary>
        ''' 服务器重新开机之后尝试恢复执行未完成的任务流程
        ''' </summary>
        ''' <param name="mysql_data"></param>
        ''' <param name="app"><see cref="Task"/>模板的具体的实现定义</param>
        ''' <returns></returns>
        <Extension> Public Function Restore(mysql_data As mysql.task_pool, app As Type) As Task

        End Function

        Const mysqli_taskExpiredTime$ = "SELECT * FROM `smrucc-cloud`.sys_config WHERE `variable` = 'retention_time' LIMIT 1;"
        Const mysqli_expiredTasks$ = "SELECT * FROM `smrucc-cloud`.task_pool WHERE `status` <> 0 AND task_expired(time_complete, '{0}');"

        ''' <summary>
        ''' 对已经过期的任务进行工作区的清理工作，以减少服务器的硬盘空间的占用
        ''' </summary>
        ''' <param name="mysqli"></param>
        <Extension> Public Sub Cleanup(mysqli As mysqliEnd)
            Dim retention_time As Integer = Scripting.CTypeDynamic(Of Single)(mysqli.ExecuteScalar(Of sys_config)(mysqli_taskExpiredTime)?.value, 24.0!)
            Dim SQL$ = String.Format(mysqli_expiredTasks, retention_time)
            Dim tasks = mysqli.Query(Of task_pool)(SQL)

            For Each task As task_pool In tasks
                If task.workspace.DirectoryExists Then
                    Call FileSystem.RmDir(task.workspace)
                End If
            Next
        End Sub
    End Module
End Namespace
