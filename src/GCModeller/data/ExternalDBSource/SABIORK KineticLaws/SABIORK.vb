#Region "Microsoft.VisualBasic::e41196ff165fb36c31fcd06640a88604, ExternalDBSource\SABIORK KineticLaws\SABIORK.vb"

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

    '     Class SABIORK
    ' 
    '         Properties: Buffer, CompoundSpecies, Fast, Identifiers, kineticLawID
    '                     LocalParameters, startValuepH, startValueTemperature
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace SabiorkKineticLaws

    Public Class SABIORK : Inherits Equation

        ''' <summary>
        ''' http://sabiork.h-its.org/sabioRestWebServices/kineticLaws?kinlawids=
        ''' </summary>
        Public Const URL_SABIORK_KINETIC_LAWS_QUERY As String = "http://sabiork.h-its.org/sabioRestWebServices/kineticLaws?kinlawids="

        Public Property kineticLawID As Long
        Public Property startValuepH As Double
        Public Property startValueTemperature As Double
        Public Property Buffer As String
        Public Property Fast As Boolean

        Public Property CompoundSpecies As SBMLParser.CompoundSpecie()
        Public Property Identifiers As String()
        Public Property LocalParameters As [Property]()

        Public Overrides Function ToString() As String
            Return kineticLawID
        End Function
    End Class
End Namespace
