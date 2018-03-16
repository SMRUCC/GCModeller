#Region "Microsoft.VisualBasic::166ce0917d015a56ea24c9269bb0442a, RDotNET\Graphics\MetricsInfo.vb"

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

    '     Structure MetricsInfo
    ' 
    '         Properties: Ascent, Descent, Width
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Graphics

    Public Structure MetricsInfo
        Public Property Ascent() As Double
            Get
                Return m_Ascent
            End Get
            Set(value As Double)
                m_Ascent = value
            End Set
        End Property
        Private m_Ascent As Double

        Public Property Descent() As Double
            Get
                Return m_Descent
            End Get
            Set(value As Double)
                m_Descent = value
            End Set
        End Property
        Private m_Descent As Double

        Public Property Width() As Double
            Get
                Return m_Width
            End Get
            Set(value As Double)
                m_Width = value
            End Set
        End Property
        Private m_Width As Double
    End Structure
End Namespace
