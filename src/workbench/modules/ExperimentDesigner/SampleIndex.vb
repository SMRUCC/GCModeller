#Region "Microsoft.VisualBasic::2fbf9f0ef357cefe385a37c3b9846d55, modules\ExperimentDesigner\SampleIndex.vb"

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

    '   Total Lines: 51
    '    Code Lines: 41 (80.39%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (19.61%)
    '     File Size: 1.56 KB


    ' Class SampleIndex
    ' 
    '     Properties: group_size, sampleInfo, size
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetSampleClass, GetSampleColor, GetSampleName
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Public Class SampleIndex

    Public ReadOnly Property sampleInfo As SampleInfo()
        Get
            Return m_sampleinfo
        End Get
    End Property

    ReadOnly m_sampleinfo As SampleInfo()
    ReadOnly m_sampleindex As Dictionary(Of String, SampleInfo)
    ReadOnly m_samplegroups As DataAnalysis

    Public ReadOnly Property size As Integer
        Get
            Return m_sampleinfo.Length
        End Get
    End Property

    Public ReadOnly Property group_size As Integer
        Get
            Return m_samplegroups.size
        End Get
    End Property

    Sub New(sampleinfo As IEnumerable(Of SampleInfo))
        m_sampleinfo = sampleinfo.SafeQuery.ToArray
        m_sampleindex = sampleinfo.ToDictionary(Function(s) s.ID)
        m_samplegroups = New DataAnalysis(m_sampleinfo)
    End Sub

    Public Function GetSampleName(sample_id As IEnumerable(Of String)) As String()
        Return sample_id _
            .Select(Function(id) m_sampleindex(id).sample_name) _
            .ToArray
    End Function

    Public Function GetSampleClass(sample_id As IEnumerable(Of String)) As String()
        Return sample_id _
            .Select(Function(id) m_sampleindex(id).sample_info) _
            .ToArray
    End Function

    Public Function GetSampleColor(sample_id As IEnumerable(Of String)) As String()
        Return sample_id _
            .Select(Function(id) m_sampleindex(id).color) _
            .ToArray
    End Function

End Class

