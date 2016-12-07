#Region "Microsoft.VisualBasic::7891e72e7d28353bed76decf7188d042, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\SDK_PATH.vb"

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


''' <summary>
''' .NET Framework的Reference Assembly文件夹的位置
''' </summary>
''' <remarks></remarks>
Module SDK_PATH

    ''' <summary>
    ''' 从高版本到低版本排列，从x64到x86排列
    ''' </summary>
    ''' <remarks></remarks>
    Private ReadOnly _pathList As String() = New String() {
        "C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.3",
        "C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1",
        "C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5",
        "C:\Windows\Microsoft.NET\Framework\v4.0.30319",
        "C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0",
        "C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\Client",
        "C:\Windows\Microsoft.NET\Framework\v3.5",
        "C:\Windows\Microsoft.NET\Framework\v2.0.50727"
    }

    ''' <summary>
    ''' .NET Framework的Reference Assembly文件夹的位置
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property AvaliableSDK As String
        Get
            For Each Path As String In _pathList
                If FileIO.FileSystem.DirectoryExists(Path) Then
                    Return Path
                End If
            Next

            Return ""
        End Get
    End Property
End Module

