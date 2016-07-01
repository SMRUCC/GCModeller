#Region "Microsoft.VisualBasic::d8d2f70e19befce32f8f3002f590acf2, ..\GCModeller\analysis\SequenceToolkit\ClustalOmega\Release.vb"

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

Public Module Release

    ''' <summary>
    ''' Release the clustal program files from this assembly module resources data.
    ''' (将本模块资源数据之中的Clustal程序释放至目标文件夹之中)
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <returns>返回clustal程序的路径</returns>
    ''' <remarks></remarks>
    Public Function ReleasePackage(DIR As String) As String
        On Error Resume Next

        Call FileIO.FileSystem.CreateDirectory(DIR)

        Call My.Resources.clustalo.FlushStream(path:=DIR & "/clustalo.exe")
        Call My.Resources.libgcc_s_dw2_1.FlushStream(path:=DIR & "/libgcc_s_dw2-1.dll")
        Call My.Resources.libgomp_1.FlushStream(path:=DIR & "/libgomp-1.dll")
        Call My.Resources.libstdc___6.FlushStream(path:=DIR & "/libstdc++-6.dll")
        Call My.Resources.mingwm10.FlushStream(path:=DIR & "/mingwm10.dll")
        Call My.Resources.pthreadGC2.FlushStream(path:=DIR & "/pthreadGC2.dll")

        Return DIR & "/clustalo.exe"
    End Function
End Module

