#Region "Microsoft.VisualBasic::76e11d527c39d266e3371f5d6e547408, analysis\Metagenome\Metagenome\BEBaC\Model.vb"

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

    '   Total Lines: 30
    '    Code Lines: 24 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (20.00%)
    '     File Size: 897 B


    '     Class I3merVector
    ' 
    '         Properties: Frequency, Name, Vector
    ' 
    '     Class Cluster
    ' 
    '         Properties: members
    ' 
    '         Function: PartitionProbability
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataMining.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace BEBaC

    Public Class I3merVector : Inherits EntityBase(Of Double)
        Public Property Name As String
        Public Property Vector As Dictionary(Of I3Mers, Integer)
        Public Property Frequency As Dictionary(Of I3Mers, Double)
            Get
                Return f
            End Get
            Set(value As Dictionary(Of I3Mers, Double))
                f = value
                entityVector = f.Values.ToArray
            End Set
        End Property

        Dim f As Dictionary(Of I3Mers, Double)
    End Class

    Public Class Cluster

        Public Property members As List(Of I3merVector)

        Public Function PartitionProbability() As Double
            Return members.Probability
        End Function
    End Class
End Namespace
