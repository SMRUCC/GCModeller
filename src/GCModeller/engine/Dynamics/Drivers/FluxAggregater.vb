#Region "Microsoft.VisualBasic::99da6a58e920530fd260cd71a432fac8, GCModeller\engine\Dynamics\Drivers\FluxAggregater.vb"

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

    '   Total Lines: 27
    '    Code Lines: 22
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 932 B


    '     Class FluxAggregater
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: getFlux
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Engine

    Public Class FluxAggregater

        ReadOnly fluxDynamicsCache As IGrouping(Of String, var)()

        Sub New(core As Vessel)
            fluxDynamicsCache = core.m_dynamics _
                .Select(Function(m) m.getLastFluxVariants) _
                .IteratesALL _
                .GroupBy(Function(a) a.Name) _
                .ToArray
        End Sub

        Public Function getFlux() As Dictionary(Of String, Double)
            Return fluxDynamicsCache _
                .ToDictionary(Function(a) a.Key,
                                Function(a)
                                    Return Aggregate x In a Into Sum(x.Value)
                                End Function)
        End Function
    End Class
End Namespace
