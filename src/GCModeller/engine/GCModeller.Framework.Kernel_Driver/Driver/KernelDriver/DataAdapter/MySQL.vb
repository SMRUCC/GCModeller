#Region "Microsoft.VisualBasic::0e7a81d38293771a01b677cd800586de, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\KernelDriver\DataAdapter\MySQL.vb"

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


Public Class MySQLService(Of T) : Inherits DataStorage(Of T, DataStorage.FileModel.DataSerials(Of T))

    Public Overloads Overrides Function WriteData(chunkbuffer As IEnumerable(Of DataStorage.FileModel.DataSerials(Of T)), url As String) As Boolean
        Throw New NotImplementedException
    End Function

    Public Overloads Overrides Function WriteData(url As String) As Boolean
        Throw New NotImplementedException
    End Function
End Class

