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
