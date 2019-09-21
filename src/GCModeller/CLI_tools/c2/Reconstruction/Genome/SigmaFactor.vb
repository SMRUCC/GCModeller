#Region "Microsoft.VisualBasic::f1a88dd605b20543ada15d4bdd84d318, CLI_tools\c2\Reconstruction\Genome\SigmaFactor.vb"

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

    '     Class SigmaFactor
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Performance
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Reconstruction

    Public Class SigmaFactor : Inherits Operation

        Sub New(Session As OperationSession)
            Call MyBase.New(Session)
        End Sub

        Public Overrides Function Performance() As Integer
            Dim sbjSigmaFactors = (From Protein In MyBase.Subject.GetProteins Where Protein.Types.IndexOf("Sigma-Factors") > -1 Select Protein).ToArray
            Throw New NotImplementedException
        End Function
    End Class
End Namespace
