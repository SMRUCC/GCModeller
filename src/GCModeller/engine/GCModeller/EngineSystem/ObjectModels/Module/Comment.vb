#Region "Microsoft.VisualBasic::682e2cee4f5f4d58682e3f5ed9ba0952, engine\GCModeller\EngineSystem\ObjectModels\Module\Comment.vb"

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

    '     Module Comment
    ' 
    '         Properties: InteractionAlgorithm_CommentText
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace EngineSystem.ObjectModels.Module

    ''' <summary>
    ''' Comment for the <see cref="EngineSystem.ObjectModels.Module"></see> namespace
    ''' </summary>
    ''' <remarks></remarks>
    Module Comment

        ''' <summary>
        ''' 计算原理的解释
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property InteractionAlgorithm_CommentText As String
            Get
                Return _
<DFL>
The working algorithm of the GCModeller is base on the dynamics fuzzy logic. 
Why chose DFL? here is the reason:

1. The events in the living system is a probability event, the probabilities of a molecule 
interact with another molecule depends on the molecule quantity in a small unit compartment.
As the biochemical can 
</DFL>
            End Get
        End Property
    End Module
End Namespace
