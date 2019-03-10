﻿#Region "Microsoft.VisualBasic::2d895e59f8d5430b19479284dd69ef8e, Data\DataFrame\IO\BackendWriter.vb"

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

    '     Class BackendWriter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Queue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text

Namespace IO

    ''' <summary>
    ''' 常用于WebApp进行后端数据保存
    ''' </summary>
    Public Class BackendWriter

        Sub New(path$, Optional append As Boolean = True, Optional encoding As Encodings = Encodings.UTF8)

        End Sub

        Public Sub Queue(row As RowObject)

        End Sub
    End Class
End Namespace
