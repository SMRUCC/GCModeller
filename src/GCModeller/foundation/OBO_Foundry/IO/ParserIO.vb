#Region "Microsoft.VisualBasic::f7b01624798d7ed2049727fae1373387, foundation\OBO_Foundry\IO\ParserIO.vb"

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


    ' Code Statistics:

    '   Total Lines: 132
    '    Code Lines: 94 (71.21%)
    ' Comment Lines: 21 (15.91%)
    '    - Xml Docs: 80.95%
    ' 
    '   Blank Lines: 17 (12.88%)
    '     File Size: 5.82 KB


    '     Module ParserIO
    ' 
    '         Function: asTable, createModel, (+2 Overloads) LoadData
    ' 
    '         Sub: checkField
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection
Imports Field = SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection.Field

Namespace IO

    Public Module ParserIO

        ''' <summary>
        ''' header or term object.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="lines"></param>
        ''' <returns></returns>
        <Extension>
        Public Function LoadData(Of T As Class)(lines As IEnumerable(Of String)) As T
            Dim schema As Dictionary(Of BindProperty(Of Field)) = Reflector.LoadClassSchema(Of T)()
            Dim data As Dictionary(Of String, String()) = createModel(lines.ToArray)

            Return schema.LoadData(Of T)(data)
        End Function

#Const DEVELOPMENT = DEBUG

        Const TypeMissMatch1$ = "The type of the property is ""System.String"", but the data from file is ""Array(Of System.String)""!"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="schema">对象的定义</param>
        ''' <param name="data">从文件之中读取出来的一段数据</param>
        ''' <returns></returns>
        <Extension>
        Public Function LoadData(Of T As Class)(schema As Dictionary(Of BindProperty(Of Field)), data As Dictionary(Of String, String())) As T
            Dim o As T = Activator.CreateInstance(Of T)()
            Dim array$()

            For Each field As BindProperty(Of Field) In schema.Values
                Dim name As String = field.field.name

                ' Class之中有定义但是文件之中没有数据，这个是正常现象，则跳过
                If Not data.ContainsKey(name) Then
                    Continue For
                Else
                    array$ = data(name)
                End If

                If field.Type = GetType(String) Then
#If DEVELOPMENT Then
                    If array.Length > 1 Then
                        ' Class之中定义为字符串，但是文件之中是数组，则定义出错了
                        Throw New InvalidCastException(name & ": " & TypeMissMatch1)
                    End If
#End If
                    Call field.SetValue(o, array$(Scan0%))
                ElseIf field.Type Is GetType(String()) Then
                    Call field.SetValue(o, array$)
                ElseIf field.Type Is GetType(Dictionary(Of String, String)) Then
                    Call field.SetValue(o, asTable(array$))
                Else
                    Throw New NotImplementedException(field.ToString)
                End If
            Next

#If DEVELOPMENT Then
            Call checkField(schema, data)
#End If
            Return o
        End Function

        Private Function asTable(data As String()) As Dictionary(Of String, String)
            Return data _
                .Select(Function(s) s.GetTagValue(, trim:=True)) _
                .GroupBy(Function(a) a.Name) _
                .ToDictionary(Function(a) a.Key,
                              Function(group)
                                  Return group _
                                      .Select(Function(a)
                                                  Return a.Value.GetStackValue("""", """").Trim
                                              End Function) _
                                      .Where(Function(str) Not str.StringEmpty) _
                                      .JoinBy(" ")
                              End Function)
        End Function

#If DEVELOPMENT Then
        Private Sub checkField(schema As Dictionary(Of BindProperty(Of Field)), data As Dictionary(Of String, String()))
            Dim names As String() = schema.Values _
                .Select(Function(p)
                            Return p.field.name
                        End Function) _
                .ToArray

            For Each key As String In data.Keys
                If Array.IndexOf(names, key$) = -1 Then
                    ' 文件之中定义有的但是Class之中没有被定义
                    Dim array$() = data(key$)
                    Dim type$ = If(array.Length = 1, GetType(String), GetType(String())).ToString
                    Dim msg$ = $"Missing property definition in the object: <Field(""{key$}"")>Public Property {key} As {type$}"

                    Throw New Exception(msg)
                End If
            Next
        End Sub
#End If

        ''' <summary>
        ''' Parsing a term object as data model
        ''' </summary>
        ''' <param name="lines"></param>
        ''' <returns></returns>
        Private Function createModel(lines As String()) As Dictionary(Of String, String())
            Dim LQuery = From line As String
                         In lines
                         Let x = line.GetTagValue(": ")
                         Where Not String.IsNullOrEmpty(x.Name)
                         Select x
                         Group x By x.Name Into Group

            Return LQuery.ToDictionary(Function(x) x.Name,
                                       Function(x)
                                           Return x.Group _
                                               .Select(Function(value)
                                                           Return value.Value
                                                       End Function) _
                                               .ToArray
                                       End Function)
        End Function
    End Module
End Namespace
