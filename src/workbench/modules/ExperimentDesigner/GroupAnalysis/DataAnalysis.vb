#Region "Microsoft.VisualBasic::1313884a71b3d8d40d7b6baeff3357ea, modules\ExperimentDesigner\GroupAnalysis\DataAnalysis.vb"

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

    '   Total Lines: 67
    '    Code Lines: 43 (64.18%)
    ' Comment Lines: 15 (22.39%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (13.43%)
    '     File Size: 1.95 KB


    ' Class DataAnalysis
    ' 
    '     Properties: control, designs, experiment, size
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: GenericEnumerator, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' the different expression analysis design
''' </summary>
''' <remarks>
''' a collection of the sample <see cref="DataGroup"/> for run the different expression analysis.
''' usually be <see cref="experiment"/> vs <see cref="control"/>.
''' </remarks>
Public Class DataAnalysis : Implements Enumeration(Of DataGroup)

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

    Public Iterator Function GenericEnumerator() As IEnumerator(Of DataGroup) Implements Enumeration(Of DataGroup).GenericEnumerator
        For Each group As DataGroup In designs
            Yield group
        Next
    End Function
End Class
