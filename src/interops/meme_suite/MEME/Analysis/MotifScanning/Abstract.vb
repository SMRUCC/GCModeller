#Region "Microsoft.VisualBasic::6357804ea25f5a6c7abe84b05ebc0dea, meme_suite\MEME\Analysis\MotifScanning\Abstract.vb"

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

    '     Interface IMotifTrace
    ' 
    '         Properties: MotifTrace
    ' 
    '     Interface IFootprintTrace
    ' 
    '         Properties: RegulatorTrace
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Analysis.MotifScans

    ''' <summary>
    ''' 应用于调控位点的
    ''' </summary>
    Public Interface IMotifTrace
        Property MotifTrace As String
    End Interface

    ''' <summary>
    ''' 应用于调控作用的
    ''' </summary>
    Public Interface IFootprintTrace : Inherits IMotifTrace
        Property RegulatorTrace As String
    End Interface
End Namespace
