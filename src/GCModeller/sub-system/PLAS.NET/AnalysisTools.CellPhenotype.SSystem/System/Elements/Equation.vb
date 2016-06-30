Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.CellPhenotype.SSystem.Script
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel
Imports Microsoft.VisualBasic

Namespace Kernel.ObjectModels

    Public Class Equation : Inherits Expression

        Dim sBuilder As StringBuilder = New StringBuilder(1024)

        ''' <summary>
        ''' 使用代谢底物的UniqueID属性值作为数值替代的表达式
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Expression As String

        Friend Kernel As Kernel

        ''' <summary>
        ''' The target that associated with this channel.
        ''' (与本计算通道相关联的目标对象)
        ''' </summary>
        ''' <remarks></remarks>
        Friend Var As Var

        Sub New(s As SEquation)
            Me.Model = s
            Me.Expression = s.Expression
            Me.Identifier = s.x
        End Sub

        Sub New(id As String, expr As String)
            Call Me.New(New SEquation(id, expr))
        End Sub

        ''' <summary>
        ''' Evaluate the expression value of the property <see cref="Equation.Expression"></see>.(计算<see cref="Equation.Expression"></see>属性表达式的值)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Evaluate() As Double
            Call Me.sBuilder.Clear()

            sBuilder.Append(Me._Expression)

            For Each e In Kernel.Vars 'Replace the name using the value
                Call sBuilder.Replace(e.UniqueId, e.Value)
            Next

            Dim rtvl As Double = Microsoft.VisualBasic.Mathematical.Expression.Evaluate(sBuilder.ToString)
            Return rtvl
        End Function

        Public Overrides ReadOnly Property Value As Double
            Get
                Return Me.Var.Value
            End Get
        End Property

        Public Function Elapsed() As Boolean
            Var.Value += (Me.Evaluate * 0.1)
            Call Microsoft.VisualBasic.Mathematical.ScriptEngine.SetVariable(Var.UniqueId, Var.Value)

            Return True
        End Function

        Public Overrides Function ToString() As String
            If Var Is Nothing Then
                Return String.Format("{0}'={1}", Identifier, Expression)
            Else
                Return String.Format("{0}; //{1}'={2}", Var.ToString, Identifier, Expression)
            End If
        End Function

        ''' <summary>
        ''' Set up the simulation kernel.
        ''' (设置模拟核心)
        ''' </summary>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Sub [Set](e As Kernel)
            Dim Query As Generic.IEnumerable(Of Var) = From o As Var In e.Vars Where String.Equals(o.UniqueId, Identifier) Select o '

            Kernel = e
            Var = Query.First
        End Sub

        Public ReadOnly Property Model As SEquation

        Public Overrides Function get_ObjectHandle() As ObjectHandle
            Return New ObjectHandle With {
                .Identifier = Identifier,
                .Handle = Handle
            }
        End Function
    End Class
End Namespace