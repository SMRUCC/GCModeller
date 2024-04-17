#Region "Microsoft.VisualBasic::312a2053e96ff47dae3c0980a2b6116f, WebCloud\SMRUCC.HTTPInternal\Platform\Plugin\ExternalCall.vb"

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

    '     Module ExternalCall
    ' 
    '         Function: __getPlugins, Scan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq

Namespace Platform.Plugins

    Public Module ExternalCall

        Public Function Scan(platform As PlatformEngine) As PluginBase()
            Dim plugins As New List(Of PluginBase)

            For Each dll$ In ls - l - {"*.exe", "*.dll"} <= App.HOME
                Try
                    Call plugins.Add(dll.__getPlugins(platform))
                Catch ex As Exception
                    ex = New Exception(dll, ex)  ' 可能不是.NET Assembly，则忽略掉错误记录下来然后继续下一个
                    Call App.LogException(ex)
                End Try
            Next

            Return plugins.ToArray
        End Function

        <Extension> Private Function __getPlugins(dll As String, platform As PlatformEngine) As PluginBase()
            Dim assm As Assembly = Assembly.LoadFile(dll)
            Dim types As Type() = LinqAPI.Exec(Of Type) _
 _
                () <= From typeDef As Type
                      In EmitReflection.GetTypesHelper(assm)
                      Where typeDef.IsInheritsFrom(GetType(PluginBase)) AndAlso
                          Not typeDef.IsAbstract
                      Select typeDef

            If types.Length = 0 Then
                Return New PluginBase() {}
            End If

            Dim plugins As PluginBase() = types _
                .Select(Of PluginBase)(Function(typeDef As Type)
                                           Return DirectCast(Activator.CreateInstance(typeDef, {platform}), PluginBase)
                                       End Function) _
                .ToArray

            Return plugins
        End Function
    End Module
End Namespace
