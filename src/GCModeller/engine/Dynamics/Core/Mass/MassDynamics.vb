Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

Namespace Core

    ''' <summary>
    ''' Convert <see cref="Channel"/> matrix to mass equations
    ''' </summary>
    Public Class MassDynamics : Inherits var
        Implements IReadOnlyId, INonlinearVar

        ''' <summary>
        ''' <see cref="Factor.ID"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property Name As String Implements IReadOnlyId.Identity
            Get
                Return mass.ID
            End Get
            Set(value As String)
                ' set name is not allowed
            End Set
        End Property

        Public Overrides Property Value As Double Implements Ivar.value
            Get
                Return mass.Value
            End Get
            Set(value As Double)
                If Not mass Is Nothing Then
                    mass.Value = value
                End If
            End Set
        End Property

        Dim channels As Channel()
        ''' <summary>
        ''' 这个因子向量是相对于反应过程从左到右而言的
        ''' </summary>
        Dim factors As Double()
        Dim mass As Factor
        Dim shareFactors As (left As Dictionary(Of String, Double), right As Dictionary(Of String, Double))
        Dim fluxVariants As var()

        Public Function Evaluate() As Double Implements INonlinearVar.Evaluate
            Dim additions As Double
            Dim dir As Directions
            Dim variants As Double
            Dim flux As Channel
            Dim fluxVariant As Double

            For i As Integer = 0 To channels.Length - 1
                flux = channels(i)
                dir = flux.direct

                Select Case dir
                    Case Directions.forward
                        variants = flux.forward.coefficient - flux.reverse.coefficient
                        fluxVariant = flux.CoverLeft(shareFactors.left, variants)
                        variants = factors(i) * fluxVariant
                    Case Directions.reverse
                        variants = flux.reverse.coefficient - flux.forward.coefficient
                        fluxVariant = -flux.CoverRight(shareFactors.right, variants)
                        variants = factors(i) * fluxVariant
                    Case Directions.stop
                        variants = 0
                        fluxVariant = 0
                    Case Else
                        Throw New InvalidProgramException
                End Select

                additions += variants
                fluxVariants(i).Value = fluxVariant
            Next

            Return additions
        End Function

        Public Function getLastFluxVariants() As IEnumerable(Of var)
            Return fluxVariants
        End Function

        Public Overrides Function ToString() As String
            Return mass.ToString
        End Function

        Private Shared Function createMassIndex(channels As Channel()) As Dictionary(Of String, List(Of Channel))
            Dim index As New Dictionary(Of String, List(Of Channel))

            For Each flux As Channel In channels
                For Each m In flux.GetReactants.JoinIterates(flux.GetProducts)
                    If Not index.ContainsKey(m.mass.ID) Then
                        Call index.Add(m.mass.ID, New List(Of Channel))
                    End If

                    Call index(m.mass.ID).Add(flux)
                Next
            Next

            Return index
        End Function

        ''' <summary>
        ''' create dynamics equation for RK4 ODEs solver
        ''' </summary>
        ''' <param name="env"></param>
        ''' <returns></returns>
        Public Shared Iterator Function PopulateDynamics(env As Vessel) As IEnumerable(Of MassDynamics)
            Dim factors As New List(Of Double)
            Dim matter As Variable
            Dim massIndex = createMassIndex(env.Channels)
            Dim channels As Channel()

            For Each mass As Factor In env.m_massIndex.Values
                factors.Clear()
                channels = massIndex(mass.ID).ToArray

                For Each flux As Channel In channels
                    matter = flux.GetReactants _
                        .Where(Function(a) a.mass.ID = mass.ID) _
                        .FirstOrDefault

                    If Not matter Is Nothing Then
                        ' 反应物端是消耗当前代谢物
                        ' 所以相关系数为负数值
                        factors.Add(-matter.coefficient)
                    Else
                        matter = flux.GetProducts _
                            .Where(Function(a) a.mass.ID = mass.ID) _
                            .First
                        ' 反应产物端则是增加当前代谢物
                        ' 相关系数为正实数
                        factors.Add(matter.coefficient)
                    End If
                Next

                Yield New MassDynamics With {
                    .mass = mass,
                    .factors = factors.ToArray,
                    .channels = channels,
                    .shareFactors = env.shareFactors,
                    .fluxVariants = channels _
                        .Select(Function(a, i)
                                    Return New var With {
                                        .Index = i,
                                        .Name = a.ID,
                                        .Value = 0
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function
    End Class
End Namespace