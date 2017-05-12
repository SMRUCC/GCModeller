Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/MapPlot.exe

Namespace GCModellerApps


    ''' <summary>
    '''Map.CLI
    ''' </summary>
    '''
    Public Class MapPlot : Inherits InteropService


        Sub New(App$)
            MyBase._executableAssembly = App$
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Config_Template(Optional _out As String = "") As Integer
            Dim CLI$ = $"/Config.Template /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Visualize the blastp result.
        ''' </summary>
        '''
        Public Function Visual_BBH(_in As String, _PTT As String, _density As String, Optional _limits As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Visual.BBH /in ""{_in}"" /PTT ""{_PTT}"" /density ""{_density}"" /limits ""{_limits}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Blastn result alignment visualization from the NCBI web blast. This tools is only works for a plasmid blastn search result or a small gene cluster region in a large genome.
        ''' </summary>
        '''
        Public Function Visualize_blastn_alignment(_in As String, _genbank As String, Optional _orf_catagory As String = "", Optional _region As String = "", Optional _out As String = "", Optional _local As Boolean = False) As Integer
            Dim CLI$ = $"/Visualize.blastn.alignment /in ""{_in}"" /genbank ""{_genbank}"" /orf.catagory ""{_orf_catagory}"" /region ""{_region}"" /out ""{_out}"" {If(_local, "/local", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Drawing the chromosomes map from the PTT object as the basically genome information source.
        ''' </summary>
        '''
        Public Function Draw_ChromosomeMap(_ptt As String, Optional _conf As String = "", Optional _out As String = "", Optional _cog As String = "") As Integer
            Dim CLI$ = $"--Draw.ChromosomeMap /ptt ""{_ptt}"" /conf ""{_conf}"" /out ""{_out}"" /cog ""{_cog}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Draw_ChromosomeMap_genbank(_gb As String, Optional _conf As String = "", Optional _out As String = "", Optional _cog As String = "") As Integer
            Dim CLI$ = $"--Draw.ChromosomeMap.genbank /gb ""{_gb}"" /conf ""{_conf}"" /out ""{_out}"" /cog ""{_cog}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function
    End Class
End Namespace
