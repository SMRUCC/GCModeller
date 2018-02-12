#Region "Microsoft.VisualBasic::b96efda9c3a6ac6fbacaf3adf4fe2514, engine\GCModeller\Extensions\README.vb"

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

    ' Module README
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Terminal.stdio

''' <summary>
''' README documents for this program assembly module. 
''' (本应用程序集模块的自述文档)
''' </summary>
''' <remarks></remarks>
Module README

    ''' <summary>
    ''' The detail information of the installation, download links, required version of the 
    ''' runtime environment required by this application assembly module. 
    ''' (本应用程序集模块所需求的运行时环境的版本、安装和下载链接的详细信息)
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly RuntimeEnviroment =
        <XML>

            <!-- The correctly installation and the running of this program requires a correctly CLR(Common Language Runtime) runtime environment. -->

            <platform platform="Microsoft Windows">
                <platform
                    osplatform="Microsoft Windows 7/8 or higher windows platform"
                    CLR="Microsoft .NET Framework 4.5.1"
                    download-link="http://www.microsoft.com/en-us/download/details.aspx?id=39328"/>
                <platform
                    osplatform="Microsoft Windows Xp/Vista/2000/2003/2008/later"
                    CLR="Novell Mono 2.1"
                    download-link="http://download.mono-project.com/archive/2.10.9/windows-installer/0/mono-2.10.9-gtksharp-2.12.11-win32-0.exe"/>
            </platform>

            <platform platform="GNU Linux">
                <platform
                    osplatform="Ubuntu 12.04 or higher version"
                    CLR="Novell Mono 2.1"
                    commands="sudo apt-get install mono-complete"/>
                <platform
                    osplatform="openSUSE"
                    CLR="Novell Mono 2.1"
                    commands="zypper addrepo http://download.mono-project.com/download-stable/openSUSE_11.4 mono-stable
                              zypper refresh --repo mono-stable
                              zypper dist-upgrade --repo mono-stable"/>
            </platform>

            <platform platform="MAC OS X">
                <platform
                    osplatform="Apple MAC OS X 10.7 or later version of the osx"
                    CLR="Novell Mono 2.1"
                    download-link="http://download.mono-project.com/archive/2.10.11/macos-10-x86//MonoFramework-MRE-2.10.11.macos10.xamarin.x86.dmg"/>
            </platform>
        </XML>

    ''' <summary>
    ''' Project home site url of this program assembly on google code server.
    ''' (本应用程序集在谷歌服务器上面的项目主页)
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Project As String = <Website url="http://code.google.com/p/genome-in-code/"/>
End Module
