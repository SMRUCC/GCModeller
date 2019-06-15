Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Visualize.Circos.Colors

Module COGColors

    <ExportAPI("COG.Colors")>
    Public Function GetCogColorProfile(MyvaCOG As MyvaCOG(), defaultColor As String) As Func(Of String, String)
        Dim COGs As String() =
                LinqAPI.Exec(Of String) <= From gene As MyvaCOG
                                           In MyvaCOG
                                           Let cId As String = gene.COG
                                           Where Not String.IsNullOrEmpty(cId)
                                           Select cId.ToUpper
                                           Distinct
        Dim ColorProfiles As New Dictionary(Of String, String)
        Dim Colors = PerlColor.Colors.Shuffles.AsList
        Dim i As Integer = 0

        Call Colors.Remove(defaultColor.ToLower)

        For i = Colors.Count To COGs.Length + 10
            Call Colors.Add("Color_" & i)
        Next

        i = 0

        For Each strId As String In COGs
            Dim ColorName As String = Colors(i)

            i += 1
            Call ColorProfiles.Add(strId, ColorName)
        Next

        Return AddressOf New MvyaColorProfile(MyvaCOG, ColorProfiles, defaultColor).GetColor
    End Function

End Module
