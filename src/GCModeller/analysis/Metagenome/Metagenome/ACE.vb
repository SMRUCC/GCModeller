Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' Ace – the ACE estimator (http://www.mothur.org/wiki/Ace)；用来估计群落中OTU 数目的指数，由Chao 提出，是生态学中估计物种总数的常用指数之一
''' </summary>
Public Module ACE

    <Extension> Public Function Srare(OTUs As OTUTable(), groups$()) As Dictionary(Of String, Double)
        Return S(OTUs, groups, 1, 10)
    End Function

    Public Function S(OTUs As OTUTable(), groups$(), min%, max%) As Dictionary(Of String, Double)
        Dim n As Dictionary(Of String, Double) = groups.ToDictionary(Function(name) name, Function(x) 0R)

        For k As Integer = min To max
            Dim fk = F(OTUs, groups, k)

            For Each name In groups
                n(name) += fk(name)
            Next
        Next

        Return n
    End Function

    Public Function Sabund(OTUs As OTUTable(), groups$()) As Dictionary(Of String, Double)
        Dim Sobs As Double = OTUs _
            .Select(Function(otu) otu.Properties.Values) _
            .IteratesALL _
            .Max
        Return S(OTUs, groups, 11, Sobs)
    End Function

    Public Function Nrare(OTUs As OTUTable(), groups As NamedVectorFactory) As Dictionary(Of String, Double)
        Dim n As Vector = groups.EmptyVector
        Dim fk As Dictionary(Of String, Double)

        For k As Integer = 1 To 10
            fk = F(OTUs, groups.Keys, k)
            n += k * groups.AsVector(fk)
        Next

        Return groups.Translate(n)
    End Function

    Public Function F(OTUs As OTUTable(), groups$(), k#) As Dictionary(Of String, Double)
        Dim fk As Dictionary(Of String, Double) = groups _
            .ToDictionary(Function(name) name,
                          Function(x) 0R)

        For Each OTU In OTUs
            For Each name In groups
                If OTU(name) = k Then
                    fk(name) += 1
                End If
            Next
        Next

        Return fk
    End Function

    Public Function C_ACE(OTUs As OTUTable(), groups$()) As Dictionary(Of String, Double)
        Dim f1 = F(OTUs, groups, k:=1)
        Dim n = Nrare(OTUs, groups)
        Dim c = groups.ToDictionary(Function(name) name,
                                    Function(g) 1 - f1(g) / n(g))
        Return c
    End Function

    Public Function S_ACE(OTUs As OTUTable(), groups$()) As Dictionary(Of String, Double)
        Dim Sa = Sabund(OTUs, groups)
        Dim Sr = Srare(OTUs, groups)
        Dim C = C_ACE(OTUs, groups)
        Dim f1 = F(OTUs, groups, k:=1)
        Dim n = Nrare(OTUs, groups)
        Dim gamma = GammaACE(OTUs, groups, Sr, C, n)
        Dim ACE = groups.ToDictionary(
            Function(group) group,
            Function(g)
                Return Sa(g) + Sr(g) / C(g) + f1(g) / C(g) * gamma(g) ^ 2
            End Function)
        Return ACE
    End Function

    Public Function GammaACE(OTUs As OTUTable(), groups$(), S As Dictionary(Of String, Double), C As Dictionary(Of String, Double), n As Dictionary(Of String, Double)) As Dictionary(Of String, Double)
        Dim k = SumFk(OTUs, groups)
        Dim x = groups.ToDictionary(
            Function(name) name,
            Function(g)
                Return S(g) / C(g) * k(g) / (C(g) * n(g) * (n(g) - 1)) - 1
            End Function)
        Dim maxGamma = x.ToDictionary(Function(g) g.Key, Function(g) Math.Max(g.Value, 0))
        Return maxGamma
    End Function

    Private Function SumFk(OTUs As OTUTable(), groups$()) As Dictionary(Of String, Double)
        Dim value = groups.ToDictionary(Function(name) name, Function() 0R)

        For k As Integer = 1 To 10
            Dim fk = F(OTUs, groups, k)

            For Each name In groups
                value(name) += k * (k - 1) * fk(name)
            Next
        Next

        Return value
    End Function
End Module
