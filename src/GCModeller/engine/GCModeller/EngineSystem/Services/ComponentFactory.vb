#Region "Microsoft.VisualBasic::74ee350e2c889a776c174a0c3d004a95, engine\GCModeller\EngineSystem\Services\ComponentFactory.vb"

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

    '     Class ComponentFactory
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateObject
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace EngineSystem.Services

    Public Class ComponentFactory : Inherits RuntimeObject

        Dim _ModelSource As EngineSystem.ObjectModels.SubSystem.CellSystem

        Sub New(ModelSource As EngineSystem.ObjectModels.SubSystem.CellSystem)
            _ModelSource = ModelSource
        End Sub

#Region "EngineSystem.ObjectModels.Entity"
        Public Function CreateObject() As EngineSystem.ObjectModels.Entity.Compound
            Throw New NotImplementedException
        End Function
#End Region
    End Class
End Namespace
