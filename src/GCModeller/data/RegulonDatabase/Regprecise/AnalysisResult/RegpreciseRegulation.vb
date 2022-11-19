#Region "Microsoft.VisualBasic::398ef9ff24bab726f3373a57d6df4429, GCModeller\data\RegulonDatabase\Regprecise\AnalysisResult\RegpreciseRegulation.vb"

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


    ' Code Statistics:

    '   Total Lines: 43
    '    Code Lines: 30
    ' Comment Lines: 4
    '   Blank Lines: 9
    '     File Size: 1.82 KB


    '     Module RegulationModels
    ' 
    '         Function: ToString
    '         Class OperonRegulation
    ' 
    '             Properties: Door, GeneId, RegulationMode, Regulator
    ' 
    '             Function: ToString
    ' 
    '         Interface IRegulationModel
    ' 
    '             Properties: Regulated, RegulationMode, Regulator
    ' 
    '         Class RegpreciseRegulation
    ' 
    '             Properties: Gene, RegulationMode, Regulator
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace RegulonDatabase

    ''' <summary>
    ''' Write for csv file
    ''' </summary>
    ''' <remarks></remarks>
    Public Module RegulationModels

        Public Class OperonRegulation : Implements IRegulationModel

            Public Property Regulator As String Implements IRegulationModel.Regulator
            Public Property Door As String Implements IRegulationModel.Regulated
            Public Property GeneId As String()
            Public Property RegulationMode As String Implements IRegulationModel.RegulationMode

            Public Overrides Function ToString() As String
                Return RegulationModels.ToString(Me) & String.Format(" [{0}]", String.Join("; ", GeneId))
            End Function
        End Class

        Public Interface IRegulationModel
            Property Regulator As String
            Property Regulated As String
            Property RegulationMode As String
        End Interface

        Public Function ToString(RegulationModel As IRegulationModel) As String
            Dim Regulation As String = If(String.IsNullOrEmpty(RegulationModel.RegulationMode), "Regulates", RegulationModel.RegulationMode)
            Return String.Join(" ", New String() {RegulationModel.Regulator, Regulation, RegulationModel.Regulated})
        End Function

        Public Class RegpreciseRegulation : Implements IRegulationModel

            Public Property Regulator As String Implements IRegulationModel.Regulator
            Public Property Gene As String Implements IRegulationModel.Regulated
            Public Property RegulationMode As String Implements IRegulationModel.RegulationMode

            Public Overrides Function ToString() As String
                Return RegulationModels.ToString(Me)
            End Function
        End Class
    End Module
End Namespace
