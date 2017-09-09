Imports System.Drawing
Imports System.Runtime.CompilerServices

Public Module Extensions

    ''' <summary>
    ''' ``<see cref="SampleInfo.ID"/> -> <see cref="SampleInfo.sample_group"/>``
    ''' </summary>
    ''' <param name="sampleInfo"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SampleGroupInfo(sampleInfo As IEnumerable(Of SampleInfo)) As Dictionary(Of String, String)
        Return sampleInfo.ToDictionary(
            Function(sample) sample.ID,
            Function(sample) sample.sample_group)
    End Function

    <Extension>
    Public Function SampleGroupColor(sampleInfo As IEnumerable(Of SampleInfo)) As Dictionary(Of String, Color)

    End Function
End Module
