Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Emit.CodeDOM_VBC
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

''' <summary>
''' 
''' </summary>
Public Module CodeGenerator

    Const DOC_SECTIONS As String = "<[!]-- =+"
    Const DOC_END_HEAD As String = "=+ -->"

    ''' <summary>
    ''' 从Java的网页帮助文档之中生成VB源代码的一个辅助开发工具
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Public Function GenerateCode(url As String) As String
        Dim WebPage As String = url.GET
        Dim Tokens As String() = Regex.Split(WebPage, DOC_SECTIONS).Skip(1).ToArray
        Dim hash = GenerateHashd(Tokens)
        Dim code As String = GenerateCode(hash)

        code = $"#Region ""Java™ Platform, Standard Edition 7 API Specification, Version=1.7, Culture=neutral, PublicKeyToken=b77a5c561934e089""
' {url}
#End Region


" & code

        Return code
    End Function

    Private Function GenerateHashd(Tokens As String()) As Dictionary(Of String, String)
        Dim LQuery = (From s As String In Tokens
                      Let InnerTokens = Regex.Split(s, DOC_END_HEAD)
                      Let Key As String = InnerTokens.First.Trim
                      Let value As String = InnerTokens.Last.Trim
                      Where Not String.IsNullOrEmpty(value)
                      Select Key, value).ToArray
        Dim hash = LQuery.ToDictionary(Function(obj) obj.Key, elementSelector:=Function(obj) obj.value)
        Return hash
    End Function

    Private Function GenerateCode(hash As Dictionary(Of String, String)) As String
        Dim Doc As Global.System.CodeDom.CodeNamespace = GenerateDefinitionFramework(hash)
        Dim code As String = Doc.GenerateCode.Replace(" ", "").Replace("MyBase.New", "")
        code = "Imports Oracle" & vbCrLf & code
#If DEBUG Then
        Call $" [{NameOf(CodeDomProvider)}]  --------> {vbCrLf & vbCrLf}{code}".__DEBUG_ECHO
#End If
        Return code
    End Function

    Const DOC_CLASS_SUMMARY As String = "START OF CLASS DATA"

    Private Function GenerateDefinitionFramework(hash As Dictionary(Of String, String)) As CodeDom.CodeNamespace
        If Not hash.ContainsKey(DOC_CLASS_SUMMARY) Then
            Throw New Exception($"[{CodeGenerator.DOC_CLASS_SUMMARY}] Document missing class object definition summary!")
        End If

        Dim Doc As String = hash(CodeGenerator.DOC_CLASS_SUMMARY)
        Dim clsNamespace As String = "<div class=""subTitle"">.+?</div>"
        Dim clsName As String = "<li>.*?<ul class=""inheritance"">.*?<li>.+?</li>.*?</ul>.*?</li>"

        clsNamespace = Regex.Match(Doc, clsNamespace, RegexOptions.Singleline).Value.GetValue
        clsName = Regex.Match(Doc, clsName, RegexOptions.Singleline).Value
        clsName = Regex.Match(clsName, "<li>[.a-zA-Z]+</li>", RegexOptions.Singleline).Value.GetValue.Split("."c).Last

        Dim classDef As New CodeDom.CodeTypeDeclaration(clsName)
        Dim clsNamespaceDef As New CodeDom.CodeNamespace(clsNamespace)
        Dim clsInherits As String = ""

        classDef.Comments.Add(New CodeDom.CodeCommentStatement(XmlComments(GetSummaryComments(Doc, clsInherits))))

        If String.IsNullOrEmpty(clsInherits) Then
            clsInherits = "System.Object"
        End If

        classDef.BaseTypes.Add(New CodeDom.CodeTypeReference(clsInherits))

        For Each [Interface] As String In GetImplementsInterface(Doc)
            classDef.BaseTypes.Add(New CodeDom.CodeTypeReference([Interface]))
        Next

        Call GenerateFields(classDef, hash)
        Call GenerateConstructors(classDef, hash)
        Call GenerateEnumMethods(classDef, hash)
        Call GenerateMethods(classDef, hash)

        Call clsNamespaceDef.Types.Add(classDef)

        Return clsNamespaceDef
    End Function

#Region "METHOD DETAIL"

    Const ENUM_CONSTANT_DETAIL As String = "ENUM CONSTANT DETAIL"
    Const DOC_METHOD_DETAIL As String = "METHOD DETAIL"

    ''' <summary>
    ''' Java里面的枚举常量都是共享的方法来的？？？？
    ''' </summary>
    ''' <param name="clsDef"></param>
    ''' <param name="hash"></param>
    Public Sub GenerateEnumMethods(ByRef clsDef As CodeDom.CodeTypeDeclaration, hash As Dictionary(Of String, String))
        If Not hash.ContainsKey(ENUM_CONSTANT_DETAIL) Then
            Return
        End If

        Dim Doc As String = hash(CodeGenerator.ENUM_CONSTANT_DETAIL)
        Dim Tokens As String() = Strings.Split(Strings.Split(Doc, "<h3>Enum Constant Detail</h3>").Last, "<a name=")

        For Each def As String In Tokens
            If Not Len(def) > 5 Then
                Continue For
            End If

            Dim field As CodeDom.CodeMemberField = Nothing
            Call clsDef.Members.Add(GenerateEnumMethod(def, field))
            Call clsDef.Members.Add(field)
        Next
    End Sub

    ''' <summary>
    ''' 生成静态的域 和函数的返回方法
    ''' </summary>
    ''' <param name="Token"></param>
    ''' <returns></returns>
    Private Function GenerateEnumMethod(Token As String, ByRef sharedField As CodeDom.CodeMemberField) As CodeDom.CodeMemberMethod
        Dim Comments As String = Regex.Match(Token, "<div.+</div>", RegexOptions.Singleline).Value.GetValue

        Comments = Strings.Split(Comments, "</div>").First
        Token = Strings.Split(Token, "Throws:").First

        If InStr(Token, "Returns:") > 0 Then
            Token = Regex.Match(Token, "<h4>.+?</h4>.*?<pre>.+?>Returns:.+?</dd>", RegexOptions.Singleline).Value
        End If

        Dim ReturnsComment As String = Strings.Split(Regex.Match(Token, "<span class=""strong"">.*?Returns:</span>.*?</dt>.*?<dd>.+?</dd>", RegexOptions.Singleline).Value, "Returns:").Last
        Dim Name As String = Regex.Match(Token, "<h4>.+?</h4>", RegexOptions.Singleline).Value.GetValue
#If DEBUG Then
        Call Console.WriteLine($"[DEBUG ====> {NameOf(GenerateMethod)}: {Name}]")
#End If
        ReturnsComment = Regex.Match(ReturnsComment, "<dd>.+?</dd>", RegexOptions.Singleline).Value.GetValue

        Dim Def As String = Regex.Match(Token, "<pre>.+?</pre>", RegexOptions.Singleline).Value.Replace("&nbsp;", " ")
        Dim XmlComment As StringBuilder = New StringBuilder(XmlComments(Comments))
        Dim Tokens As String() = Def.Split
        Dim attr As String = Tokens.First.Replace("<pre>", "")
        Dim Method As New CodeDom.CodeMemberMethod With {
            .Attributes = CodeGenerator.GetMemberAccess(attr) + CodeGenerator.GetMemberAccess("static"),
            .Name = Name
        }
        Dim ReturnType As String = CodeGenerator.TrimType(Tokens(Tokens.Length - 2).GetValue)

        Method.ReturnType = New CodeDom.CodeTypeReference(ReturnType)
        sharedField = __EnumSharedField("__" & Name, ReturnType)

        If Not String.Equals(GetType(Void).FullName, ReturnType) Then
            ReturnsComment = ReturnsComment.Replace(vbLf, "")
            ReturnsComment = String.Join(" ", (From s As String In Strings.Split(ReturnsComment, vbCr) Select Trim(s)).ToArray)

            Call XmlComment.AppendLine()
            Call XmlComment.Append($"'' <returns>{ReturnsComment}</returns>")
        End If
        Call Method.Comments.Add(New CodeDom.CodeCommentStatement(XmlComment.ToString))
        Call Method.Statements.Add(CodeDOMExpressions.Return(Field(sharedField.Name, type:=ReturnType)))

        Return Method
    End Function

    Private Function __EnumSharedField(Name As String, type As String) As CodeDom.CodeMemberField
        Dim Field As New CodeDom.CodeMemberField(New CodeDom.CodeTypeReference(type), Name)
        Field.Attributes = CodeDom.MemberAttributes.Static + CodeDom.MemberAttributes.Private
        Field.InitExpression = CodeDOMExpressions.[New](type)
        Return Field
    End Function

    Public Sub GenerateMethods(ByRef clsDef As CodeDom.CodeTypeDeclaration, hash As Dictionary(Of String, String))
        If Not hash.ContainsKey(DOC_METHOD_DETAIL) Then
            Return
        End If

        Dim Doc As String = hash(CodeGenerator.DOC_METHOD_DETAIL)
        Dim Tokens As String() = Strings.Split(Strings.Split(Doc, "<h3>Method Detail</h3>").Last, "<a name=")

        For Each Method In (From s As String In Tokens Where Len(s) > 5 Select GenerateMethod(s)).ToArray
            Call clsDef.Members.Add(Method)
        Next

    End Sub

    ''' <summary>
    ''' 修剪方法定义的摘要信息
    ''' </summary>
    ''' <param name="def"></param>
    ''' <returns></returns>
    Private Function __trimDef(def As String) As String
        Dim Matches = Regex.Matches(def, "<a.+?/a>", RegexOptions.Singleline)
        Dim sbr As New StringBuilder(def)
        For Each m As Match In Matches
            Call sbr.Replace(m.Value, CodeGenerator.href2FullName(m.Value))
        Next
        Return sbr.ToString
    End Function

    ReadOnly __attributes As String() = {"public", "private", "protected", "friend", "static", "final"}

    Private Function __buildAttrs(ByRef Tokens As String()) As CodeDom.MemberAttributes
        Dim attr As CodeDom.MemberAttributes
        Dim i As Integer = 0

        Tokens = (From s As String In Tokens Let ss As String = Trim(s) Where Not String.IsNullOrEmpty(ss) Select ss).ToArray
        Tokens(Scan0) = Tokens(Scan0).Replace("<pre>", "").Trim

        For i = 0 To Tokens.Length - 1
            Dim value = CodeGenerator.GetMemberAccess(Tokens(i))
            If value = CodeDom.MemberAttributes.VTableMask Then

                If String.Equals("final", Tokens(i)) Then
                    i += 1
                End If

                Exit For
            Else
                attr += value
            End If
        Next

        Tokens = Tokens.Skip(i).ToArray

        Return attr
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Token"></param>
    ''' <returns></returns>
    Private Function GenerateMethod(Token As String) As CodeDom.CodeMemberMethod
        Dim Comments As String = Regex.Match(Token, "<div.+</div>", RegexOptions.Singleline).Value.GetValue

        Comments = Strings.Split(Comments, "</div>").First
        Token = Strings.Split(Token, "Throws:").First

        If InStr(Token, "Returns:") > 0 Then
            Token = Regex.Match(Token, "<h4>.+?</h4>.*?<pre>.+?>Returns:.+?</dd>", RegexOptions.Singleline).Value
        End If

        Dim ReturnsComment As String = Strings.Split(Regex.Match(Token, "<span class=""strong"">.*?Returns:</span>.*?</dt>.*?<dd>.+?</dd>", RegexOptions.Singleline).Value, "Returns:").Last
        Dim Name As String = Regex.Match(Token, "<h4>.+?</h4>", RegexOptions.Singleline).Value.GetValue
#If DEBUG Then
        Call Console.WriteLine($"[DEBUG ====> {NameOf(GenerateMethod)}: {Name}]")
#End If
        ReturnsComment = Regex.Match(ReturnsComment, "<dd>.+?</dd>", RegexOptions.Singleline).Value.GetValue

        Dim Def As String = Regex.Match(Token, "<pre>.+?</pre>", RegexOptions.Singleline).Value.Replace("&nbsp;", " ")

        Def = __trimDef(Def)

        Dim XmlComment As StringBuilder = New StringBuilder(XmlComments(Comments))
        Dim Tokens As String() = Def.Split

        If Tokens.Length < 2 Then
            Call Token.__DEBUG_ECHO
        End If

        Dim accessor As CodeDom.MemberAttributes = __buildAttrs(Tokens)
        Dim Method As New CodeDom.CodeMemberMethod With {.Attributes = accessor, .Name = Name}
        Dim ReturnType As String = Tokens(Scan0)
        Dim isArray As Boolean = InStr(ReturnType, "[]") > 0

        ReturnType = CodeGenerator.TrimType(ReturnType.GetValue)

        If Not String.Equals(ReturnType, Tokens(1)) Then
            If isArray Then
                ReturnType = ReturnType & "[]"
            End If
        End If

        Method.ReturnType = New CodeDom.CodeTypeReference(ReturnType)

        If InStr(Def, "()") > 0 Then '没有参数

        Else

            Dim Parameters As String() = (From m As Match
                                  In Regex.Matches(Token, "<dd>.*?<code>.+?</code>.+?</dd>", RegexOptions.Singleline)
                                          Select m.Value).ToArray
            Dim paraInfo = getParameters(Def)

            For Each pInfo As String In Parameters
                Name = Regex.Match(pInfo, "<code>.+?</code>").Value.GetValue

                If Not paraInfo.ContainsKey(Name) Then
                    Continue For
                Else
                    Call XmlComment.AppendLine()
                End If

                Dim Type As String = paraInfo(Name)

                Comments = Mid(pInfo, InStr(pInfo, " - ") + 3).Replace("</dd>", "").Trim
                Comments = String.Join(" ", (From s As String In Strings.Split(Comments.Replace(vbCr, ""), vbLf) Select Trim(s)).ToArray)

                Dim Parameter As New CodeDom.CodeParameterDeclarationExpression(New CodeDom.CodeTypeReference(Type), Name)
                Call Method.Parameters.Add(Parameter)

                Call XmlComment.Append($"'' <param name=""{Name}"">{ Comments }</param>")
            Next
        End If

        If Not String.Equals(GetType(Void).FullName, ReturnType) Then
            ReturnsComment = ReturnsComment.Replace(vbLf, "")
            ReturnsComment = String.Join(" ", (From s As String In Strings.Split(ReturnsComment, vbCr) Select Trim(s)).ToArray)

            Call XmlComment.AppendLine()
            Call XmlComment.Append($"'' <returns>{ReturnsComment}</returns>")
        End If
        Call Method.Comments.Add(New CodeDom.CodeCommentStatement(XmlComment.ToString))

        Return Method
    End Function

#End Region

#Region "CONSTRUCTOR DETAIL"

    Const DOC_CONSTRUCTOR_DETAIL As String = "CONSTRUCTOR DETAIL"

    Private Sub GenerateConstructors(ByRef clsDef As CodeDom.CodeTypeDeclaration, hash As Dictionary(Of String, String))
        If Not hash.ContainsKey(DOC_CONSTRUCTOR_DETAIL) Then
            Return
        End If

        Dim Doc As String = hash(CodeGenerator.DOC_CONSTRUCTOR_DETAIL)
        Const ConstructorDefRegex As String = "<h4>.+?</h4>.*?</li>"

        Dim Tokens As String() = (From m As Match In Regex.Matches(Doc, ConstructorDefRegex, RegexOptions.Singleline) Select m.Value).ToArray

        For Each constructor In (From s As String In Tokens Select GenerateConstructor(s)).ToArray
            Call clsDef.Members.Add(constructor)
        Next

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Token"></param>
    ''' <returns></returns>
    Private Function GenerateConstructor(Token As String) As CodeDom.CodeConstructor
        Dim Comments As String = Regex.Match(Token, "<div.+</div>", RegexOptions.Singleline).Value.GetValue
        Dim Parameters As String() = (From m As Match
                                      In Regex.Matches(Strings.Split(Token, "Throws:").First, "<dd>.*?<code>.+?</code>.+?</dd>", RegexOptions.Singleline)
                                      Select m.Value).ToArray
        Dim Def As String = Regex.Match(Token, "<pre>.+?</pre>", RegexOptions.Singleline).Value.Replace("&nbsp;", " ")
        Dim paraInfo = getParameters(Def)
        Dim XmlComment As StringBuilder = New StringBuilder(XmlComments(Comments))
        Dim attr As String = Def.Split.First.Replace("<pre>", "")
        Dim Constructor As New CodeDom.CodeConstructor With {.Attributes = CodeGenerator.GetMemberAccess(attr)}

        For Each pInfo As String In Parameters
            Dim Name As String = Regex.Match(pInfo, "<code>.+?</code>").Value.GetValue
            Dim Type As String = paraInfo(Name)

            Comments = Mid(pInfo, InStr(pInfo, " - ") + 3).Replace("</dd>", "").Trim

            Dim Parameter As New CodeDom.CodeParameterDeclarationExpression(New CodeDom.CodeTypeReference(Type), Name)
            Call Constructor.Parameters.Add(Parameter)

            Call XmlComment.AppendLine()
            Call XmlComment.Append($"'' <param name=""{Name}"">{Comments.Replace(vbCr, "").Replace(vbLf, "")}</param>")
        Next

        Call Constructor.Comments.Add(New CodeDom.CodeCommentStatement(XmlComment.ToString))

        Return Constructor
    End Function

    Private Function getParameters(Def As String) As Dictionary(Of String, String)
        Try
            Return CodeGenerator._getParameters(Def)
        Catch ex As Exception
            Throw New Exception($"[DEBUG  {NameOf(getParameters)} ----> {NameOf(Def)}]{vbCrLf}{vbCrLf}{ex.ToString}")
        End Try
    End Function

    Private Function _getParameters(Def As String) As Dictionary(Of String, String)
        Def = Regex.Match(Def, "\(.+?\)", RegexOptions.Singleline).Value

        Dim Tokens As String() = Strings.Split(Def.Replace(vbCr, ""), vbLf)
        Dim LQuery = (From s As String In Tokens
                      Let ss As String = Trim(s)
                      Where Not String.IsNullOrEmpty(ss)
                      Select getParameter(Mid(ss, 1, Len(ss) - 1).Trim)).ToArray
        Dim hash = LQuery.ToDictionary(Function(obj) obj.Key, elementSelector:=Function(obj) obj.Value)

        Return hash
    End Function

    Private Function getParameter(Token As String) As KeyValuePair(Of String, String)
        Dim Type As String = Token.href
        Dim Tokens As String() = Token.Split
        Dim Name As String = Tokens.Last

        If String.IsNullOrEmpty(Type) Then
            Type = Tokens.First

            If Type.First = "("c Then
                Type = Mid(Type, 2).Trim
            End If
        Else
            Type = href2FullName(Type)
        End If

        Type = TrimType(Type)

        Return New KeyValuePair(Of String, String)(Name, Type)
    End Function
#End Region

#Region "FIELD DETAIL"

    Const DOC_FIELD_DETAIL As String = "FIELD DETAIL"

    Private Sub GenerateFields(ByRef clsDef As CodeDom.CodeTypeDeclaration, hash As Dictionary(Of String, String))
        If Not hash.ContainsKey(DOC_FIELD_DETAIL) Then
            Return
        End If

        Dim Doc As String = hash(CodeGenerator.DOC_FIELD_DETAIL)

        Const FieldDefRegex As String = "<h4>.+?</h4>.*?<pre>.+?</pre>.*?<div class=""block"">.+?</div>"

        Dim Tokens As String() = (From m As Match In Regex.Matches(Doc, FieldDefRegex, RegexOptions.Singleline) Select m.Value).ToArray

        For Each Field As CodeDom.CodeMemberField In (From s As String In Tokens Select GenerateField(s)).ToArray
            Call clsDef.Members.Add(Field)
        Next
    End Sub

    Private Function GenerateField(Token As String) As CodeDom.CodeMemberField
        Dim Name As String = Regex.Match(Token, "<h4>.+?</h4>").Value.GetValue
        Dim Type As String = Regex.Match(Token, "<pre>.+?</pre>").Value.GetValue.Replace("&nbsp;", " ")
        Dim Comments As String = Regex.Match(Token, "<div Class=""block"">.+?</div>", RegexOptions.IgnoreCase).Value.GetValue

        Dim TypeFullName As String = Type.href
        Dim Tokens As String() = Type.Split
        Dim AccessLevel As CodeDom.MemberAttributes = __buildAttrs(Tokens)

        If Not String.IsNullOrEmpty(TypeFullName) Then
            Type = href2FullName(TypeFullName)
        Else
            Type = Tokens(Scan0)
        End If

        Dim FieldType As CodeDom.CodeTypeReference = New CodeDom.CodeTypeReference(TrimType(Type))
        Dim Field As New CodeDom.CodeMemberField(FieldType, Name) With {.Attributes = GetMemberAccess(AccessLevel)}

        Call Field.Comments.Add(New CodeDom.CodeCommentStatement(XmlComments(Comments)))

        Return Field
    End Function
#End Region

#Region "Supports Common"

    Private Function TrimType(type As String) As String
        type = type.Replace("&gt;", ")")
        type = type.Replace("&lt;", "(Of ")

        If InStr(type, ".") > 0 Then
            Return type '完整的类型名称，不做任何处理
        End If

        If InStr(type, "[]") > 0 Then
            Dim baseType As String = Mid(type, 1, InStr(type, "[") - 1)
            type = CodeGenerator.baseType(baseType) & Mid(type, InStr(type, "["))
            Return type
        End If

        type = CodeGenerator.baseType(type)
        Return type
    End Function

    Private Function baseType(type As String) As String
        Select Case type.ToLower
            Case "int" : Return GetType(Integer).FullName
            Case "float" : Return GetType(Double).FullName
            Case "void" : Return GetType(Void).FullName

            Case Else
                Return type
        End Select
    End Function

    Private Function GetMemberAccess(str As String) As CodeDom.MemberAttributes
        Select Case str.ToLower
            Case "protected" : Return CodeDom.MemberAttributes.Family
            Case "public" : Return CodeDom.MemberAttributes.Public
            Case "private" : Return CodeDom.MemberAttributes.Private
            Case "static" : Return CodeDom.MemberAttributes.Static
            Case "friend" : Return CodeDom.MemberAttributes.FamilyOrAssembly

            Case Else
                Call $"{str} is not implemented yet!".__DEBUG_ECHO
                Return CodeDom.MemberAttributes.VTableMask
        End Select
    End Function

    Private Function GetImplementsInterface(str As String) As String()
        Const Interfaces As String = "All Implemented Interfaces:</dt>.*?<dd>(<a .+?</a>)+</dd>.*?</dl>"

        str = Regex.Match(str, Interfaces, RegexOptions.Singleline).Value

        Dim Tokens As String() = (From m As Match In Regex.Matches(str, "<a .+?</a>") Select href2FullName(m.Value.href)).ToArray
        Return Tokens
    End Function

    Private Function XmlComments(str As String) As String
        Dim sbr As New StringBuilder(1024)
        Dim Tokens As String() = Strings.Split(str.Replace(vbCr, ""), vbLf)

        Call sbr.AppendLine("'' <summary>")
        For Each s As String In Tokens
            Call sbr.AppendLine("'' " & s)
        Next
        Call sbr.Append("'' </summary>")

        Return sbr.ToString
    End Function

    Private Function href2FullName(href As String) As String
        href = Regex.Match(href, "([/][a-z]+)+", RegexOptions.IgnoreCase).Value.Replace("/", ".")
        href = Mid(href, 2)
        Return href
    End Function

    Private Function GetSummaryComments(str As String, ByRef [inherits] As String) As String

        Const Summary As String = "<pre>.+?<span class=""strong"">.+?</span>.+?<div class=""block"">.+?</div>"

        str = Regex.Match(str, Summary, RegexOptions.Singleline).Value
        Dim Tokens As String() = Strings.Split(str, "</pre>")

        [inherits] = Tokens.First
        [inherits] = Regex.Match([inherits], "extends.+?<a href="".+?"" title="".+?"">.+?</a>").Value
        [inherits] = [inherits].href
        [inherits] = href2FullName([inherits])

        str = Tokens.Last
        str = Mid(str, InStr(str, ">") + 1).Trim

        Try
            str = Mid(str, 1, InStrRev(str, "<") - 1).Trim
        Catch ex As Exception

        End Try

        Return str
    End Function
#End Region
End Module
