#Region "Microsoft.VisualBasic::f9d70ffa27a15205a33d12fe899b138f, WebCloud\SMRUCC.HTTPInternal\Core\FileSystem\InternalUpdates.vb"

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

    '     Module InternalUpdates
    ' 
    '         Sub: RunCacheUpdate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Core.Cache

    ''' <summary>
    ''' 文件内存缓存的更新策略
    ''' 
    ''' 1. 先遍历已缓存的文件，将被删除的文件从内存中删除释放
    ''' 2. 再遍历已缓存的文件，比较文件的最后写日期，将发生更新的文件重新缓存到内存中
    ''' 3. 然后遍历文件夹，添加新的文件
    ''' </summary>
    Module InternalUpdates

        <Extension>
        Public Sub RunCacheUpdate(vfs As VirtualFileSystem)

        End Sub
    End Module
End Namespace
