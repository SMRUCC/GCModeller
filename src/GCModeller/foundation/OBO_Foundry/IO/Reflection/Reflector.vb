#Region "Microsoft.VisualBasic::fc202110fe0d90b0b8316cd8f28de619, GCModeller\foundation\OBO_Foundry\IO\Reflection\Reflector.vb"

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

    '   Total Lines: 78
    '    Code Lines: 60
    ' Comment Lines: 6
    '   Blank Lines: 12
    '     File Size: 3.14 KB


    '     Module Reflector
    ' 
    '         Function: LoadClassSchema, parseBindFields, schemaParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language

Namespace IO.Reflection

    Public Module Reflector

        ReadOnly cache As New Dictionary(Of Type, Dictionary(Of BindProperty(Of Field)))

        ''' <summary>
        ''' Parsing the object fields template in the obo files
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadClassSchema(Of T As Class)() As Dictionary(Of BindProperty(Of Field))
            Return cache.ComputeIfAbsent(
                key:=GetType(T),
                lazyValue:=Function(type)
                               Return type _
                                   .GetProperties(PublicProperty) _
                                   .parseBindFields _
                                   .schemaParser
                           End Function)
        End Function

        ReadOnly supportedComplex As Index(Of Type) = {GetType(String()), GetType(Dictionary(Of String, String))}

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function parseBindFields(properties As PropertyInfo()) As BindProperty(Of Field)()
            Return LinqAPI.Exec(Of BindProperty(Of Field)) _
 _
               () <= From [property] As PropertyInfo
                     In properties
                     Let attrs As Object() = [property].GetCustomAttributes(
                         attributeType:=GetType(Field),
                         inherit:=True
                     )
                     Let tName = [property].PropertyType
                     Where Not attrs.IsNullOrEmpty AndAlso DataFramework.IsPrimitive(tName) OrElse tName Like supportedComplex
                     Let field = DirectCast(attrs.First, Field)
                     Select New BindProperty(Of Field)(field, [property])

        End Function

        <Extension>
        Private Function schemaParser(fields As BindProperty(Of Field)()) As Dictionary(Of BindProperty(Of Field))
            Dim schema As New Dictionary(Of BindProperty(Of Field))
            Dim field As Field
            Dim name As String

            For Each bField As BindProperty(Of Field) In fields
                field = bField.field

                If String.IsNullOrEmpty(field.name) Then
                    If field.toLower Then
                        name = bField.Identity.ToLower
                    Else
                        name = bField.Identity
                    End If
                Else
                    name = Nothing
                End If

                Call field.SetFields(name:=name)
                Call schema.Add(bField)
            Next

            Return schema
        End Function
    End Module
End Namespace
