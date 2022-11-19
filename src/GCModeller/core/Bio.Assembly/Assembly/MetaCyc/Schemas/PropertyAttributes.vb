#Region "Microsoft.VisualBasic::97266bdafafee0f07d6057fb8ee72f85, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\PropertyAttributes.vb"

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

    '   Total Lines: 72
    '    Code Lines: 54
    ' Comment Lines: 6
    '   Blank Lines: 12
    '     File Size: 3.25 KB


    '     Class PropertyAttributes
    ' 
    '         Properties: PropertyValue
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetAdditionalAttribute, GetEnumerator, GetEnumerator1, (+2 Overloads) ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text

Namespace Assembly.MetaCyc.Schema

    Public Class PropertyAttributes : Implements Generic.IEnumerable(Of KeyValuePair(Of String, String))

        Protected Friend _Attributes As KeyValuePair(Of String, String)()

        Public Overridable Property PropertyValue As String

        Default Public ReadOnly Property [Property](Name As String) As String
            Get
                Dim LQuery = (From item In _Attributes Where String.Equals(Name, item.Key) Select item).ToArray
                If LQuery.IsNullOrEmpty Then
                    Return ""
                Else
                    Return LQuery.First.Value
                End If
            End Get
        End Property

        Sub New(strValue As String)
            _PropertyValue = strValue
            _Attributes = PropertyAttributes.GetAdditionalAttribute(_PropertyValue)
        End Sub

        Public Overloads Shared Function ToString(PropertyValue As String, attributes As KeyValuePair(Of String, String)()) As String
            Dim sBuilder As StringBuilder = New StringBuilder(PropertyValue, 1024)
            For Each item As KeyValuePair(Of String, String) In attributes
                Call sBuilder.Append(String.Format(" [^{0} - {1}]", item.Key, item.Value))
            Next

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' 获取连接在某一个属性值的附属属性，以及原有的属性值
        ''' </summary>
        ''' <param name="strValue"></param>
        ''' <returns></returns>
        ''' <remarks>PropertyValue [^AttributeName AttributeValue]</remarks>
        Public Shared Function GetAdditionalAttribute(ByRef strValue As String) As KeyValuePair(Of String, String)()
            Dim Attribute As String = Regex.Match(strValue, " \[\^.+\]").Value
            If String.IsNullOrEmpty(Attribute) Then '没有附加的额外的属性值
                Return New KeyValuePair(Of String, String)() {}
            Else
                Dim attrList = (From m As Match In Regex.Matches(Attribute, "\[\^.+?\]") Select m.Value).ToArray
                Dim LQuery = (From attrItem As String In attrList
                              Let attrName As String = Regex.Match(attrItem, "\[\^\S+? ").Value
                              Let attrValue As String = attrItem.Replace(attrName & "- ", "")
                              Select New KeyValuePair(Of String, String)(Mid(attrName, 3).Trim, Mid(attrValue, 1, Len(attrValue) - 1))).ToArray
                strValue = strValue.Replace(Attribute, "")
                Return LQuery
            End If
        End Function

        Public Overrides Function ToString() As String
            Return PropertyValue
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String)) Implements IEnumerable(Of KeyValuePair(Of String, String)).GetEnumerator
            For Each attr In _Attributes
                Yield attr
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
