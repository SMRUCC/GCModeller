#Region "Microsoft.VisualBasic::52760a5a416485b3ac5216566ade98f4, RDotNET.Extensions.VisualBasic\Extensions\Serialization\Serialization.vb"

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

    '     Module SerializationExtensions
    ' 
    '         Function: __createMatrix, __loadFromStream, InternalLoadS4Object, LoadRStream, S4Object
    ' 
    '         Sub: __mappingCollectionType, __rVectorToNETProperty, __valueMapping
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.SymbolicExpressionExtension

Namespace Serialization

    ''' <summary>
    ''' Convert the R object into a .NET object from the specific type schema information.
    ''' (将R之中的对象内存数据转换为.NET之中指定的对象实体)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("R.Serialization",
                  Description:="Convert the R object into a .NET object from the specific type schema information.",
                  Category:=APICategories.UtilityTools,
                  Publisher:="xieguigang@gcmodeller.org")>
    Public Module SerializationExtensions

        ''' <summary>
        ''' Deserialize the R object into a specific .NET object. <see cref="RDotNET.SymbolicExpression"></see> --> "T"
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="RData"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 反序列化的规则：
        ''' 
        ''' 1. S4对象里面的Slot为对象类型之中的属性
        ''' 2. 任何对象属性都会被表示为数组
        ''' </remarks>
        ''' 
        <Extension>
        Public Function S4Object(Of T As Class)(RData As RDotNET.SymbolicExpression) As T
            Dim value As Object = __loadFromStream(RData, GetType(T), 1)
            Return DirectCast(value, T)
        End Function

        ''' <summary>
        ''' Needs your manual type casting in your program. 
        ''' </summary>
        ''' <param name="RData"></param>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("RStream.Load")>
        Public Function LoadRStream(<Parameter("R.S4Object")> RData As RDotNET.SymbolicExpression, Type As Type) As Object
            Dim value As Object = __loadFromStream(RData, Type, 1)
            Return value
        End Function

        ''' <summary>
        ''' The recursive operation of the S4Object in R starts from here. this recursive operation will stop when the property value is not a S4Object.
        ''' (这个可能是一个递归的过程，一直解析到各个属性的R类型不再是S4对象类型为止)
        ''' </summary>
        ''' <param name="RData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InternalLoadS4Object(RData As RDotNET.SymbolicExpression, type As Type, DebugLevel As Integer) As Object
            Dim mappings As Dictionary(Of BindProperty(Of SchemaMaps.DataFrameColumnAttribute)) = LoadMapping(type, mapsAll:=True)
            Dim obj As Object = Activator.CreateInstance(type)

            Call $"{type.FullName}  ---> R.S4Object (""{String.Join("; ", RData.GetAttributeNames)}"")".__DEBUG_ECHO

            For Each slot As BindProperty(Of SchemaMaps.DataFrameColumnAttribute) In mappings.Values
                Dim RSlot As RDotNET.SymbolicExpression = RData.GetAttribute(slot.field.Name)
                Dim value As Object = __loadFromStream(RSlot, slot.Type, DebugLevel)

                Call __valueMapping(value, slot.member, obj:=obj)
            Next

            Return obj
        End Function

        ''' <summary>
        ''' 
        ''' </summary> 
        ''' <param name="value"></param>
        ''' <param name="pInfo"></param>
        ''' <param name="obj">对象实例</param>
        ''' <remarks></remarks>
        Private Sub __valueMapping(value As Object, pInfo As PropertyInfo, ByRef obj As Object)
            If Not value Is Nothing Then
                Dim type As Type = pInfo.PropertyType

                If type.HasElementType Then
                    Call __mappingCollectionType(value, pInfo, obj, type)
                Else
                    Call __rVectorToNETProperty(
                        pTypeInfo:=value.GetType,
                        value:=value,
                        obj:=obj,
                        pInfo:=pInfo
                    )
                End If
            End If
        End Sub

        ''' <summary>
        ''' All of the object in R is a vector, so that we needs this function to convert the R vector to a property value.
        ''' </summary>
        ''' <param name="pTypeInfo"></param>
        ''' <param name="value"></param>
        ''' <param name="pInfo"></param>
        ''' <param name="obj"></param>
        ''' <remarks></remarks>
        Private Sub __rVectorToNETProperty(pTypeInfo As Type, value As Object, pInfo As PropertyInfo, ByRef obj As Object)
            If Not pTypeInfo.HasElementType Then '目标映射的属性不是数组，但是R之中的几乎所有的对象都是使用数组表示的，故而在这里可能需要取第一个元素
                Call pInfo.SetValue(obj, value)
                Return
            End If

            Dim source As Object() = (From val As Object In DirectCast(value, IEnumerable) Select val).ToArray

            If source.IsNullOrEmpty Then
                Call pInfo.SetValue(obj, Nothing)
            Else
                value = source.First
                Call pInfo.SetValue(obj, value)
            End If
        End Sub

        ''' <summary>
        ''' Object() to T()()
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="pInfo"></param>
        ''' <param name="obj"></param>
        ''' <param name="pTypeInfo"></param>
        ''' <remarks></remarks>
        Private Sub __mappingCollectionType(value As Object, pInfo As PropertyInfo, ByRef obj As Object, pTypeInfo As Type)
            Dim type As Type = pTypeInfo.GetElementType
            Dim source As Object() = (From val As Object In DirectCast(value, IEnumerable) Select val).ToArray
            Dim list As Array = Array.CreateInstance(type, source.Length)

            For i As Integer = 0 To source.Length - 1
                Call list.SetValue(source(i), i)
            Next

            Call pInfo.SetValue(obj, list)
        End Sub

        ''' <summary>
        ''' Load the R symbolic expression data recursivly start from here.
        ''' </summary>
        ''' <param name="RData"></param>
        ''' <param name="TypeInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Private Function __loadFromStream(RData As RDotNET.SymbolicExpression, TypeInfo As Type, DebugLevel As Integer) As Object
            If RData Is Nothing Then
                Return Nothing
            Else
#If DEBUG Then
                Call Console.WriteLine(New String("."c, DebugLevel) & ">   " & RData.Type.ToString)
#End If
            End If

            Select Case RData.Type

                Case Internals.SymbolicExpressionType.S4

                    'Load the R symbolic expression data recursivly start from here.
                    Return InternalLoadS4Object(RData, TypeInfo, DebugLevel + 1)

                Case Internals.SymbolicExpressionType.LogicalVector
                    Return RData.AsLogical.ToArray
                Case Internals.SymbolicExpressionType.CharacterVector
                    Return RData.AsCharacter.ToArray
                Case Internals.SymbolicExpressionType.IntegerVector
                    Return RData.AsInteger.ToArray
                Case Internals.SymbolicExpressionType.NumericVector
                    Return RData.AsNumeric.ToArray
                Case Internals.SymbolicExpressionType.List
                    Return __createMatrix(RData, TypeInfo, DebugLevel + 1)
                Case Internals.SymbolicExpressionType.LanguageObject
                    Dim lang As Language = RData.AsLanguage
                    Dim calls As Symbol() = lang.FunctionCall.ToArray
                    Dim result = calls.Select(Function(x) x.PrintName).ToArray
                    Return result

                Case Else
                    'Throw New NotImplementedException(RData.Type.ToString)
                    Return Nothing
            End Select
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="RData"></param>
        ''' <param name="typeInfo"></param>
        ''' <param name="debugLv">Debugger output levels</param>
        ''' <returns></returns>
        Private Function __createMatrix(RData As RDotNET.SymbolicExpression, typeInfo As Type, debugLv As Integer) As Object
            Dim list As SymbolicExpression() = RData.AsList.ToArray
            Dim matrix As Object = LinqAPI.Exec(Of Object) _
 _
                () <= From vec As SymbolicExpression
                      In list
                      Let obj As Object = __loadFromStream(vec, typeInfo, debugLv)
                      Select obj

            Return matrix
        End Function
    End Module
End Namespace
