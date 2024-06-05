#Region "Microsoft.VisualBasic::7a15f46dd67e930f97876c5e9a480fdb, foundation\PSICQUIC\psidev\XML\[xsd]net.sf.psidev.mi\Nodes\Entry.vb"

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

    '   Total Lines: 57
    '    Code Lines: 50 (87.72%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (12.28%)
    '     File Size: 2.16 KB


    '     Class Entry
    ' 
    '         Properties: ExperimentList, InteractionList, InteractorList
    ' 
    '         Function: GetExperiment, GetInteractor, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace XML

    Public Class Entry

        <XmlArray("experimentList")> Public Property ExperimentList As ExperimentDescription()
            Get
                Return _experiments.Values.ToArray
            End Get
            Set(value As ExperimentDescription())
                If value.IsNullOrEmpty Then
                    _experiments = New Dictionary(Of Integer, ExperimentDescription)
                Else
                    _experiments = value.ToDictionary(Function(obj) obj.Id)
                End If
            End Set
        End Property
        <XmlArray("interactorList")> Public Property InteractorList As Interactor()
            Get
                Return _interactors.Values.ToArray
            End Get
            Set(value As Interactor())
                If value.IsNullOrEmpty Then
                    _interactors = New Dictionary(Of Integer, Interactor)
                Else
                    _interactors = value.ToDictionary(Function(obj) obj.Id)
                End If
            End Set
        End Property
        <XmlArray("interactionList")> Public Property InteractionList As Interaction()

        Dim _interactors As Dictionary(Of Integer, Interactor)
        Dim _experiments As Dictionary(Of Integer, ExperimentDescription)

        Public Function GetInteractor(handle As Integer) As Interactor
            If _interactors.ContainsKey(handle) Then
                Return _interactors(handle)
            Else
                Return Nothing
            End If
        End Function

        Public Function GetExperiment(handle As Integer) As ExperimentDescription
            If _experiments.ContainsKey(handle) Then
                Return _experiments(handle)
            Else
                Return Nothing
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Join(", ", (From obj In _interactors Select obj.Value.Synonym).ToArray)
        End Function
    End Class
End Namespace
