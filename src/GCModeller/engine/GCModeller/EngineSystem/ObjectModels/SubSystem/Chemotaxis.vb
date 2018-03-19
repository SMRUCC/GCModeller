#Region "Microsoft.VisualBasic::cc6bf7f048fb4e5ab9a89303e076aad3, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\Chemotaxis.vb"

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

    '     Class Chemotaxis
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __innerTicks, CreateServiceSerials, Initialize
    ' 
    '         Sub: MemoryDump
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace EngineSystem.ObjectModels.SubSystem

    Public Class Chemotaxis : Inherits EngineSystem.ObjectModels.SubSystem.CellComponentSystemFramework(Of EngineSystem.ObjectModels.Module.Chemotaxis)

        Sub New(Metabolism As SubSystem.MetabolismCompartment, CultivationMediums As SubSystem.CultivationMediums)
            Call MyBase.New(Metabolism)
        End Sub

        Public Overrides Function CreateServiceSerials() As Services.MySQL.IDataAcquisitionService()
            Throw New NotImplementedException
        End Function

        Public Overrides Function Initialize() As Integer
            Throw New NotImplementedException
        End Function

        Public Overrides Sub MemoryDump(Dir As String)

        End Sub

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Return 0
        End Function
    End Class
End Namespace
