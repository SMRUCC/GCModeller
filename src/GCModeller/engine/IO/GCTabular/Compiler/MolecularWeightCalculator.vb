#Region "Microsoft.VisualBasic::30d211233993632c4e09b066a33c09c0, engine\IO\GCTabular\Compiler\MolecularWeightCalculator.vb"

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

    '     Class MolecularWeightCalculator
    ' 
    '         Sub: CalculateK
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine

Namespace Compiler.Components

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MolecularWeightCalculator

        Public Sub CalculateK(ModelLoader As FileStream.IO.XmlresxLoader)
            Dim MetabolismModels = ModelLoader.MetabolismModel
            Dim Metabolites = ModelLoader.MetabolitesModel

            For i As Integer = 0 To MetabolismModels.Count - 1
                Dim FluxModel = MetabolismModels(i)
                Dim LEFT = (From item In FluxModel._Internal_compilerLeft Select Metabolites(item.Value).MolWeight).ToArray
                If (From n In LEFT Where n = 0.0R Select 1).ToArray.Count > 0 Then
                    Continue For
                End If
                Dim RIGHT = (From item In FluxModel._Internal_compilerRight Select Metabolites(item.Value).MolWeight).ToArray
                If (From n In RIGHT Where n = 0.0R Select 1).ToArray.Count > 0 Then
                    Continue For
                End If

                Dim nLEFT = LEFT.Average, nRIGHT = RIGHT.Average
                FluxModel.p_Dynamics_K_1 = nLEFT / nRIGHT
                FluxModel.p_Dynamics_K_2 = nRIGHT / nLEFT
            Next
        End Sub
    End Class
End Namespace
