#Region "Microsoft.VisualBasic::ed6ab4f6d46dd9eed2c29f17a632d0ea, RDotNet.Extensions.Bioinformatics\Declares\deSolve\integrator.vb"

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

    '     Enum integrator
    ' 
    '         adams, bdf, bdf_d, daspk, euler
    '         impAdams, impAdams_d, iteration, lsoda, lsodar
    '         lsode, lsodes, ode23, ode45, radau
    '         rk4, vode
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace deSolve

    Public Enum integrator
        lsoda
        lsode
        lsodes
        lsodar
        vode
        daspk
        euler
        rk4
        ode23
        ode45
        radau
        bdf
        bdf_d
        adams
        impAdams
        impAdams_d
        iteration
    End Enum
End Namespace
