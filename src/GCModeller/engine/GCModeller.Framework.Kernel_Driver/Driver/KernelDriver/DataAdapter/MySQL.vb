#Region "Microsoft.VisualBasic::f66af4e6b7df34f0aca18cb4c59acecd, GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\KernelDriver\DataAdapter\MySQL.vb"

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


    ' Code Statistics:

    '   Total Lines: 10
    '    Code Lines: 8
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 455 B


    ' Class MySQLService
    ' 
    '     Function: (+2 Overloads) WriteData
    ' 
    ' /********************************************************************************/

#End Region

Public Class MySQLService(Of T) : Inherits DataStorage(Of T, DataStorage.FileModel.DataSerials(Of T))

    Public Overloads Overrides Function WriteData(chunkbuffer As IEnumerable(Of DataStorage.FileModel.DataSerials(Of T)), url As String) As Boolean
        Throw New NotImplementedException
    End Function

    Public Overloads Overrides Function WriteData(url As String) As Boolean
        Throw New NotImplementedException
    End Function
End Class
