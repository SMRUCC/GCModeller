Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.SBML.Level2.Elements
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver

Namespace Kernel.ObjectModels

    Public Class Var : Inherits Variable

        <XmlAttribute> Public Property Title As String
        <XmlElement> Public Property Comment As String

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Comment) Then
                Return String.Format("{0}={1}", IIf(Len(Title) > 0, Title, UniqueId), Value)
            Else
                Return String.Format("{0}={1}; //{2}", IIf(Len(Title) > 0, Title, UniqueId), Value, Comment)
            End If
        End Function

        Public Shared Narrowing Operator CType(e As Var) As Double
            Return e.Value
        End Operator

        Public Shared Narrowing Operator CType(e As Var) As String
            Return IIf(Len(e.Title) > 0, e.Title, e.UniqueId)
        End Operator

        Public Shared Widening Operator CType(e As Specie) As Var
            Return New Var With {
                .UniqueId = e.ID,
                .Title = e.name,
                .Value = Val(e.InitialAmount)
            }
        End Operator

        Public Shared Function TryParse(strData As String) As Var
            Return CType(strData, Var)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s">Script line.(脚本行文本)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Widening Operator CType(s As String) As Var
            Dim Tokens As String() = Mid(s, 6).Split(CChar("="))
            Return New Var With {
                .UniqueId = Tokens.First.Trim,
                .Value = Val(Tokens.Last)
            }
        End Operator
    End Class
End Namespace