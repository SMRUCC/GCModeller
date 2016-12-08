#Region "Microsoft.VisualBasic::59c15cf3e211b70eaa850f9fe702b92b, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\LINQStatement.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports sciBASIC.ComputingServices.Linq.Framework.DynamicCode
Imports sciBASIC.ComputingServices.Linq.Framework.Provider
Imports sciBASIC.ComputingServices.Linq.LDM.Statements.Tokens

Namespace LDM.Statements

    ''' <summary>
    ''' A linq statement object model.
    ''' </summary>
    ''' <remarks>
    ''' From [Object [As TypeId]] 
    ''' In [Collection] 
    ''' Let [Declaration1, Declaration2, ...]
    ''' Where [Condition Test] 
    ''' Select [Object/Object Constrctor] 
    ''' [Distinct] 
    ''' [Order Statement]</remarks>
    Public Class LinqStatement

        ''' <summary>
        ''' An object element in the target query collection.(目标待查询集合之中的一个元素)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property var As FromClosure
        ''' <summary>
        ''' Target query collection expression, this can be a file path or a database connection string.
        ''' (目标待查询集合，值可以为一个文件路径或者数据库连接字符串)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property source As InClosure
        ''' <summary>
        ''' A read only object collection which were construct by the LET statement token in the LINQ statement.
        ''' (使用Let语句所构造出来的只读对象类型的对象申明集合)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PreDeclare As LetClosure()
        ''' <summary>
        ''' Where test condition for the query.(查询所使用的Where条件测试语句)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Where As WhereClosure
        Public Property AfterDeclare As LetClosure()
        ''' <summary>
        ''' A expression for return the query result.(用于生成查询数据返回的语句)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SelectClosure As SelectClosure

        ''' <summary>
        ''' 获取目标LINQCollection待查询集合中的元素对象的类型标识符，以进行外部模块的动态加载
        ''' 与RegistryItem中的Name属性值相对应
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeId As String
            Get
                Return var.TypeId
            End Get
        End Property

        ''' <summary>
        ''' Original statement text of this linq expression
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Text As String

        Public Overrides Function ToString() As String
            Return Text
        End Function

        Public ReadOnly Property CompiledCode As String
            Get
                Dim code As String =
                    LinqClosure.BuildClosure(var.Name,
                                             var.TypeId,
                                             PreDeclare.ToArray(Function(x) x.Code),
                                             AfterDeclare.ToArray(Function(x) x.Code),
                                             SelectClosure.Projects,
                                             Where.Code)
                Return code
            End Get
        End Property

        ''' <summary>
        ''' Try to parsing a linq query script into a statement object model and compile the model into a assembly dynamic.
        ''' (尝试着从所输入的命令语句中解析出一个LINQ查询命令对象，并完成动态编译过程)
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function TryParse(source As String, Optional types As TypeRegistry = Nothing) As LinqStatement
            Dim tokens As ClosureTokens() = ClosureParser.TryParse(source)
            Dim statement As New LinqStatement With {
                ._Text = source
            }
            Return __innerParser(tokens, types, statement)
        End Function

        Private Shared Function __innerParser(tokens As ClosureTokens(), types As TypeRegistry, linq As LinqStatement) As LinqStatement
            linq.var = New FromClosure(tokens, linq)
            linq.source = InClosure.CreateObject(tokens, linq)
            linq.PreDeclare = GetPreDeclare(tokens, linq, types)
            linq.Where = New WhereClosure(tokens, linq)
            linq.AfterDeclare = GetAfterDeclare(tokens, linq, types)
            linq.SelectClosure = New SelectClosure(tokens, linq)

            Return linq
        End Function

        Public Shared Function TryParse(source As IEnumerable(Of Token(Of TokenIcer.Tokens)),
                                        Optional types As TypeRegistry = Nothing,
                                        Optional text As String = "") As LinqStatement
            Dim tokens As ClosureTokens() = ClosureParser.TryParse(source)
            Dim statement As New LinqStatement With {
                ._Text = text
            }

            If String.IsNullOrEmpty(text) Then
                statement._Text = String.Join(" ", source.ToArray(Function(x) x.Text))
            End If

            Return __innerParser(tokens, types, statement)
        End Function
    End Class
End Namespace
