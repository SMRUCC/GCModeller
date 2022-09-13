#Region "Microsoft.VisualBasic::c74d036eb9fb2eae6cf32525be00866d, GCModeller\foundation\OBO_Foundry\IO\Writer.vb"

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

    '   Total Lines: 71
    '    Code Lines: 46
    ' Comment Lines: 14
    '   Blank Lines: 11
    '     File Size: 2.70 KB


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
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection
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
        Public Function ToLines(Of T As Class)(target As T, schema As Dictionary(Of BindProperty(Of Field))) As String()
            Dim bufs As New List(Of String)
            Dim name$
            Dim value As Object
            Dim vals As Object()

            For Each [property] As BindProperty(Of Field) In schema.Values
                If [property].Type Is GetType(String) Then
                    name = [property].field.name
                    value = [property].GetValue(target)

                    If value Is Nothing Then
                        Continue For
                    End If

                    bufs += String.Format("{0}: {1}", name, value.ToString)
                Else
                    vals = [property].GetValue(target)

                    If vals.IsNullOrEmpty Then
                        Continue For
                    End If

                    Dim pvalue = From o As Object
                                 In vals
                                 Let str As String = Scripting.ToString(o)
                                 Select str

                    bufs += From val As String
                            In pvalue
                            Let pname As String = [property].field.name
                            Select String.Format("{0}: {1}", pname, val)
                End If
            Next

            Return bufs.ToArray
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
            Return target.ToLines(Reflector.LoadClassSchema(Of T)())
        End Function
    End Module
End Namespace
