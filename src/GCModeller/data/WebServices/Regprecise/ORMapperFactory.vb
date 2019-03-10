#Region "Microsoft.VisualBasic::a65904d9fc925857a228f38cd190f906, data\WebServices\Regprecise\ORMapperFactory.vb"

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

    '     Module ORMapperFactory
    ' 
    '         Function: CreateArrayMapper, CreateObjectMapper, CreateType, GenerateAssemblyAndModule, Ref
    ' 
    '         Sub: CreateConstructor, CreateProperty
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Threading

Namespace Regprecise

    Public Module ORMapperFactory

        Public Function Ref(Of T As Class)(Optional name As String = "") As KeyValuePair(Of Type, Type)
            Dim typeRef As Type = GetType(T)
            If String.IsNullOrEmpty(name) Then
                name = typeRef.Name
            End If

            Dim array = ORMapperFactory.CreateArrayMapper(typeRef, name)
            Dim obj = ORMapperFactory.CreateObjectMapper(typeRef, name)

            Return New KeyValuePair(Of Type, Type)(array, obj)
        End Function

        Public Function CreateObjectMapper(mappedType As Type, name As String) As Type
            Dim modBuilder As ModuleBuilder = Nothing
            Dim asmBuilder As AssemblyBuilder = GenerateAssemblyAndModule(modBuilder)
            ' create new type for table name
            Dim typeBuilder As TypeBuilder = CreateType(modBuilder, $"JSON_{mappedType.Name}")
            ' create constructor
            Call CreateConstructor(typeBuilder)
            Call CreateProperty(typeBuilder, mappedType, name, isArray:=False)

            Dim mapperType As Type = typeBuilder.CreateType()
            Return mapperType
        End Function

        ''' <summary>
        ''' 创建动态类型进行json反序列化
        ''' </summary>
        ''' <param name="mappedType">动态集合</param>
        ''' <param name="name">集合的属性名称</param>
        ''' <returns></returns>
        Public Function CreateArrayMapper(mappedType As Type, name As String) As Type
            Dim modBuilder As ModuleBuilder = Nothing
            Dim asmBuilder As AssemblyBuilder = GenerateAssemblyAndModule(modBuilder)
            ' create new type for table name
            Dim typeBuilder As TypeBuilder = CreateType(modBuilder, $"JSONarray_{mappedType.Name}")
            ' create constructor
            Call CreateConstructor(typeBuilder)
            Call CreateProperty(typeBuilder, mappedType, name, True)

            Dim mapperType As Type = typeBuilder.CreateType()
            Return mapperType
        End Function

        Private Sub CreateProperty(typeBuilder As TypeBuilder, mappedType As Type, propertyName As String, isArray As Boolean)
            Dim propType As Type = If(isArray, mappedType.MakeArrayType, mappedType)
            ' Generate a private field
            Dim field As FieldBuilder = typeBuilder.DefineField("_" & propertyName, propType, FieldAttributes.[Private])
            ' Generate a public property
            Dim [property] As PropertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propType, New Type() {propType})
            ' The property set and property get methods require a special set of attributes:
            Dim GetSetAttr As MethodAttributes = MethodAttributes.[Public] Or MethodAttributes.HideBySig
            ' Define the "get" accessor method for current private field.
            Dim currGetPropMthdBldr As MethodBuilder = typeBuilder.DefineMethod($"_getOf_{propertyName}", GetSetAttr, propType, Type.EmptyTypes)
            ' Intermediate Language stuff...
            Dim currGetIL As ILGenerator = currGetPropMthdBldr.GetILGenerator()
            currGetIL.Emit(OpCodes.Ldarg_0)
            currGetIL.Emit(OpCodes.Ldfld, field)
            currGetIL.Emit(OpCodes.Ret)
            ' Define the "set" accessor method for current private field.
            Dim currSetPropMthdBldr As MethodBuilder = typeBuilder.DefineMethod($"_setOf_{propertyName}", GetSetAttr, Nothing, New Type() {propType})
            ' Again some Intermediate Language stuff...
            Dim currSetIL As ILGenerator = currSetPropMthdBldr.GetILGenerator()
            currSetIL.Emit(OpCodes.Ldarg_0)
            currSetIL.Emit(OpCodes.Ldarg_1)
            currSetIL.Emit(OpCodes.Stfld, field)
            currSetIL.Emit(OpCodes.Ret)

            ' Last, we must map the two methods created above to our PropertyBuilder to 
            ' their corresponding behaviors, "get" and "set" respectively. 
            [property].SetGetMethod(currGetPropMthdBldr)
            [property].SetSetMethod(currSetPropMthdBldr)
        End Sub

        Private Function GenerateAssemblyAndModule(ByRef modBuilder As ModuleBuilder) As AssemblyBuilder
            Dim asmBuilder As AssemblyBuilder
            Dim assemblyName As New AssemblyName() With {.Name = "DynamicJSONMapper"}
            Dim thisDomain As AppDomain = Thread.GetDomain()
            asmBuilder = thisDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
            modBuilder = asmBuilder.DefineDynamicModule(asmBuilder.GetName().Name, False)
            Return asmBuilder
        End Function

        ''' <summary>
        ''' create new type for table name
        ''' </summary>
        ''' <param name="modBuilder"></param>
        ''' <param name="typeName"></param>
        ''' <returns></returns>
        Private Function CreateType(modBuilder As ModuleBuilder, typeName As String) As TypeBuilder
            Dim typeBuilder As TypeBuilder =
            modBuilder.DefineType(typeName,
                                  TypeAttributes.[Public] Or
                                  TypeAttributes.[Class] Or
                                  TypeAttributes.AutoClass Or
                                  TypeAttributes.AnsiClass Or
                                  TypeAttributes.BeforeFieldInit Or
                                  TypeAttributes.AutoLayout, GetType(Object))

            Return typeBuilder
        End Function

        ''' <summary>
        ''' create constructor
        ''' </summary>
        ''' <param name="typeBuilder"></param>
        Private Sub CreateConstructor(typeBuilder As TypeBuilder)
            Dim constructor As ConstructorBuilder =
            typeBuilder.DefineConstructor(
            MethodAttributes.[Public] Or
            MethodAttributes.SpecialName Or
            MethodAttributes.RTSpecialName, CallingConventions.Standard, New Type(-1) {})
            'Define the reflection ConstructorInfor for System.Object
            Dim conObj As ConstructorInfo = GetType(Object).GetConstructor(New Type(-1) {})

            'call constructor of base object
            Dim il As ILGenerator = constructor.GetILGenerator()
            il.Emit(OpCodes.Ldarg_0)
            il.Emit(OpCodes.[Call], conObj)
            il.Emit(OpCodes.Ret)
        End Sub
    End Module

End Namespace
