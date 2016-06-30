Module Strings
    Public ReadOnly AppPath As String = My.Application.Info.DirectoryPath
    Public ReadOnly LocalStorage As String = AppPath & "/LocalStorage/"

    ''' <summary>
    ''' .../projs/
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Project As String = AppPath & "/Projs/"

    Public ReadOnly LicenseFile As String = AppPath & "/License.rtf"

    Public ReadOnly LogFile As String = Settings.LogDIR & "/dev2.log"

    Public Const BlankFlush As String = "                                                                                                                                                                                                                                                                   "
    ''' <summary>
    ''' (Logging Object) GCModeller
    ''' </summary>
    ''' <remarks></remarks>
    Public Const Modeller As String = "GCModeller"
    ''' <summary>
    ''' (Logging Object) LibMySQL
    ''' </summary>
    ''' <remarks></remarks>
    Public Const LibMySQL As String = "LibMySQL"
    ''' <summary>
    ''' (Logging Object) IDE
    ''' </summary>
    ''' <remarks></remarks>
    Public Const IDE As String = "IDE"

    Public ReadOnly Languages As String() = {"zh-CN", "en-US", "fr-FR"}
End Module
