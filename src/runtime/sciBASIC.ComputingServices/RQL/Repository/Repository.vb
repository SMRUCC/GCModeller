#Region "Microsoft.VisualBasic::87d1df93beea5ec12fff54f6bfaccdaf, ..\sciBASIC.ComputingServices\RQL\Repository\Repository.vb"

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

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON.JsonContract
Imports Microsoft.VisualBasic.Text
Imports sciBASIC.ComputingServices.Linq.Framework.DynamicCode
Imports sciBASIC.ComputingServices.Linq.Framework.Provider
Imports sciBASIC.ComputingServices.Linq.Framework.Provider.ImportsAPI
Imports sciBASIC.ComputingServices.RQL.StorageTek

Namespace Linq

    ''' <summary>
    ''' Repository database
    ''' </summary>
    Public Class Repository : Implements ISaveHandle

        ''' <summary>
        ''' {lower_case.url, type_info}
        ''' </summary>
        ''' <returns></returns>
        Public Property Models As New Dictionary(Of String, EntityProvider)

        ReadOnly __types As TypeRegistry
        ReadOnly __api As APIProvider
        ReadOnly __compiler As DynamicCompiler

        Sub New()
            __api = APIProvider.LoadDefault
            __types = TypeRegistry.LoadDefault
            __compiler = New DynamicCompiler(__types, __api)
        End Sub

        Sub New(compiler As DynamicCompiler)
            __compiler = compiler
            __api = compiler.ApiProvider
            __types = compiler.EntityProvider
        End Sub

        Public Sub AddLinq(url As Value(Of String), resource As String, Linq As GetLinqResource)
            Dim res As EntityProvider = LinqSource.Source(resource, Linq)
            If Models.ContainsKey(url = (+url).ToLower) Then
                Call Models.Remove(url)
            End If
            Call Models.Add(url, res)
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="url">大小写不敏感，不需要额外的处理</param>
        ''' <returns></returns>
        Public Function GetRepository(url As String, Optional where As String = "") As IEnumerable
            Dim api As EntityProvider = Models(url.ToLower)

            If String.IsNullOrEmpty(where) Then
                Return api.GetRepository
            Else
                Dim prefix As String = Regex.Match(where, "^\s*where\s*=", RegexOptions.IgnoreCase Or RegexOptions.Multiline).Value
                If Not String.IsNullOrEmpty(prefix) Then
                    where = Mid(where, prefix.Length + 1)
                End If

                Return api.LinqWhere(where, __compiler)
            End If
        End Function

        ''' <summary>
        ''' 获取得到的是集合之中的元素的类型
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Overloads Function [GetType](url As String) As Type
            Dim api As EntityProvider = Models(url.ToLower)
            Return api.GetType
        End Function

        Public Shared Function LoadFile(url As String) As Repository
            Try
                Return LoadJsonFile(Of Repository)(url)
            Catch ex As Exception
                ex = New Exception(url, ex)
                Call App.LogException(ex)

                Dim __new As New Repository
                Call __new.Save(url, Encodings.ASCII)
                Return __new
            End Try
        End Function

        Public Shared Function LoadDefault() As Repository
            Return LoadFile(DefaultFile)
        End Function

        Public Shared ReadOnly Property DefaultFile As String =
            App.ProductSharedDIR & "/RQL.Provider.json"

        Private Function ISaveHandle_Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            If String.IsNullOrEmpty(Path) Then
                Path = DefaultFile
            End If
            Return Me.GetJson.SaveTo(Path, encoding)
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return ISaveHandle_Save(Path, encoding.GetEncodings)
        End Function
    End Class
End Namespace

