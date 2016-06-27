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
            For Each item In attributes
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