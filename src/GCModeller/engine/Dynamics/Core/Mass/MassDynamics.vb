Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq

Namespace Core

    ''' <summary>
    ''' Convert <see cref="Channel"/> matrix to mass equations
    ''' </summary>
    Public Class MassDynamics : Implements IReadOnlyId

        Public ReadOnly Property massID As String Implements IReadOnlyId.Identity
            Get
                Return mass.ID
            End Get
        End Property

        Dim channels As Channel()
        ''' <summary>
        ''' 这个因子向量是相对于反应过程从左到右而言的
        ''' </summary>
        Dim factors As Double()
        Dim mass As Factor

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

        Public Shared Iterator Function PopulateDynamics(massEnv As Factor(), channels As Channel()) As IEnumerable(Of MassDynamics)
            Dim factors As New List(Of Double)
            Dim matter As Variable
            Dim massIndex = createMassIndex(channels)

            For Each mass As Factor In massEnv
                factors.Clear()
                channels = massIndex(mass.ID).ToArray

                For Each flux As Channel In channels
                    matter = flux.GetReactants _
                        .Where(Function(a) a.mass.ID = mass.ID) _
                        .FirstOrDefault

                    If Not matter Is Nothing Then
                        factors.Add(matter.coefficient)
                    Else
                        matter = flux.GetProducts _
                            .Where(Function(a) a.mass.ID = mass.ID) _
                            .First
                        factors.Add(matter.coefficient)
                    End If
                Next

                Yield New MassDynamics With {
                    .mass = mass,
                    .factors = factors.ToArray,
                    .channels = channels
                }
            Next
        End Function
    End Class
End Namespace