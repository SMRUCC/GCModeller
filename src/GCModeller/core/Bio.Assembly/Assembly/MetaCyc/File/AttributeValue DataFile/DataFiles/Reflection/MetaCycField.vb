
Namespace Assembly.MetaCyc.File.DataFiles.Reflection

    ''' <summary>
    ''' MetaCyc数据库中的一个对象的一个属性值
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, allowmultiple:=False, inherited:=True)>
    Public Class MetaCycField : Inherits Attribute

        ''' <summary>
        ''' 域名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name As String
        ''' <summary>
        ''' 域类型，提供三种类型：字符，字符串以及字符数组
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Type As MetaCycField.Types = MetaCycField.Types.String

        ''' <summary>
        ''' Data field type in a Metacyc database object instance.(MetaCyc数据库中的一个对象实例的数据属性域的数据类型)
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum Types
            ''' <summary>
            ''' A single string type variable.
            ''' </summary>
            ''' <remarks></remarks>
            [String]
            ''' <summary>
            ''' A string array type variable.
            ''' </summary>
            ''' <remarks></remarks>
            TStr
            ''' <summary>
            ''' A single Char type variable.
            ''' </summary>
            ''' <remarks></remarks>
            [Char]
        End Enum

        Public Overrides Function ToString() As String
            If Type = Types.String Then Return String.Format("(String) {0}", Name)
            'Else
            Return String.Format("(String[]) {0}", Name)
        End Function
    End Class
End Namespace