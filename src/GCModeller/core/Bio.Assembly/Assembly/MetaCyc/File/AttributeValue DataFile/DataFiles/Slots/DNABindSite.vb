Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Reflection
Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' The class describes DNA regions that are binding sites for transcription factors.
    ''' (本对象描述了一个能够与转录因子相结合的DNA片段)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DNABindSite : Inherits MetaCyc.File.DataFiles.Slots.Object

        ''' <summary>
        ''' This slot defines the position on the replicon of the center of this binding site.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property AbsCenterPos As String

        ''' <summary>
        ''' This slot links the binding site to a Regulation frame describing the regulatory 
        ''' interaction in which this binding site participates.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>本属性指向Regulation对象</remarks>
        <ExternalKey("regulations", "", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property InvolvedInRegulation As List(Of String)

        ''' <summary>
        ''' This slot defines the extent of a binding site in base pairs. If a value for this 
        ''' slot is omitted, the site length will be computed based on the DNA-Footprint-Size 
        ''' of the binding protein. Thus, a value for this slot should only be supplied here 
        ''' if the site length for a particular transcription factor is not consistent across 
        ''' all its sites.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property SiteLength As String

        <ExternalKey("transunits", "", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property ComponentOf As List(Of String)

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.dnabindsites
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As DNABindSite
        '    Dim NewObj As DNABindSite = New DNABindSite

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of DNABindSite) _
        '        (MetaCyc.File.AttributeValue.Object.Format(DNABindSites.AttributeList, e), NewObj)

        '    If NewObj.Object.ContainsKey("SITE-LENGTH") Then NewObj.SiteLength = NewObj.Object("SITE-LENGTH") Else NewObj.SiteLength = String.Empty
        '    If NewObj.Object.ContainsKey("ABS-CENTER-POS") Then NewObj.AbsCenterPos = NewObj.Object("ABS-CENTER-POS") Else NewObj.AbsCenterPos = String.Empty
        '    NewObj.InvolvedInRegulation = StringQuery(NewObj.Object, "INVOLVED-IN-REGULATION( \d+)?")

        '    Return NewObj
        'End Operator
    End Class
End Namespace