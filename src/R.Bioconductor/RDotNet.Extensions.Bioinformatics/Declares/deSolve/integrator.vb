#Region "Microsoft.VisualBasic::f8f6d382149e1eb5e986d0da34e2c228, ..\R.Bioconductor\RDotNet.Extensions.Bioinformatics\Declares\deSolve\integrator.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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
