#Region "Microsoft.VisualBasic::fd2957d24924b10c6be10ccf06c45cb2, RNA-Seq\BOW\CliResCommon.vb"

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

    ' Module CliResCommon
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: TryRelease
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection

Public Module CliResCommon

    Private ReadOnly bufType As Type = GetType(Byte())
    Private ReadOnly Resource As Dictionary(Of String, Func(Of Byte())) = (
        From p As PropertyInfo
        In GetType(My.Resources.Resources) _
            .GetProperties(bindingAttr:=BindingFlags.NonPublic Or BindingFlags.Static)
        Where p.PropertyType.Equals(bufType)
        Select p) _
              .ToDictionary(Of String, Func(Of Byte()))(
 _
               Function(obj) obj.Name,
               Function(obj) New Func(Of Byte())(Function() DirectCast(obj.GetValue(Nothing, Nothing), Byte())))

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Name">使用 NameOf 操作符来获取资源</param>
    ''' <returns></returns>
    Public Function TryRelease(Name As String) As String
        Dim Path As String = $"{Settings.Session.DataCache}/{Name}.exe"

        If Path.FileExists Then
            Return Path
        End If

        If Not CliResCommon.Resource.ContainsKey(Name) Then
            Return ""
        End If

        Dim bufs = CliResCommon.Resource(Name)()
        Try
            Return If(bufs.FlushStream(Path), Path, "")
        Catch ex As Exception
            ex = New Exception(Name, ex)
            ex = New Exception(Path, ex)
            Call ex.PrintException
            Call App.LogException(ex)
            Return ""
        End Try
    End Function

End Module
