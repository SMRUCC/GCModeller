#Region "Microsoft.VisualBasic::f5515d0b3298822a0da5930a984abab0, ..\visualbasic.DBI\LibMySQL\Workbench\Dump\RestoreWorker.vb"

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

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Workbench.Dump

    Public Class RestoreWorker

        Public ReadOnly Property MySQL As MySQL

        Sub New(uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
            MySQL = New MySQL
            Call MySQL.Connect(uri)
        End Sub

        ''' <summary>
        ''' 会需要动态编译
        ''' </summary>
        ''' <param name="dumpDir"></param>
        ''' <returns></returns>
        Public Function ImportsData(dumpDir As String, Optional dbName As String = "") As Boolean
            Dim SQLs As String() = FileIO.FileSystem.GetFiles(
                dumpDir,
                FileIO.SearchOption.SearchTopLevelOnly,
                "*.sql").ToArray(Function(file) FileIO.FileSystem.ReadAllText(file))

            If String.IsNullOrEmpty(dbName) Then
                dbName = FileIO.FileSystem.GetDirectoryInfo(dumpDir).Name
                dbName = dbName.NormalizePathString
            End If

            Call MySQL.Execute($"CREATE SCHEMA `{dbName}` ;")

            '   Dim Tables = SQLs.ToArray(Of KeyValuePair)(Function(sql) CodeGenerator.GenerateClass(sql, ""))

            Throw New NotImplementedException
        End Function
    End Class
End Namespace
