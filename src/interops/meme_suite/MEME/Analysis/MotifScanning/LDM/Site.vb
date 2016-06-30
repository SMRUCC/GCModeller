Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.Similarity
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.ComponentModel
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.DatabaseServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Analysis.MotifScans

    <XmlType("regulon_site",
                               [Namespace]:="http://gcmodeller.org/annotationTools/meme/pwm/regprecise_sites")>
    Public Class Site : Inherits MEME.LDM.Site

        ''' <summary>
        ''' <see cref="Regprecise.WebServices.JSONLDM.regulator.vimssId"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("regulators")> Public Property Regulators As Integer()

        Public Shared Function CreateObject(site As MEME.LDM.Site) As Site
            Return New Site With {
                .Name = site.Name.Split("|"c).First,
                .Pvalue = site.Pvalue,
                .Site = site.Site,
                .Start = site.Start,
                .Right = site.Right
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strand">这个位点所处在的基因<see cref="Name"/>的链的方向</param>
        ''' <returns></returns>
        Public Function GetDist(strand As Strands) As Integer
            If strand = Strands.Forward Then
                Return Right
            Else
                Return Start
            End If
        End Function

        ''' <summary>
        ''' 假若这个位点的名称是基因号的话，可以使用这个方法得到基因组上面的位置，这个函数只适用于ATG上游的情况
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLoci(PTT As PTT) As NucleotideLocation
            Dim g As GeneBrief = PTT(Name)
            If g Is Nothing Then
                Return Nothing
            End If
            Dim loci As NucleotideLocation = g.Location.Normalization           ' ATG上游

            If loci.Strand = Strands.Forward Then
                Dim left As Long = loci.Left - Size + Me.Start
                Dim right As Long = left + Me.Length
                Return New NucleotideLocation(left, right, Strands.Forward)
            Else
                Dim left As Long = loci.Right + Me.Right
                Dim right As Long = left + Me.Length
                Return New NucleotideLocation(left, right, Strands.Reverse)
            End If
        End Function
    End Class
End Namespace