﻿#Region "Microsoft.VisualBasic::21c4b77c869f821f1b7476e626fc24dd, foundation\OBO_Foundry\IO\Writer.vb"

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

    '   Total Lines: 91
    '    Code Lines: 65 (71.43%)
    ' Comment Lines: 14 (15.38%)
    '    - Xml Docs: 92.86%
    ' 
    '   Blank Lines: 12 (13.19%)
    '     File Size: 3.73 KB


    '     Module Writer
    ' 
    '         Function: (+2 Overloads) ToLines
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection
Imports any = Microsoft.VisualBasic.Scripting
Imports Field = SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection.Field

Namespace IO

    Public Module Writer

        ''' <summary>
        ''' For generates the obo document and save data model into the file system.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="target"></param>
        ''' <param name="schema"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ToLines(Of T As Class)(target As T,
                                                        schema As Dictionary(Of BindProperty(Of Field)),
                                                        Optional excludes As Index(Of String) = Nothing,
                                                        Optional strip_namespace_prefix As String = Nothing) As IEnumerable(Of String)
            Dim name$
            Dim value As Object
            Dim vals As Array = Nothing
            Dim pvalue As IEnumerable(Of String)
            Dim property_namespace_prefix As Boolean = strip_namespace_prefix Is Nothing

            For Each [property] As BindProperty(Of Field) In schema.Values
                If [property].Type Is GetType(String) Then
                    name = [property].field.name
                    value = [property].GetValue(target)

                    If value Is Nothing Then
                        Continue For
                    ElseIf Not excludes Is Nothing AndAlso name Like excludes Then
                        Continue For
                    End If

                    Yield String.Format("{0}: {1}", name, value.ToString)
                Else
                    value = [property].GetValue(target)

                    If value Is Nothing Then
                        Continue For
                    ElseIf value.GetType.IsArray Then
                        vals = value
                    ElseIf TypeOf value Is Dictionary(Of String, String) Then
                        vals = DirectCast(value, Dictionary(Of String, String)) _
                            .Select(Function(t1) $"{t1.Key} ""{t1.Value}""") _
                            .ToArray
                    Else
                        Throw New NotImplementedException(value.GetType.FullName)
                    End If

                    If vals.IsNullOrEmpty Then
                        Continue For
                    Else
                        name = [property].field.name

                        If Not excludes Is Nothing AndAlso name Like excludes Then
                            Continue For
                        End If
                    End If

                    If property_namespace_prefix OrElse name <> "property_value" Then
                        ' do nothing when reserved namespace prefix
                        pvalue = From o As Object
                                 In vals
                                 Let str As String = any.ToString(o)
                                 Select String.Format("{0}: {1}", name, str)
                    Else
                        ' trim namespace prefix for property valye
                        pvalue = From o As Object
                                 In vals.AsParallel
                                 Let str As String = any.ToString(o).Replace(strip_namespace_prefix, "")
                                 Select String.Format("{0}: {1}", name, str)
                    End If

                    For Each line As String In pvalue
                        Yield line
                    Next
                End If
            Next
        End Function

        ''' <summary>
        ''' For generates the obo document and save data model into the file system.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="target"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ToLines(Of T As Class)(target As T) As String()
            Return target _
                .ToLines(Reflector.LoadClassSchema(Of T)()) _
                .ToArray
        End Function
    End Module
End Namespace
