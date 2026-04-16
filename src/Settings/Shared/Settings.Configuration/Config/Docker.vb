#Region "Microsoft.VisualBasic::209ea0c2e499e867f23ea71155d0982f, Shared\Settings.Configuration\Config\Docker.vb"

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

'     Class Docker
' 
'         Properties: AppHome, ImageID, Local, Virtual
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices

Namespace Settings

#Disable Warning

    ''' <summary>
    ''' The configuration model of the GCModeller docker environment.
    ''' </summary>
    ''' 
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class Docker

        ''' <summary>
        ''' Docker容器对象的image编号
        ''' </summary>
        ''' <returns></returns>
        Public Property ImageID As String
        ''' <summary>
        ''' 应用程序的文件夹路径
        ''' </summary>
        ''' <returns></returns>
        Public Property AppHome As String

#Region "FileSystem Mount"
        ''' <summary>
        ''' 数据共享文件夹的宿主机上面的路径
        ''' </summary>
        ''' <returns></returns>
        Public Property Local As String
        ''' <summary>
        ''' 数据共享文件夹再虚拟机之中的路径
        ''' </summary>
        ''' <returns></returns>
        Public Property Virtual As String
#End Region

    End Class
End Namespace
