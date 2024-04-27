#Region "Microsoft.VisualBasic::cf87dc2693caadf89027200847646028, G:/GCModeller/src/workbench/modules/ExperimentDesigner//DataAnalysis.vb"

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

    '   Total Lines: 90
    '    Code Lines: 52
    ' Comment Lines: 26
    '   Blank Lines: 12
    '     File Size: 2.79 KB


    ' Class DataAnalysis
    ' 
    '     Properties: control, designs, experiment, size
    ' 
    '     Function: ToString
    ' 
    ' Class DataGroup
    ' 
    '     Properties: color, sample_id, sampleGroup, shape
    ' 
    '     Function: CreateDataGroups
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' the different expression analysis design
''' </summary>
''' <remarks>
''' a collection of the sample <see cref="DataGroup"/> for run the different expression analysis.
''' usually be <see cref="experiment"/> vs <see cref="control"/>.
''' </remarks>
Public Class DataAnalysis

    ''' <summary>
    ''' show be contains at least two sample group
    ''' </summary>
    ''' <returns></returns>
    Public Property designs As DataGroup()

    ''' <summary>
    ''' number of sample groups to run analysis
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property size As Integer
        Get
            Return designs.TryCount
        End Get
    End Property

    Public ReadOnly Property experiment As DataGroup
        Get
            If size = 2 Then
                Return _designs(0)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property control As DataGroup
        Get
            If size = 2 Then
                Return _designs(1)
            Else
                Return Nothing
            End If
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return designs.Keys.JoinBy(" vs ")
    End Function

End Class

''' <summary>
''' the group view of the <see cref="SampleInfo"/> data
''' </summary>
Public Class DataGroup : Implements INamedValue

    ''' <summary>
    ''' the sample group label: <see cref="SampleInfo.sample_info"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleGroup As String Implements INamedValue.Key
    ''' <summary>
    ''' a collection of the sample id that belongs to current sample data group
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_id As String()
    Public Property color As String
    Public Property shape As String

    Public Shared Iterator Function CreateDataGroups(samples As IEnumerable(Of SampleInfo)) As IEnumerable(Of DataGroup)
        For Each group As IGrouping(Of String, SampleInfo) In samples.GroupBy(Function(s) s.sample_info)
            Dim template As SampleInfo = group.First

            Yield New DataGroup With {
                .sampleGroup = group.Key,
                .sample_id = group _
                    .Select(Function(s) s.ID) _
                    .ToArray,
                .color = template.color,
                .shape = template.shape
            }
        Next
    End Function

End Class
