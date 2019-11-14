#Region "Microsoft.VisualBasic::34f477c6c5605b4bb0c4c29981950f08, engine\GCModeller\GUID\Guid.vb"

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

    ' Class Guid
    ' 
    '     Function: AbsolutelyEqual, AbsolutelyEquals, Create, Generate, ToString
    '     Operators: <>, =
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' The Global Unique Identifier object using for identify a model component in the database.
''' (用于在数据库之中标识一个模型组件对象的全局唯一标识符对象)
''' </summary>
''' <remarks></remarks>
Public Class Guid

    Public [Class] As Guid.Classes
    Public Category As Guid.CategoryItems
    Public Uid As Guid.UidF
    Public RegistryNumber As Guid.RegistryNumberF

    Friend [String] As String

    ''' <summary>
    ''' Generate the guid string from this guid object.
    ''' (生成本GUID对象所代表的GUID字符串) 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Generate() As String
        RegistryNumber.Uid2 = Uid.Uid2
        [String] = String.Format("{0}-{1}-{2}-{3}", [Class].Description, Category._hash, Uid, RegistryNumber)
        Return [String]
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0}-{1}-{2}-{3}", [Class].Description, Category._hash, Uid, RegistryNumber)
    End Function

    ''' <summary>
    ''' Convert a guid string into a guid object instance.
    ''' (将一个GUID字符串转换为一个GUID对象实例) 
    ''' </summary>
    ''' <param name="GUID">The target guid string.(目标待转换的GUID字符串)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Widening Operator CType(GUID As String) As Guid
        Dim Tokens As String() = GUID.Split(CChar("-"))
        Return New Guid With {
            .Class = Tokens(0),
            .Category = Tokens(1) & "-" & Tokens(2),
            .Uid = Tokens(3),
            .RegistryNumber = Tokens(4)}
    End Operator

    Public Shared Narrowing Operator CType(e As Guid) As String
        Return e.Generate
    End Operator

    Public Shared Function Create() As Guid
        Dim NewGUID As New Guid With {.Class = Classes.Entity, .Category = CategoryItems.Entity.DNA}
        NewGUID.RegistryNumber = 0
        NewGUID.Uid = String.Empty

        Return NewGUID
    End Function

    ''' <summary>
    ''' The GUID relative equals, this operation just needs the 3 part of the GUID object is equal 
    ''' that these two GUID object is equal.
    ''' (相对相等，只要两个GUID对象的前三位相等即可)
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Operator =(a As Guid, b As Guid) As Boolean
        Return (a.Class = b.Class) AndAlso (a.Category = b.Category) AndAlso (a.RegistryNumber = b.RegistryNumber)
    End Operator

    Public Shared Operator <>(a As Guid, b As Guid) As Boolean
        Return Not (a = b)
    End Operator

    ''' <summary>
    ''' Judge that those two GUID object instance is absolutely equals to each other. 
    ''' (判断这两个GUID是否绝对相等)
    ''' </summary>
    ''' <param name="e">
    ''' The GUID object 'e' which will be judge with this GUID object instance.
    ''' (将要与本GUID对象实例进行比较的目标GUID对象实例)
    ''' </param>
    ''' <returns>
    ''' Get the value from the judge operation of those two GUID object equals from its string value wheter they are equals or not.
    ''' (通过判断两个GUID对象的字符串形式的GUID值是否相等来判断这两个GUID对象实例是否绝对相等)
    ''' </returns>
    ''' <remarks></remarks>
    Public Function AbsolutelyEqual(e As Guid) As Boolean
        Return String.Equals(Me.Generate, e.Generate)
    End Function

    ''' <summary>
    ''' Judge that those two GUID object instance is absolutely equals to each other. 
    ''' (判断这两个GUID是否绝对相等)
    ''' </summary>
    ''' <param name="a">The GUID object 'a'</param>
    ''' <param name="b">The GUID object 'b'</param>
    ''' <returns>
    ''' Get the value from the judge operation of those two GUID object equals from its string value wheter they are equals or not.
    ''' (通过判断两个GUID对象的字符串形式的GUID值是否相等来判断这两个GUID对象实例是否绝对相等)
    ''' </returns>
    ''' <remarks></remarks>
    Public Shared Function AbsolutelyEquals(a As Guid, b As Guid) As Boolean
        Return String.Equals(a.Generate, b.Generate)
    End Function
End Class
