#Region "Microsoft.VisualBasic::118749193baa7a0fb3af8ca1eeb84d43, GCModeller\analysis\Metagenome\Metagenome\ACE.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 100
    '    Code Lines: 79
    ' Comment Lines: 3
    '   Blank Lines: 18
    '     File Size: 3.52 KB


    ' Module ACE
    ' 
    '     Function: C_ACE, F, GammaACE, Nrare, S
    '               S_ACE, Sabund, Srare, SumFk
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting

''' <summary>
''' Ace – the ACE estimator (http://www.mothur.org/wiki/Ace)；用来估计群落中OTU 数目的指数，由Chao 提出，是生态学中估计物种总数的常用指数之一
''' </summary>
Public Module ACE

    <Extension> Public Function Srare(OTUs As OTUTable(), groups As NamedVectorFactory) As Vector
        Return S(OTUs, groups, 1, 10)
    End Function

    Public Function S(OTUs As OTUTable(), groups As NamedVectorFactory, min%, max%) As Vector
        Dim n As Vector = groups.EmptyVector
        Dim fk As Dictionary(Of String, Double)

        For k As Integer = min To max
            fk = F(OTUs, groups.Keys, k)
            n += groups.AsVector(fk)
        Next

        Return n
    End Function

    Public Function Sabund(OTUs As OTUTable(), groups As NamedVectorFactory) As Vector
        Dim Sobs As Double = OTUs _
            .Select(Function(otu) otu.Properties.Values) _
            .IteratesALL _
            .Max
        Return S(OTUs, groups, 11, Sobs)
    End Function

    Public Function Nrare(OTUs As OTUTable(), groups As NamedVectorFactory) As Vector
        Dim n As Vector = groups.EmptyVector
        Dim fk As Dictionary(Of String, Double)

        For k As Integer = 1 To 10
            fk = F(OTUs, groups.Keys, k)
            n += k * groups.AsVector(fk)
        Next

        Return n
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

    Public Function C_ACE(OTUs As OTUTable(), groups As NamedVectorFactory) As Vector
        Dim f1 = groups.AsVector(F(OTUs, groups.Keys, k:=1))
        Dim n = Nrare(OTUs, groups)
        Dim c = 1 - f1 / n
        Return c
    End Function

    Public Function S_ACE(OTUs As OTUTable(), groups$()) As Dictionary(Of String, Double)
        Dim factors As New NamedVectorFactory(groups)
        Dim Sa = Sabund(OTUs, factors)
        Dim Sr = Srare(OTUs, factors)
        Dim C = C_ACE(OTUs, factors)
        Dim f1 = factors.AsVector(F(OTUs, groups, k:=1))
        Dim n = Nrare(OTUs, factors)
        Dim gamma = GammaACE(OTUs, factors, Sr, C, n)
        Dim ACE = Sa + Sr / C + f1 / C * gamma ^ 2
        Return factors.Translate(ACE)
    End Function

    Public Function GammaACE(OTUs As OTUTable(), groups As NamedVectorFactory, S As Vector, C As Vector, n As Vector) As Vector
        Dim k = SumFk(OTUs, groups)
        Dim x = S / C * k / (C * n * (n - 1)) - 1
        Dim maxGamma = Vector.Max(x, 0)
        Return maxGamma
    End Function

    Private Function SumFk(OTUs As OTUTable(), groups As NamedVectorFactory) As Vector
        Dim v As Vector = groups.EmptyVector
        Dim fk As Dictionary(Of String, Double)

        For k As Integer = 1 To 10
            fk = F(OTUs, groups.Keys, k)
            v += k * (k - 1) * groups.AsVector(fk)
        Next

        Return v
    End Function
End Module
