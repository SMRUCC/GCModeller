#Region "Microsoft.VisualBasic::f6474d0e749a6ff95aaf41a691dc2605, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\ParserAPI\Parser.vb"

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

Imports System.CodeDom
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports Microsoft.VisualBasic.Linq.LDM.Statements.TokenIcer
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace LDM.Parser

    ''' <summary>
    ''' Parses expressions written in strings into CodeDom expressions.  There is a certain 
    ''' amount of context that the parser may need to be familiar with.  This is why the 
    ''' parsing methods are not exposed as static.
    ''' </summary>
    Public Class Parser : Implements System.IDisposable

        Private _Enums As Dictionary(Of String, CodeTypeReference) = New Dictionary(Of String, CodeTypeReference)
        Private _Fields As StringCollection = New StringCollection

        ''' <summary>
        ''' A collection of identifiers that should be recognized as enums.
        ''' </summary>
        Public ReadOnly Property Enums() As Dictionary(Of String, CodeTypeReference)
            Get
                Return _Enums
            End Get
        End Property

        ''' <summary>
        ''' A collection of names of fields.
        ''' </summary>
        Public ReadOnly Property Fields() As StringCollection
            Get
                Return _Fields
            End Get
        End Property

        ''' <summary>
        ''' Parses an expression into a <see cref="CodeExpression"/>.
        ''' </summary>
        ''' <param name="exp">expression to parse</param>
        ''' <returns>CodeDom representing the expression</returns>
        Public Function ParseExpression(exp As String) As CodeExpression
            Return ParseExpression(New Tokenizer(exp))
        End Function

        ''' <summary>
        ''' Parses an expression into a <see cref="CodeExpression"/>.
        ''' </summary>
        ''' <param name="t">expression to parse</param>
        ''' <returns>CodeDom representing the expression</returns>
        Public Function ParseExpression(t As Tokenizer) As CodeExpression
            If Not t.IsInvalid Then
                t.GetNextToken()
                Return ReadExpression(t, TokenPriority.None)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Recursive method that reads an expression.
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="priority"></param>
        ''' <returns></returns>
        Private Function ReadExpression(t As Tokenizer, priority As TokenPriority) As CodeExpression
            Dim left As CodeExpression = Nothing, right As CodeExpression = Nothing
            Dim cont As Boolean = True, applyNot As Boolean = False, applyNegative As Boolean = False
            While cont
                If t.Current.Type = Primitive Then
                    left = New CodePrimitiveExpression(t.Current.ParsedObject)
                    t.GetNextToken()
                    cont = False
                ElseIf t.Current.Type.IsOperator Then
                    ' t.DirectlyMoveNext()

                    ' An operator here is considered a unary operator.
                    Select Case t.Current.Text
                        Case "-"
                            applyNegative = True
                        Case "!"
                            applyNot = True
                        Case Else
                            ' Throw New Exception("Unexpected operator: " & t.Current.Text)
                    End Select

                    If t.GetNextToken().Type.IsOperator Then
                        Call t.DirectlyMoveNext()
                    End If

                    Continue While
                ElseIf t.Current.Type = Tokens.Identifier Then
                    left = ReadIdentifier(t)
                    cont = False
                ElseIf t.Current.Type = OpenParens Then
                    t.GetNextToken()
                    left = ReadExpression(t, TokenPriority.None)
                    t.GetNextToken()
                    If TypeOf left Is CodeTypeReferenceExpression Then
                        left = New CodeCastExpression(TryCast(left, CodeTypeReferenceExpression).Type, ReadExpression(t, TokenPriority.None))
                    End If
                    cont = False
                Else
                    t.GetNextToken()
                End If
                If t.IsInvalid Then
                    cont = False
                End If
            End While
            If left Is Nothing Then
                Throw New Exception("No expression found.")
            End If
            If applyNot Then
                left = New CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.ValueEquality, New CodePrimitiveExpression(False))
            ElseIf applyNegative Then
                left = New CodeBinaryOperatorExpression(New CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, left)
            End If
            If t.IsInvalid OrElse t.Current.Type = CloseParens OrElse t.Current.Type = Comma OrElse t.Current.Type = Tokens.CloseBracket Then
                Return left
            End If
            cont = True
            While Not t.IsInvalid ' cont AndAlso Not t.IsInvalid
                Dim token As Token = t.Current
                If token.Type.IsOperator Then
                    If t.Current.Priority < priority Then
                        Exit While
                    Else
                        ' In the case we have an operator, we'll assume it's a binary operator.
                        Dim binOp As CodeBinaryOperatorType
                        Dim notEquals As Boolean = False
                        Select Case token.Text
                            Case ">"
                                binOp = CodeBinaryOperatorType.GreaterThan
                            Case ">="
                                binOp = CodeBinaryOperatorType.GreaterThanOrEqual
                            Case "<"
                                binOp = CodeBinaryOperatorType.LessThan
                            Case "<="
                                binOp = CodeBinaryOperatorType.LessThanOrEqual
                            Case "=", "=="
                                binOp = CodeBinaryOperatorType.ValueEquality
                            Case "!="
                                binOp = CodeBinaryOperatorType.ValueEquality
                                notEquals = True
                            Case "|"
                                binOp = CodeBinaryOperatorType.BitwiseOr
                            Case "||"
                                binOp = CodeBinaryOperatorType.BooleanOr
                            Case "&"
                                binOp = CodeBinaryOperatorType.BitwiseAnd
                            Case "&&"
                                binOp = CodeBinaryOperatorType.BooleanAnd
                            Case "-"
                                binOp = CodeBinaryOperatorType.Subtract
                            Case "+"
                                binOp = CodeBinaryOperatorType.Add
                            Case "/"
                                binOp = CodeBinaryOperatorType.Divide
                            Case "%"
                                binOp = CodeBinaryOperatorType.Modulus
                            Case "*"
                                binOp = CodeBinaryOperatorType.Multiply
                            Case Else
                                Throw New Exception("Unrecognized operator: " & t.Current.Text)
                        End Select
                        If t.IsInvalid Then
                            Throw New Exception("Expected token for right side of binary expression.")
                        End If
                        t.DirectlyMoveNext()
                        right = ReadExpression(t, token.Priority)
                        left = New CodeBinaryOperatorExpression(left, binOp, right)

                        ' If the operator was the not equals operator, we just negate the previous binary expression.
                        If notEquals Then
                            left = New CodeBinaryOperatorExpression(left, binOp, New CodePrimitiveExpression(False))
                        End If
                    End If
                ElseIf Token.Type = CloseParens Then
                    't.GetNextToken();
                    cont = False
                ElseIf Token.Type = Dot Then
                    ' A dot could appear after some parentheses.  In this case we need to parse 
                    ' what's after the dot as an identifier.
                    t.GetNextToken()
                    right = ReadIdentifier(t)
                    Dim ceTemp As CodeExpression = right
                    While True
                        If TypeOf ceTemp Is CodeVariableReferenceExpression Then
                            left = New CodePropertyReferenceExpression(left, TryCast(ceTemp, CodeVariableReferenceExpression).VariableName)
                            Exit While
                        ElseIf TypeOf ceTemp Is CodePropertyReferenceExpression Then
                            Dim cpre As CodePropertyReferenceExpression = TryCast(ceTemp, CodePropertyReferenceExpression)
                            If TypeOf cpre.TargetObject Is CodeThisReferenceExpression Then
                                cpre.TargetObject = left
                                left = cpre
                                Exit While
                            Else
                                ceTemp = cpre.TargetObject
                            End If
                        ElseIf TypeOf ceTemp Is CodeFieldReferenceExpression Then
                            Dim cfre As CodeFieldReferenceExpression = TryCast(ceTemp, CodeFieldReferenceExpression)
                            If TypeOf cfre.TargetObject Is CodeThisReferenceExpression Then
                                cfre.TargetObject = left
                                left = cfre
                                Exit While
                            End If
                        ElseIf TypeOf ceTemp Is CodeMethodInvokeExpression Then
                            Dim cmie As CodeMethodInvokeExpression = TryCast(ceTemp, CodeMethodInvokeExpression)
                            If TypeOf cmie.Method.TargetObject Is CodeThisReferenceExpression Then
                                cmie.Method.TargetObject = left
                                left = cmie
                                Exit While
                            Else
                                ceTemp = cmie.Method.TargetObject
                            End If
                        Else
                            Throw New Exception("Unexpected identifier found after .")
                        End If
                    End While
                    cont = False
                Else
                    Call t.GetNextToken()
                    Exit While
                End If
            End While
            Return left
        End Function

        ''' <summary>
        ''' When an identifier is encountered, it could be a number of things.  A single identifer by itself
        ''' is considered a variable.  The pattern identifier[.identifier]+ will consider the 
        ''' first identifier as a variable and the others as properties.  Any identifier that is followed
        ''' by an open parenthesis is considered to be a function call.  Indexes are not handled yet, but
        ''' should be handled in the future.  If the identifier is "this" then a this reference is used.
        ''' </summary>
        ''' <param name="t"></param>
        ''' <returns></returns>
        Private Function ReadIdentifier(t As Tokenizer) As CodeExpression
            Dim ce As CodeExpression = Nothing
            Dim token As Token = t.Current
            ce = New CodeVariableReferenceExpression(token.Text)
            token = t.GetNextToken()
            Return ce
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' �������ĵ���

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  �ͷ��й�״̬(�йܶ���)��
                End If

                ' TODO:  �ͷŷ��й���Դ(���йܶ���)����д����� Finalize()��
                ' TODO:  �������ֶ�����Ϊ null��
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  ��������� Dispose(ByVal disposing As Boolean)�����ͷŷ��й���Դ�Ĵ���ʱ��д Finalize()��
        'Protected Overrides Sub Finalize()
        '    ' ��Ҫ���Ĵ˴��롣    �뽫��������������� Dispose(ByVal disposing As Boolean)�С�
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic ��Ӵ˴�����Ϊ����ȷʵ�ֿɴ���ģʽ��
        Public Sub Dispose() Implements IDisposable.Dispose
            ' ��Ҫ���Ĵ˴��롣    �뽫��������������� Dispose (disposing As Boolean)�С�
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
