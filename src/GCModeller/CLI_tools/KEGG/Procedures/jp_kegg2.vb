#Region "Microsoft.VisualBasic::bd9e788c401a6c77214adaebcf78e0d5, ..\GCModeller\CLI_tools\KEGG\Procedures\jp_kegg2.vb"

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

Public MustInherit Class jp_kegg2

    Public ReadOnly Property KEGG As Oracle.LinuxCompatibility.MySQL.MySQL

    Public Const DB_NAME As String = "jp_kegg2"

    Sub New(uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
        uri.Database = DB_NAME
        Me.KEGG = uri
        Call $"{Me.KEGG.Ping}ms.....".__DEBUG_ECHO
    End Sub
End Class

