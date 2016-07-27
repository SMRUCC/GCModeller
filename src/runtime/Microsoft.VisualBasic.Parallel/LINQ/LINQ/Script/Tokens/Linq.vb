Imports Microsoft.VisualBasic.Linq.Framework.Provider
Imports Microsoft.VisualBasic.Linq.LDM.Statements
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Script.Tokens

    Public MustInherit Class TokenBase

        Protected ReadOnly __source As Token(Of TokenIcer.Tokens)()

        Sub New(source As IEnumerable(Of Token(Of TokenIcer.Tokens)))
            __source = source.ToArray
        End Sub
    End Class

    ''' <summary>
    ''' Value assignment statement for assign the value the a variable in the LINQ script runtime.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Linq : Inherits TokenBase

        ''' <summary>
        ''' Variable name
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Name As String
        Public ReadOnly Property Linq As LinqStatement

        Sub New(source As IEnumerable(Of Token(Of TokenIcer.Tokens)), types As TypeRegistry)
            Call MyBase.New(source)

            If String.Equals(__source(Scan0).Text, "var", StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(__source(2).Text, "=") Then
                source = source.Skip(1)
            End If

            Name = source(Scan0).Text
            Linq = LinqStatement.TryParse(source.Skip(2), types)
        End Sub

        Public Overrides Function ToString() As String
            Return $"var {Name} = {Linq.ToString}"
        End Function

        Public Shared Narrowing Operator CType(linq As Linq) As LinqStatement
            Return linq.Linq
        End Operator
    End Class
End Namespace