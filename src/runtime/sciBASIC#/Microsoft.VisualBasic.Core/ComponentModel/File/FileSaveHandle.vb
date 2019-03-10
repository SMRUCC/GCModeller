﻿#Region "Microsoft.VisualBasic::bd978faa87a7eccb3c8bd4a9a14a5500, Microsoft.VisualBasic.Core\ComponentModel\File\FileSaveHandle.vb"

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

    '     Interface ISaveHandle
    ' 
    '         Function: (+2 Overloads) Save
    ' 
    '     Interface IFileReference
    ' 
    '         Properties: FilePath
    ' 
    '     Interface IDocumentEditor
    ' 
    '         Properties: DocumentPath
    ' 
    '         Function: LoadDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Text

Namespace ComponentModel

    ''' <summary>
    ''' This is a file object which have a handle to save its data to the filesystem.(这是一个带有文件数据保存方法的文件模型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ISaveHandle

        ''' <summary>
        ''' Handle for saving the file data.(保存文件的方法)
        ''' </summary>
        ''' <param name="path">The file path that will save data to.(进行文件数据保存的文件路径)</param>
        ''' <param name="encoding">The text encoding value for the text document.(文本文档的编码格式)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Save(Optional path$ = "", Optional encoding As Encoding = Nothing) As Boolean
        Function Save(Optional path$ = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean
    End Interface

    ''' <summary>
    ''' 表示一个对文件的引用接口
    ''' </summary>
    Public Interface IFileReference
        ''' <summary>
        ''' 进行文件引用的路径
        ''' </summary>
        ''' <returns></returns>
        Property FilePath As String
    End Interface

    Public Interface IDocumentEditor : Inherits ISaveHandle
        Property DocumentPath As String
        Function LoadDocument(path As String) As Boolean
    End Interface
End Namespace
