#Region "Microsoft.VisualBasic::052c01b932da27725e27a6db05ecd480, core\Bio.Assembly\Assembly\MetaCyc\Schemas\Metabolic\Reaction.vb"

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

    '     Class Reaction
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MetaCyc.Schemas

    Public Class Reaction : Inherits SlotSchema(Of MetaCyc.File.DataFiles.Slots.Reaction)
        Public PhysiologicallyRelevant As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Compounds -----(Appears-in-left-side-of)----> Reaction
        ''' </remarks>
        Public Left As MetaCyc.Schemas.Compound()
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Compounds -----(Appears-in-right-side-of)-----> Reaction
        ''' </remarks>
        Public Right As MetaCyc.Schemas.Compound()
    End Class
End Namespace
