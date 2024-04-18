#Region "Microsoft.VisualBasic::5b7c3123a456b5bede2cd0a546e1b461, WebCloud\SMRUCC.HTTPInternal\Platform\Plugin\Scripting.vb"

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

    '     Class ScriptingAttribute
    ' 
    '         Properties: FileTypes
    '         Delegate Function
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: ToString
    ' 
    '     Module ScriptingExtensions
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: LoadHandlers, ReadHTML
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.DataFramework
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.WebCloud.HTTPInternal.Platform.Plugins.ScriptingAttribute

Namespace Platform.Plugins

    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class ScriptingAttribute : Inherits Attribute

        Public ReadOnly Property FileTypes As String()

        Public Delegate Function ScriptHandler(wwwroot$, path$, encoding As Encodings) As String

        ''' <summary>
        ''' ``*.vbhtml`` etc.
        ''' </summary>
        ''' <param name="extensions$"></param>
        Sub New(ParamArray extensions$())
            If extensions.IsNullOrEmpty Then
                Throw New ArgumentNullException("No file type supports!")
            Else
                FileTypes = extensions
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return FileTypes.GetJson
        End Function
    End Class

    Public Module ScriptingExtensions

        ''' <summary>
        ''' path.<see cref="ExtensionSuffix"/>.ToLower(不带小数点的后缀名，并且全部为小写形式)
        ''' </summary>
        ReadOnly scripting As IReadOnlyDictionary(Of String, ScriptHandler)
        ReadOnly delgTemplate As Type = GetType(ScriptHandler)

        Sub New()
            scripting = App.HOME.LoadHandlers
        End Sub

        <Extension>
        Public Function LoadHandlers(dir$) As Dictionary(Of String, ScriptHandler)
            Dim out As New Dictionary(Of String, ScriptHandler)

            For Each dll As String In ls - l - r - "*.dll" <= dir
                Dim assm As Assembly = Assembly.LoadFile(dll)
                Dim types As Type() = EmitReflection.GetTypesHelper(assm)
                Dim handlers = types _
                    .Select(Function(type)
                                Return type.GetMethods(bindingAttr:=PublicShared)
                            End Function) _
                    .IteratesALL _
                    .Select(Function(m)
                                Dim flag = m.GetCustomAttribute(Of ScriptingAttribute)
                                Return (flag:=flag, container:=m)
                            End Function) _
                    .Where(Function(m) Not m.flag Is Nothing) _
                    .ToArray

                For Each handler In handlers
                    Dim del As ScriptHandler = handler _
                        .container _
                        .CreateDelegate(delgTemplate)

                    For Each type$ In handler.flag.FileTypes
                        With type.Split("."c).Last.ToLower
                            out(type) = del
                        End With
                    Next
                Next
            Next

            Return out
        End Function

        <Extension>
        Public Function ReadHTML(wwwroot$, path$, Optional encoding As Encodings = Encodings.UTF8) As String
            Dim ext$ = path.ExtensionSuffix.ToLower

            If Not scripting.ContainsKey(ext) Then
                Return $"{wwwroot}/{path}".ReadAllText(encoding.CodePage)
            Else
                Return scripting(ext)(wwwroot, path, encoding)
            End If
        End Function
    End Module
End Namespace
