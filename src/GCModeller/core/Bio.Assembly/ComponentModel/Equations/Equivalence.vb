Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions

Namespace ComponentModel.EquaionModel

    Public Module Equivalence

        <Extension>
        Public Function Equals(Of T As ICompoundSpecies)(a As T, b As T, strict As Boolean) As Boolean
            If a.StoiChiometry <> b.StoiChiometry Then
                If strict Then
                    Return False
                End If
            End If

            If strict Then
                Return String.Equals(b.Identifier, a.Identifier, StringComparison.Ordinal)
            Else
                Return String.Equals(b.Identifier, a.Identifier, StringComparison.OrdinalIgnoreCase)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <param name="cpEquals">反应物的等价性的判断方法</param>
        ''' <param name="strict"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Equals(Of T As ICompoundSpecies)(a As Equation(Of T),
                                                         b As Equation(Of T),
                                                         cpEquals As Func(Of T, T, Boolean, Boolean),
                                                         Optional strict As Boolean = False) As Boolean
            If a.Reversible <> b.Reversible Then
                If strict Then
                    Return False
                End If
            End If

            Dim equas = (From x In a.Reactants
                         Let my = (From m In b.Reactants Where cpEquals(m, x, strict) Select m).FirstOrDefault
                         Where my Is Nothing  ' 说明有一些是不同的，当前的代谢物没有相同的
                         Select x).FirstOrDefault
            If Not equas Is Nothing Then ' 查找到了不同的
                If strict Then
                    Return False
                Else
                    Return __reverseEquals(a, b, cpEquals, strict)
                End If
            Else  ' 可能是反向的
                equas = (From x In a.Products
                         Let my = (From m In b.Products Where cpEquals(m, x, strict) Select m).FirstOrDefault
                         Where my Is Nothing  ' 说明有一些是不同的
                         Select x).FirstOrDefault
                If Not equas Is Nothing Then ' 查找到了不同的
                    Return False
                Else
                    Return True
                End If
            End If
        End Function

        Private Function __reverseEquals(Of T As ICompoundSpecies)(a As Equation(Of T),
                                                                   b As Equation(Of T),
                                                                   cpEquals As Func(Of T, T, Boolean, Boolean),
                                                                   strict As Boolean) As Boolean
            Dim equas = (From x In a.Products
                         Let my = (From m In b.Reactants Where cpEquals(m, x, strict) Select m).FirstOrDefault
                         Where my Is Nothing  ' 说明有一些是不同的
                         Select x).FirstOrDefault
            If Not equas Is Nothing Then
                Return False
            End If

            equas = (From x In a.Reactants
                     Let my = (From m In b.Products Where cpEquals(m, x, strict) Select m).FirstOrDefault
                     Where my Is Nothing  ' 说明有一些是不同的
                     Select x).FirstOrDefault
            If Not equas Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function
    End Module
End Namespace