#Region "Microsoft.VisualBasic::ae31dfd4bd762e886dfcc60716521b04, modules\ExperimentDesigner\DataAnalysis.vb"

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

    '   Total Lines: 94
    '    Code Lines: 55 (58.51%)
    ' Comment Lines: 26 (27.66%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 13 (13.83%)
    '     File Size: 2.92 KB


    ' Class DataAnalysis
    ' 
    '     Properties: control, designs, experiment, size
    ' 
    '     Constructor: (+1 Overloads) Sub New
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

    Sub New(samples As IEnumerable(Of SampleInfo))
        designs = DataGroup.CreateDataGroups(samples).ToArray
    End Sub

    Sub New()
    End Sub

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

    Default Public ReadOnly Property Item(index As Integer) As String
        Get
            Return _sample_id(index)
        End Get
    End Property

    ''' <summary>
    ''' create a collection of the <see cref="DataGroup"/> from the given <see cref="SampleInfo"/>
    ''' </summary>
    ''' <param name="samples"></param>
    ''' <returns></returns>
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

    Public Shared Function NameGroups(samples As IEnumerable(Of SampleInfo)) As Dictionary(Of String, String())
        Return samples _
            .GroupBy(Function(s) s.sample_info) _
            .ToDictionary(Function(g) g.Key,
                          Function(g)
                              Return g.Select(Function(s) s.ID).ToArray
                          End Function)
    End Function


End Class
