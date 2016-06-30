Attribute VB_Name = "modProgram"
Sub Main()

    If Len(Command) = 0 Then
        MsgBox "Program Argument Error!", vbCritical
        Exit Sub
    End If
    
    Dim ChunkBuffer() As String, Cmdl As String
    
    Cmdl = Command
    ChunkBuffer = Split(Cmdl, """ """)
    
    Dim ProgramPath As String, IconPath As String
    
    ProgramPath = ChunkBuffer(0)
    IconPath = ChunkBuffer(1)
   
    ProgramPath = Mid(ProgramPath, 2)
    IconPath = Mid(IconPath, 1, Len(IconPath) - 1)
       
    Call CreateAssociation(".shl", "Shoal Shell Script File", ProgramPath, IconPath)
    
End Sub
